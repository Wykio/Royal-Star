using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIntentReceiver : MonoBehaviour
{
    #region Commandes au sol
    public bool WantToGoForward { get; set; }
    public bool WantToGoBackward { get; set; }
    public bool WantToStrafeRight { get; set; }
    public bool WantToStrafeLeft { get; set; }
    public float WantToTurn { get; set; }

    #endregion

    #region Commandes en vol
    public bool AirPitchUp { get; set; }
    public bool AirPitchDown { get; set; }
    public bool AirRollRight { get; set; }
    public bool AirRollLeft { get; set; }
    #endregion

    #region commandes Boost activé
    public bool AirBoostActivate { get; set; }
    public bool BoostForward { get; set; }
    public bool BoostBackward { get; set; }
    public float BoostPitch { get; set; }
    public float BoostTurn { get; set; }

    #endregion

    #region commandes liées au tir

    public bool WantToShootFirst { get; set; }
    public int ChangerArme { get; set; }
    public int SelectedWeapon { get; set; }


    #endregion

}
