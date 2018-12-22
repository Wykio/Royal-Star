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
        
        // MonoBehaviour method called on GameObject by Unity during initialization phase.
        private void Awake()
        {   
            // this makes sure we can use PhotonNetwork.LoadLevel()
            // on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        // Start the connection process.
        // - If already connected, we attempt joining a random room
        // - if not yet connected, Connect this application instance to Photon Cloud Network
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are,
            // else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                //we need at this point to attempt joining a Random Room.
                //If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                //we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Royal Star/Launcher: OnConnectedToMaster() was called by PUN");
            //The first we try to do is to join a potential existing room.
            //If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
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
            PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayerPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Royal Star/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }
    }   
}
