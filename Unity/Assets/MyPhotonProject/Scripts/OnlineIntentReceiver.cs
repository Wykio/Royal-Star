using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace MyPhotonProject.Scripts
{
    public class OnlineIntentReceiver : AIntentReceiver
    {
        [FormerlySerializedAs("PlayerActorId")]
        [SerializeField]
        private int PlayerIndex;

        [SerializeField]
        private PhotonView photonView;

        public void Update()
        {
            if (PlayerNumbering.SortedPlayers.Length <= PlayerIndex || 
                PlayerNumbering.SortedPlayers[PlayerIndex].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                photonView.RPC("WantToMoveLeftRPC", RpcTarget.MasterClient, true);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                photonView.RPC("WantToMoveBackRPC", RpcTarget.MasterClient, true);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                photonView.RPC("WantToMoveRightRPC", RpcTarget.MasterClient, true);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                photonView.RPC("WantToMoveForwardRPC", RpcTarget.MasterClient, true);
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                photonView.RPC("WantToMoveLeftRPC", RpcTarget.MasterClient, false);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                photonView.RPC("WantToMoveBackRPC", RpcTarget.MasterClient, false);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                photonView.RPC("WantToMoveRightRPC", RpcTarget.MasterClient, false);
            }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                photonView.RPC("WantToMoveForwardRPC", RpcTarget.MasterClient, false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                photonView.RPC("WantToBumpRPC", RpcTarget.MasterClient);
            }
        }

        [PunRPC]
        void WantToMoveLeftRPC(bool intent)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WantToMoveLeft = intent;
            }
        }

        [PunRPC]
        void WantToMoveBackRPC(bool intent)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WantToMoveBack = intent;
            }
        }

        [PunRPC]
        void WantToMoveRightRPC(bool intent)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WantToMoveRight = intent;
            }
        }

        [PunRPC]
        void WantToMoveForwardRPC(bool intent)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WantToMoveForward = intent;
            }
        }

        [PunRPC]
        void WantToBumpRPC()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WantToBumpIntent = true;
            }
        }
    }
}