using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;
using System.Text;

//giving a proper namespace to your script prevents clashes with other assets and developers.
namespace Com.ESGI.RoyalStar
{
    //Notice that we are deriving our class from MonoBehaviour which essentially turns
    //our class into an Unity Component that we can then drop onto a GameObject or Prefab.
    public class Launcher : MonoBehaviourPunCallbacks
    {
        //This client's version number.
        //Users are separated from each other by gameversion (which allows you to make breaking changes).
        private string gameVersion = "1";

        [Tooltip("The maximum number of players per room. When a room is full, " +
                 "it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayerPerRoom = 4;
        
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        private bool isConnecting = false;
        
        // MonoBehaviour method called on GameObject by Unity during initialization phase.
        private void Awake()
        {   
            Debug.Log("PUN Royal Star/Launcher: Awake() was called");
            // this makes sure we can use PhotonNetwork.LoadLevel()
            // on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            Debug.Log("PUN Royal Star/Launcher: Start() was called");
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        // Start the connection process.
        // - If already connected, we attempt joining a random room
        // - if not yet connected, Connect this application instance to Photon Cloud Network
        public void Connect()
        {
            Debug.Log("PUN Royal Star/Launcher: Connect() was called");
            isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are,
            // else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN Royal Star/Launcher: Connect() 'IsConnected' was triggered");
                //we need at this point to attempt joining a Random Room.
                //If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                Debug.Log("PUN Royal Star/Launcher: Connect() 'NotConnected' was triggered");
                //we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                //Connection au serveur, active normalement "OnConnectedToMaster()"
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        
        
        //Permet de savoir si on est connecté au clood ou non.
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Royal Star/Launcher: OnConnectedToMaster() was called by PUN");
            //The first we try to do is to join a potential existing room.
            //If there is, good, else, we'll be called back with OnJoinRandomFailed()
            
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when
            // this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                Debug.Log("PUN Royal Star/Launcher: Try to join a room");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Royal Star/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Royal Star/Launcher:OnJoinRandomFailed() was called by PUN. " +
                      "No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            //We create a room a join it.
            PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayerPerRoom });
        }
        
        //We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene`
        //to sync our instance scene.
        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Royal Star/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // We load the game only if we are the master client
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the Game");
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Game");
            }
        }

        public override void OnLeftRoom()
        {
            Debug.Log("PUN Royal Star/Launcher: OnLeftRoom() called by PUN. Now this client quit the room.");
        }
    }   
}
