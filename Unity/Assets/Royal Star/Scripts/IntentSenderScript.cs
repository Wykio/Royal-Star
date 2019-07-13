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

    KeyCode[] weaponKeys = new KeyCode[] {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
    };

    KeyCode[] actionKeys = new KeyCode[] {
        KeyCode.Z,
        KeyCode.S,
        KeyCode.Q,
        KeyCode.D,
        KeyCode.A,
        KeyCode.E,
        KeyCode.LeftShift,
        KeyCode.Mouse0
    };

    string[] rpcNames = new string[] {
        "WantToGoForwardRPC",
        "WantToGoBackwardRPC",
        "WantToStrafeLeftRPC",
        "WantToStrafeRightRPC",
        "AirRollLeftRPC",
        "AirRollRightRPC",
        "AirBoostActivateRPC",
        "WantToShootFirstRPC"
    };

    public void Update()
    {
        
        //si le joueur n'est pas dans la liste des joueurs ou que son numéro ne correspond pas au numéro de son indice
        if (PlayerNumbering.SortedPlayers.Length <= IndiceJoueur ||
                PlayerNumbering.SortedPlayers[IndiceJoueur].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        #region Inputs
        for (int i = 0; i < actionKeys.Length; i++)
        {
            if (Input.GetKeyDown(actionKeys[i]))
                photonView.RPC(rpcNames[i], RpcTarget.MasterClient, true);
            if (Input.GetKeyUp(actionKeys[i]))
                photonView.RPC(rpcNames[i], RpcTarget.MasterClient, false);
        }

        //récupération des axes de souris
        var sourisVerticale = -Input.GetAxis("Mouse Y");
        var sourisHorizontale = Input.GetAxis("Mouse X");

        if (sourisHorizontale != 0f)
            photonView.RPC("WantToTurnRPC", RpcTarget.MasterClient, sourisHorizontale);

        if (sourisVerticale != 0f)
            photonView.RPC("BoostPitchRPC", RpcTarget.MasterClient, sourisVerticale);

        //choisir l'arme de base
        for (int i = 0; i < weaponKeys.Length; i++)
        {
            if (Input.GetKeyDown(weaponKeys[i]))
                photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, weaponKeys[i] - KeyCode.Alpha0);
        }
        #endregion
    }

    #region [PunRPC]

    [PunRPC]
    void ChangerArmeRPC(int choix)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            ChangerArme = choix;
        }
    }

    void SelectedWeaponRPC(int index)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SelectedWeapon = index;
        }
    }

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
    void BoostPitchRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient)
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
