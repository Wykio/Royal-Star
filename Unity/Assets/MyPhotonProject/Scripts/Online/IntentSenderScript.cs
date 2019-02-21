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
        Debug.Log(PlayerNumbering.SortedPlayers.Length );
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
            photonView.RPC("WantToGoForward", RpcTarget.MasterClient, true);
            photonView.RPC("AirPitchDown", RpcTarget.MasterClient, true);
            photonView.RPC("BoostForward", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            photonView.RPC("WantToGoForward", RpcTarget.MasterClient, false);
            photonView.RPC("AirPitchDown", RpcTarget.MasterClient, false);
            photonView.RPC("BoostForward", RpcTarget.MasterClient, false);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            photonView.RPC("WantToGoBackward", RpcTarget.MasterClient, true);
            photonView.RPC("AirPitchUp", RpcTarget.MasterClient, true);
            photonView.RPC("BoostBackward", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            photonView.RPC("WantToGoBackward", RpcTarget.MasterClient, false);
            photonView.RPC("AirPitchUp", RpcTarget.MasterClient, false);
            photonView.RPC("BoostBackward", RpcTarget.MasterClient, false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("WantToStrafeLeft", RpcTarget.MasterClient, true);
            photonView.RPC("AirRollLeft", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            photonView.RPC("WantToStrafeLeft", RpcTarget.MasterClient, false);
            photonView.RPC("AirRollLeft", RpcTarget.MasterClient, false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            photonView.RPC("WantToStrafeRight", RpcTarget.MasterClient, true);
            photonView.RPC("AirRollLeft", RpcTarget.MasterClient, true);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            photonView.RPC("WantToStrafeRight", RpcTarget.MasterClient, false);
            photonView.RPC("AirRollLeft", RpcTarget.MasterClient, false);
        }

        if (sourisHorizontale != null)
        {
            photonView.RPC("WantToTurn", RpcTarget.MasterClient, sourisHorizontale);
            photonView.RPC("BoostTurn", RpcTarget.MasterClient, sourisHorizontale);
        }

        if(sourisVerticale != null)
        {
            photonView.RPC("BoostPitch", RpcTarget.MasterClient, sourisVerticale);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            photonView.RPC("AirBoostActivate", RpcTarget.MasterClient, true);
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
