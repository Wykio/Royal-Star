using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class IntentSenderScript : AIntentReceiver
{
    [FormerlySerializedAs("PlayerActorId")]
    [SerializeField] private int IndiceJoueur;

    [SerializeField] PhotonView photonView;

    public void Update()
    {
        
        //si le joueur n'est pas dans la liste des joueurs ou que son numéro ne correspond pas au numéro de son indice
        if (PlayerNumbering.SortedPlayers.Length <= IndiceJoueur ||
                PlayerNumbering.SortedPlayers[IndiceJoueur].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        //récupération des axes de souris
        var sourisVerticale = -Input.GetAxis("Mouse Y");
        var sourisHorizontale = Input.GetAxis("Mouse X");

        #region Inputs
        if (Input.GetKeyDown(KeyCode.Z))
        {
            photonView.RPC("WantToGoForwardRPC", RpcTarget.MasterClient, true);
            photonView.RPC("AirPitchDownRPC", RpcTarget.MasterClient, true);
            photonView.RPC("BoostForwardRPC", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            photonView.RPC("WantToGoForwardRPC", RpcTarget.MasterClient, false);
            photonView.RPC("AirPitchDownRPC", RpcTarget.MasterClient, false);
            photonView.RPC("BoostForwardRPC", RpcTarget.MasterClient, false);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            photonView.RPC("WantToGoBackwardRPC", RpcTarget.MasterClient, true);
            photonView.RPC("AirPitchUpRPC", RpcTarget.MasterClient, true);
            photonView.RPC("BoostBackwardRPC", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            photonView.RPC("WantToGoBackwardRPC", RpcTarget.MasterClient, false);
            photonView.RPC("AirPitchUpRPC", RpcTarget.MasterClient, false);
            photonView.RPC("BoostBackwardRPC", RpcTarget.MasterClient, false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("WantToStrafeLeftRPC", RpcTarget.MasterClient, true);
            photonView.RPC("AirRollLeftRPC", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            photonView.RPC("WantToStrafeLeftRPC", RpcTarget.MasterClient, false);
            photonView.RPC("AirRollLeftRPC", RpcTarget.MasterClient, false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            photonView.RPC("WantToStrafeRightRPC", RpcTarget.MasterClient, true);
            photonView.RPC("AirRollRightRPC", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            photonView.RPC("WantToStrafeRightRPC", RpcTarget.MasterClient, false);
            photonView.RPC("AirRollRightRPC", RpcTarget.MasterClient, false);
        }

        if (sourisHorizontale != 0f)
        {
            photonView.RPC("WantToTurnRPC", RpcTarget.MasterClient, sourisHorizontale);
            photonView.RPC("BoostTurnRPC", RpcTarget.MasterClient, sourisHorizontale);
        }

        if(sourisVerticale != 0f)
        {
            photonView.RPC("BoostPitchRPC", RpcTarget.MasterClient, sourisVerticale);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            photonView.RPC("AirBoostActivateRPC", RpcTarget.MasterClient, true);
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            photonView.RPC("AirBoostActivateRPC", RpcTarget.MasterClient, false);
        }
        #endregion
    }

    #region [PunRPC]
    [PunRPC]
    void WantToGoForwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            WantToGoForward = intent;
        }
    }

    [PunRPC]
    void WantToGoBackwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            WantToGoBackward = intent;
        }
    }

    [PunRPC]
    void WantToStrafeLeftRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            WantToStrafeLeft = intent;
        }
    }

    [PunRPC]
    void WantToStrafeRightRPC(bool intent)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            WantToStrafeRight = intent;
        }
    }

    [PunRPC]
    void WantToTurnRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            WantToTurn = intent;
        }
    }

    [PunRPC]
    void AirPitchUpRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            AirPicthUp = intent;
        }
    }

    [PunRPC]
    void AirPitchDownRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            AirPitchDown = intent;
        }
    }

    [PunRPC]
    void AirRollRightRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            AirRollRight = intent;
        }
    }

    [PunRPC]
    void AirRollLeftRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            AirRollLeft = intent;
        }
    }

    [PunRPC]
    void AirBoostActivateRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            AirBoostActivate = intent;
        }
    }

    [PunRPC]
    void BoostForwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            BoostForward = intent;
        }
    }

    [PunRPC]
    void BoostBackwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            BoostBackward = intent;
        }
    }

    [PunRPC]
    void BoostTurnRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            BoostTurn = intent;
        }
    }

    [PunRPC]
    void BoostPitchRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            BoostPicht = intent;
        }
    }

    #endregion
}
