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
    public Collider ArmeBleue1Collider;
    public GameObject ArmeBleue2;
    public Collider ArmeBleue2Collider;
    public GameObject ArmeVerte1;
    public Collider ArmeVerte1Collider;
    public GameObject ArmeVerte2;
    public Collider ArmeVerte2Collider;
    public GameObject ArmeRouge1;
    public Collider ArmeRouge1Collider;

    public string playerName = "Sami";
    public int playerID;
    public bool alive = true;

    [Header("Gestion de l'interface joueur")]
    [SerializeField] public Canvas interfaceJoueur;
    [SerializeField] public Text pv;
    [SerializeField] public Text bouclier;
    [SerializeField] public Slider boost;
    [SerializeField] public Text compteurJoueurs;
    private int healthPoints = 200;
    private int shieldPoints = 100;
    private float boostPoints = 200f;
    private bool boostOK;

    // 0 pour le laser de base, 1 pour les armes bleues, 2 pour les armes vertes et 3 pour l'arme rouge
    private int armeActive = 0;

    public WeaponManagerScript[] ShipWeapons = new WeaponManagerScript[3];

    public int currentWeaponIndex = 0;

    public void MiseAJourStats(int healthPoints, int shieldPoints, float boostPoints, int nbJoueursVivants)
    {
        //mise à jour des PV
        pv.text = healthPoints.ToString() + " PV";

        //mise à jour des PV
        bouclier.text = shieldPoints.ToString() + "% Bouclier";

        //mise à jour du boost
        boost.value = boostPoints;

        //mise à jour du compteur de joueurs
        compteurJoueurs.text = "Pilotes restants : " + nbJoueursVivants;
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

    public int getArmeActive()
    {
        return armeActive;
    }

    //fonction d'activation quand le vaisseau ramasse une arme rouge
    public void ActiverArmeRouge()
    {
        ArmeRouge1.SetActive(true);
        ArmeRouge1Collider.isTrigger = false;
    }

    //fonction d'activation quand le vaisseau ramasse une arme verte
    public void ActiverArmeVerte()
    {
        //si l'arme verte 1 est déjà activée, on vérifie la deuxième arme verte
        if(ArmeVerte1.activeSelf)
        {
            //si l'arme Verte 2 n'est pas activée, on l'active
            if(!ArmeVerte2.activeSelf)
            {
                ArmeVerte2.SetActive(true);
                ArmeVerte2Collider.isTrigger = false;
            }
        }
        else
        {
            //sinon on active l'arme verte 1
            ArmeVerte1.SetActive(true);
            ArmeVerte1Collider.isTrigger = false;
        }
    }

    //fonction d'activation quand le vaisseau ramasse une arme bleue
    public void ActiverArmeBleue()
    {
        //si l'arme bleue 1 est déjà activée, on vérifie la deuxième arme bleue
        if (ArmeBleue1.activeSelf)
        {
            //si l'arme bleue 2 n'est pas activée, on l'active
            if (!ArmeBleue2.activeSelf)
            {
                ArmeBleue2.SetActive(true);
                ArmeBleue2Collider.isTrigger = false;
            }
        }
        else
        {
            //sinon on active l'arme bleue 1
            ArmeBleue1.SetActive(true);
            ArmeBleue1Collider.isTrigger = false;
        }
    }

    //fonction pour changer d'arme
    public void ChangerArme(int choix)
    {
        switch(choix)
        {
            case 1:
                if (armeActive != 1) armeActive = 1;
                break;

            case 2:
                if (armeActive != 2 && (ArmeBleue1.activeSelf || ArmeBleue2.activeSelf)) armeActive = 2;
                Debug.Log("SHIP EXPOSER arme active = " + armeActive);
                break;

            case 3:
                if (armeActive != 3 && (ArmeVerte1.activeSelf || ArmeVerte2.activeSelf)) armeActive = 3;
                break;

            case 4:
                if (armeActive != 4 && ArmeRouge1.activeSelf) armeActive = 4;
                break;
        }
    }
}
