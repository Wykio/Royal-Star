using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
        
        // MonoBehaviour method called on GameObject by Unity during initialization phase.
        private void Awake()
        {   
            // this makes sure we can use PhotonNetwork.LoadLevel()
            // on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            Connect();
        }
        
        // Start the connection process.
        // - If already connected, we attempt joining a random room
        // - if not yet connected, Connect this application instance to Photon Cloud Network
        public void Connect()
        {
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
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Royal Star/Launcher:OnJoinRandomFailed() was called by PUN. " +
                      "No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            PhotonNetwork.CreateRoom(null, new RoomOptions());
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Royal Star/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }
    }   
}
