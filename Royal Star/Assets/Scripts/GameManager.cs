using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;

namespace Com.ESGI.RoyalStar
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        }
        
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        }
        
        // Called when the local player left the room. We need to load the launcher scene.
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        
        //LeaveRoom() is called when you hit the button in the game
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
