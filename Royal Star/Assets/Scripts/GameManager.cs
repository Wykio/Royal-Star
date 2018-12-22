using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;

namespace Com.ESGI.RoyalStar
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0} players", PhotonNetwork.CurrentRoom.PlayerCount);
            //We use PhotonNetwork.LoadLevel() to load the level we want,
            //we don't use Unity directly, because we want to rely on Photon
            //to load this level on all connected clients in the room, since we've enabled
            //PhotonNetwork.AutomaticallySyncScene for this Game.
            //PhotonNetwork.LoadLevel("Game");
        }
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }    
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }
        // Called when the local player left the room. We need to load the launcher scene.
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
