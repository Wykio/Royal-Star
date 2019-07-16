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

    readonly KeyCode[] weaponKeys = new KeyCode[] {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
    };
    readonly KeyCode[] actionKeys = new KeyCode[] {
        KeyCode.Z,
        KeyCode.S,
        KeyCode.Q,
        KeyCode.D,
        KeyCode.A,
        KeyCode.E,
        KeyCode.LeftShift,
        KeyCode.Mouse0
    };

    readonly string[] rpcNames = new string[] {
        "WantToGoForwardRPC",
        "WantToGoBackwardRPC",
        "WantToStrafeLeftRPC",
        "WantToStrafeRightRPC",
        "AirRollLeftRPC",
        "AirRollRightRPC",
        "AirBoostActivateRPC",
        "WantToShootFirstRPC"
    };

    void KeyboardAndMouseMappings()
    {
        //récupération des axes de souris
        float horizontalAxis = Input.GetAxis("Mouse X");
        float verticalAxis = -Input.GetAxis("Mouse Y");

        #region Inputs

        for (int i = 0; i < actionKeys.Length; i++)
        {
            if (Input.GetKeyDown(actionKeys[i]))
            {
                photonView.RPC(rpcNames[i], RpcTarget.MasterClient, true);
                GetType().GetMethod(rpcNames[i])?.Invoke(this, new object[] { true });
            }
            if (Input.GetKeyUp(actionKeys[i]))
            {
                photonView.RPC(rpcNames[i], RpcTarget.MasterClient, false);
                GetType().GetMethod(rpcNames[i])?.Invoke(this, new object[] { false });
            }
        }

        //choisir l'arme de base
        for (int i = 0; i < weaponKeys.Length; i++)
        {
            if (Input.GetKeyDown(weaponKeys[i]))
            {
                photonView.RPC("ChangerArmeRPC", RpcTarget.MasterClient, i);
            }
        }

        if (horizontalAxis != 0f)
            photonView.RPC("WantToTurnRPC", RpcTarget.MasterClient, horizontalAxis);
        if (verticalAxis != 0f)
            photonView.RPC("AirPitchRPC", RpcTarget.MasterClient, verticalAxis);
        
        #endregion
    }

    void JoystickMapping()
    {
        float strafeAxis = Input.GetAxis("Joystick HRS");
        float moveAxis = Input.GetAxis("Joystick VRS");
        float turnAxis = Input.GetAxis("Horizontal");
        float pitchAxis = -Input.GetAxis("Vertical");
        float turbo = Input.GetAxis("Joystick LT");
        float shoot = Input.GetAxis("Joystick RT");

        if (strafeAxis != 0f)
        {
            if (strafeAxis > 0f)
            {
                photonView.RPC("WantToStrafeLeftRPC", RpcTarget.MasterClient, true);
            }
            else
            {
                photonView.RPC("WantToStrafeRightRPC", RpcTarget.MasterClient, true);
            }
        }
        else
        {
            photonView.RPC("WantToStrafeLeftRPC", RpcTarget.MasterClient, false);
            photonView.RPC("WantToStrafeRightRPC", RpcTarget.MasterClient, false);
        }

        if (moveAxis != 0f)
        {
            if (moveAxis > 0f)
            {
                photonView.RPC("WantToGoForwardRPC", RpcTarget.MasterClient, true);
            }
            else
            {
                photonView.RPC("WantToGoBackwardRPC", RpcTarget.MasterClient, true);
            }
        }
        else
        {
            photonView.RPC("WantToGoForwardRPC", RpcTarget.MasterClient, false);
            photonView.RPC("WantToGoBackwardRPC", RpcTarget.MasterClient, false);
        }
        if (pitchAxis != 0f)
            photonView.RPC("AirPitchRPC", RpcTarget.MasterClient, pitchAxis);

        if (turnAxis != 0f)
        {
            photonView.RPC("WantToTurnRPC", RpcTarget.MasterClient, turnAxis);

            if (turnAxis > 0f)
            {
                photonView.RPC("AirRollLeftRPC", RpcTarget.MasterClient, true);
            }
            else
            {
                photonView.RPC("AirRollRightRPC", RpcTarget.MasterClient, true);
            }
        }
        else
        {
            photonView.RPC("AirRollLeftRPC", RpcTarget.MasterClient, false);
            photonView.RPC("AirRollRightRPC", RpcTarget.MasterClient, false);
        }
        if (shoot != 0f)
            photonView.RPC("WantToShootFirstRPC", RpcTarget.MasterClient, true);

        else
            photonView.RPC("WantToShootFirstRPC", RpcTarget.MasterClient, false);

        if (turbo != 0f)
            photonView.RPC("AirBoostActivateRPC", RpcTarget.MasterClient, true);

        else
            photonView.RPC("AirBoostActivateRPC", RpcTarget.MasterClient, false);
        
    }

    public void Update()
    {   
        //si le joueur n'est pas dans la liste des joueurs ou que son numéro ne correspond pas au numéro de son indice
        if (PlayerNumbering.SortedPlayers.Length <= IndiceJoueur ||
            PlayerNumbering.SortedPlayers[IndiceJoueur].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        KeyboardAndMouseMappings();
        //JoystickMapping();
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

    [PunRPC]
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
    void AirPitchRPC(float intent)
    {
        if(PhotonNetwork.IsMasterClient || prediction)
        {
            AirPitch = intent;
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
