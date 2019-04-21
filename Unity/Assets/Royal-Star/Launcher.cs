using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.ESGI.Royal_Star
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connect Using Settings");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined a room");
            if (PhotonNetwork.IsMasterClient)
            {
                //StartCoroutine(InformPlayerJoinedEndOfFrame(newPlayer.ActorNumber));
                Debug.Log("IM the master");
            } 
        }

        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Created a room");
            PhotonNetwork.CreateRoom(null, new RoomOptions
            {
                MaxPlayers = 4,
                PlayerTtl = 10000 //?
            });
        }
    }   
}
