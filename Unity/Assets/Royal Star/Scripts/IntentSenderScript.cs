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
    [SerializeField] private bool prediction = true;

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
            WantToGoForwardRPC(true);
            photonView.RPC("BoostForwardRPC", RpcTarget.MasterClient, true);
            BoostForwardRPC(true);
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            photonView.RPC("WantToGoForwardRPC", RpcTarget.MasterClient, false);
            WantToGoForwardRPC(false);
            photonView.RPC("BoostForwardRPC", RpcTarget.MasterClient, false);
            BoostForwardRPC(false);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            photonView.RPC("WantToGoBackwardRPC", RpcTarget.MasterClient, true);
            WantToGoBackwardRPC(true);
            photonView.RPC("BoostBackwardRPC", RpcTarget.MasterClient, true);
            BoostBackwardRPC(true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            photonView.RPC("WantToGoBackwardRPC", RpcTarget.MasterClient, false);
            WantToGoBackwardRPC(false);
            photonView.RPC("BoostBackwardRPC", RpcTarget.MasterClient, false);
            BoostBackwardRPC(false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("WantToStrafeLeftRPC", RpcTarget.MasterClient, true);
            WantToStrafeLeftRPC(true);
            photonView.RPC("AirRollLeftRPC", RpcTarget.MasterClient, true);
            AirRollLeftRPC(true);
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            photonView.RPC("WantToStrafeLeftRPC", RpcTarget.MasterClient, false);
            WantToStrafeLeftRPC(false);
            photonView.RPC("AirRollLeftRPC", RpcTarget.MasterClient, false);
            AirRollLeftRPC(false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            photonView.RPC("WantToStrafeRightRPC", RpcTarget.MasterClient, true);
            WantToStrafeRightRPC(true);
            photonView.RPC("AirRollRightRPC", RpcTarget.MasterClient, true);
            AirRollRightRPC(true);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            photonView.RPC("WantToStrafeRightRPC", RpcTarget.MasterClient, false);
            WantToStrafeRightRPC(false);
            photonView.RPC("AirRollRightRPC", RpcTarget.MasterClient, false);
            AirRollRightRPC(false);
        }

        if (sourisHorizontale != 0f)
        {
            photonView.RPC("WantToTurnRPC", RpcTarget.MasterClient, sourisHorizontale);
            WantToTurnRPC(sourisHorizontale);
            photonView.RPC("BoostTurnRPC", RpcTarget.MasterClient, sourisHorizontale);
            BoostTurnRPC(sourisHorizontale);
        }

        if (sourisVerticale != 0f)
        {
            photonView.RPC("BoostPitchRPC", RpcTarget.MasterClient, sourisVerticale);
            BoostPitchRPC(sourisVerticale);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            photonView.RPC("AirBoostActivateRPC", RpcTarget.MasterClient, true);
            AirBoostActivateRPC(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            photonView.RPC("AirBoostActivateRPC", RpcTarget.MasterClient, false);
            AirBoostActivateRPC(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("WantToShootFirstRPC", RpcTarget.MasterClient, true);
            WantToShootFirstRPC(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            photonView.RPC("WantToShootFirstRPC", RpcTarget.MasterClient, false);
            WantToShootFirstRPC(false);
        }

        //choisir l'arme de base
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, 1);
            photonView.RPC("SelectedWeaponRPC", RpcTarget.MasterClient, 0);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, -1);
        }

        //choisir les armes bleues
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, 2);
            photonView.RPC("SelectedWeaponRPC", RpcTarget.MasterClient, 1);
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, -1);
        }

        //choisir les armes vertes
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, 3);
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, -1);
        }

        //choisir l'arme rouge
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, 4);
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, -1);
        }
        #endregion
    }

    #region [PunRPC]

    [PunRPC]
    void ChangerArmeRPC(int choix)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            ChangerArme = choix;
        }
    }

    void SelectedWeaponRPC(int index)
    {
        if (PhotonNetwork.IsMasterClient || prediction)
        {
            SelectedWeapon = index;
        }
    }

    [PunRPC]
    void WantToGoForwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            WantToGoForward = intent;
        }
    }

    [PunRPC]
    void WantToGoBackwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            WantToGoBackward = intent;
        }
    }

    [PunRPC]
    void WantToStrafeLeftRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            WantToStrafeLeft = intent;
        }
    }

    [PunRPC]
    void WantToStrafeRightRPC(bool intent)
    {
        if (PhotonNetwork.IsMasterClient || prediction)
        {
            WantToStrafeRight = intent;
        }
    }

    [PunRPC]
    void WantToTurnRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            WantToTurn = intent;
        }
    }

    [PunRPC]
    void AirPitchUpRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            AirPitchUp = intent;
        }
    }

    [PunRPC]
    void AirPitchDownRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            AirPitchDown = intent;
        }
    }

    [PunRPC]
    void AirRollRightRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            AirRollRight = intent;
        }
    }

    [PunRPC]
    void AirRollLeftRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            AirRollLeft = intent;
        }
    }

    [PunRPC]
    void AirBoostActivateRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            AirBoostActivate = intent;
        }
    }

    [PunRPC]
    void BoostForwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            BoostForward = intent;
        }
    }

    [PunRPC]
    void BoostBackwardRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            BoostBackward = intent;
        }
    }

    [PunRPC]
    void BoostTurnRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            BoostTurn = intent;
        }
    }

    [PunRPC]
    void BoostPitchRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            BoostPitch = intent;
        }
    }

    [PunRPC]
    void WantToShootFirstRPC(bool intent)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            WantToShootFirst = intent;
        }
    }

    #endregion
}
