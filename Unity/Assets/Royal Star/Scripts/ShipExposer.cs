using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ShipExposer : MonoBehaviour
{
    [Header ("Composants du vaisseau")]
    public bool Aerien;
    public Rigidbody ShipRigidBody;
    public PhotonRigidbodyView ShipRigidbodyView;
    public Transform ShipTransform;
    public GameObject ShipRootGameObject;
    public Transform[] ShipHoverPoints;
    public Transform ShipCentreGravite;
    public Camera ShipCamera;
    public HitboxExposerScript ShipHitbox;

    [Header ("Slots d'armes")]
    public GameObject ArmeBleue1;
    public GameObject ArmeBleue2;
    public GameObject ArmeVerte1;
    public GameObject ArmeVerte2;
    public GameObject ArmeRouge1;

    public string playerName = "Sami";
    public int playerID;
    public bool alive = true;

    [Header("Gestion de l'interface joueur")]
    [SerializeField] public Canvas interfaceJoueur;
    [SerializeField] public Text pv;
    [SerializeField] public Text bouclier;
    [SerializeField] public Slider boost;
    private int healthPoints = 200;
    private int shieldPoints = 100;
    private float boostPoints = 200f;
    private bool boostOK;

    public WeaponManagerScript[] ShipWeapons = new WeaponManagerScript[3];

    public int currentWeaponIndex = 0;

    public void MiseAJourStats(int healthPoints, int shieldPoints, float boostPoints)
    {
        //mise à jour des PV
        pv.text = healthPoints.ToString() + " PV";

        //mise à jour des PV
        bouclier.text = shieldPoints.ToString() + "% Bouclier";

        //mise à jour du boost
        boost.value = boostPoints;
    }

    public void TakeDamage(int damage)
    {
        //si le vaisseau a du shield, les dégats sont appliqués dessus
        if(shieldPoints > 0)
        {
            if(shieldPoints >= damage)
            {
                shieldPoints -= damage;
            }
            else
            {
                healthPoints -= (damage - shieldPoints);
                shieldPoints = 0;
            }
        }
        else
        {
            //sinon c'est directement sur les pv
            healthPoints -= damage;
            if (healthPoints <= 0)
            {
                healthPoints = 0;
                alive = false;
            }
        }
    }

    public int getPV()
    {
        return healthPoints;
    }

    public int getBouclier()
    {
        return shieldPoints;
    }

    public float getBoost()
    {
        return boostPoints;
    }

    public void UtilisationBoost(float points)
    {
        boostPoints -= points;

        if (boostPoints < 0.0f) boostPoints = 0.0f;
    }

    public void RechargeBoost(float points)
    {
        boostPoints += points;

        if (boostPoints > 200f) boostPoints = 200f;
    }

    public bool getBoostState()
    {
        return boostOK;
    }

    public void setBoostState(bool b)
    {
        boostOK = b;
    }
}
