using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ShipExposer : MonoBehaviour
{

    public bool Aerien;
    public Rigidbody ShipRigidBody;
    public PhotonRigidbodyView ShipRigidbodyView;
    public Transform ShipTransform;
    public GameObject ShipRootGameObject;
    public Transform[] ShipHoverPoints;
    public Transform ShipCentreGravite;
    public Camera ShipCamera;
    public HitboxExposerScript ShipHitbox;
    public string playerName = "Sami";
    public int playerID;
    public bool alive = true;

    [Header("Gestion de l'interface joueur")]
    [SerializeField] public Canvas interfaceJoueur;
    [SerializeField] public Text pv;
    [SerializeField] public Text bouclier;
    private int healthPoints = 200;
    private int shieldPoints = 100;

    public WeaponManagerScript[] ShipWeapons = new WeaponManagerScript[3];

    public int currentWeaponIndex = 0;

    public void MiseAJourStats(int healthPoints, int shieldPoints)
    {
        //mise à jour des PV
        pv.text = healthPoints.ToString() + " PV";

        //mise à jour des PV
        bouclier.text = shieldPoints.ToString() + "% Bouclier";
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
}
