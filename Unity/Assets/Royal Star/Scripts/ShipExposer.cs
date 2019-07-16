using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

public class ShipExposer : MonoBehaviour
{
    [Header ("Composants du vaisseau")]
    public bool Aerien;
    public Rigidbody ShipRigidBody;
    public PhotonRigidbodyView ShipRigidbodyView;
    public PhotonView photonView;
    public Transform ShipTransform;
    public GameObject ShipRootGameObject;
    public Transform[] ShipHoverPoints;
    public Transform ShipCentreGravite;
    public Camera ShipCamera;
    public AudioSource lecteurSon;
    public HitboxExposerScript ShipHitbox;
    public bool vaisseauActif = false;
    public bool enPause = false;

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
    [SerializeField] public Text physicalStatus;
    [SerializeField] public Slider boost;
    [SerializeField] public Text compteurJoueurs;
    [SerializeField] public Text ChronoBiome;
    [SerializeField] public GameObject bouclierFX;

    [Header("Stats et facteurs")]
    [SerializeField] private int healthPoints = 200;
    [SerializeField] private int shieldPoints = 100;
    [SerializeField] private float boostPoints = 200f;
    [SerializeField] private float facteurDegatsPV = 1;
    [SerializeField] private float facteurDegatsBouclier = 1;
    [SerializeField] private float facteurUtilisationBoost = 1;
    [SerializeField] private float facteurRechargeBoost = 1;

    [Header("Collecte de données")]
    [SerializeField] public DataCollectorScript dataCollector;

    private float nextFieldOfView; 
    
    private bool boostOK;
    private float lastBoostUse = 0f;
    public bool sonBoostEnCours = false;

    // 0 pour le laser de base, 1 pour les armes bleues, 2 pour les armes vertes et 3 pour l'arme rouge
    private int armeActive = 0;

    public WeaponManagerScript[] ShipWeapons = new WeaponManagerScript[3];

    public int currentWeaponIndex = 0;

    void Start()
    {
        nextFieldOfView = ShipCamera.fieldOfView;
    }

    #region effets des biomes
    public void EffetRadiation(int degat)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (shieldPoints == 0)
            {
                healthPoints -= degat;
            }
        }
    }

    public void EffetFeu(int degat)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(shieldPoints > degat)
            {
                shieldPoints -= degat;
            }
            else
            {
                if (shieldPoints > 0) shieldPoints = 0;
                bouclierFX.SetActive(false);
            }
        }
    }

    public IEnumerator GestionChronometre(int dureeBiome, int dureeOuverturePortails)
    {
        int nbMin = dureeBiome / 60;
        int nbSec = dureeBiome % 60;

        ChronoBiome.text = nbMin + ":" + nbSec;

        for (int i = 0; i < dureeBiome; i++)
        {
            yield return new WaitForSeconds(1);

            if (nbSec == 0)
            {
                nbSec = 59;
                nbMin--;
            }
            else
                nbSec--;

            if (nbSec < 10)
                ChronoBiome.text = nbMin + ":0" + nbSec;
            else
                ChronoBiome.text = nbMin + ":" + nbSec;

            if (i < dureeBiome - dureeOuverturePortails)
                ChronoBiome.color = Color.green;
            else
                ChronoBiome.color = Color.red;

        }
    }

    public void SetFacteurs(float factPV, float factBouclier, float factUtilBoost, float factRechargeBouclier)
    {
        facteurDegatsPV = factPV;
        facteurDegatsBouclier = factBouclier;
        facteurUtilisationBoost = factUtilBoost;
        facteurRechargeBoost = factRechargeBouclier;
    }
    #endregion

    #region Gestion des stats

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
        if (!alive) return;
        //si le vaisseau a du shield, les dégats sont appliqués dessus
        if(shieldPoints > 0)
        {
            if(shieldPoints >= damage)
                shieldPoints -=  Mathf.CeilToInt(damage * facteurDegatsBouclier);
            else
            {
                healthPoints -= Mathf.CeilToInt((damage - shieldPoints) * facteurDegatsPV);
                shieldPoints = 0;
                bouclierFX.SetActive(false);
            }
        }
        else
        {
            //sinon c'est directement sur les pv
            healthPoints -= Mathf.CeilToInt(damage * facteurDegatsPV);
            if (healthPoints <= 0)
            {
                healthPoints = 0;
                alive = false;
                if (damage < 1000)
                    dataCollector.MortParTir(playerID, ShipTransform.position);
            }
        }
    }

    public int getPV()
    {
        return healthPoints;
    }

    public void Soins(int pv)
    {
        healthPoints += pv;
        if (healthPoints > 200) healthPoints = 200;
    }

    public int getBouclier()
    {
        return shieldPoints;
    }

    public void RechargeBouclier(int recharge)
    {
        shieldPoints += recharge;
        if (shieldPoints > 100) shieldPoints = 100;

        if (!bouclierFX.activeSelf) bouclierFX.SetActive(true);
    }

    public float getBoost()
    {
        return boostPoints;
    }

    public float GetLastBoostUse()
    {
        return lastBoostUse;
    }

    public void SetLastBoostUse(float time)
    {
        lastBoostUse = time;
    }

    public void UtilisationBoost(float points)
    {
        boostPoints -= Mathf.CeilToInt(points * facteurUtilisationBoost);

        if (boostPoints < 0.0f) boostPoints = 0.0f;
    }

    public void RechargeBoost(float points)
    {
        boostPoints += Mathf.CeilToInt(points * facteurRechargeBoost);

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

    [PunRPC]
    private void DesactiverBouclierFX(int idJoueur)
    {
        if(playerID == idJoueur)
        {

        }
    }

    #endregion

    #region gestion des armes
    public int getArmeActive()
    {
        return armeActive;
    }

    public void ChangeWeapon(int index)
    {
        currentWeaponIndex = index;
    }

    //fonction pour savoir si le vaisseau a des slots d'arme bleues vide
    public bool GetSlotVideArmesBleues()
    {
        //si l'arme bleue 1 n'est pas active, un slot est donc dispo
        if(!ArmeBleue1.activeSelf)
        {
            return true;
        }
        else
        {
            //sinon, si l'arme bleue 2 n'est pas active, un slot est dispo
            if (!ArmeBleue2.activeSelf)
            {
                return true;
            }
            //sinon tout les slots sont occupés
            else return false;
        }
    }

    //fonction pour savoir si le vaisseau a des slots d'arme verte vide
    public bool GetSlotVideArmesVertes()
    {
        //si l'arme verte 1 n'est pas active, un slot est donc dispo
        if (!ArmeVerte1.activeSelf)
        {
            return true;
        }
        else
        {
            //sinon, si l'arme verte 2 n'est pas active, un slot est dispo
            if (!ArmeVerte2.activeSelf)
            {
                return true;
            }
            //sinon tout les slots sont occupés
            else return false;
        }
    }

    //fonction pour savoir si le vaisseau a des slots d'arme verte vide
    public bool GetSlotVideArmeRouge()
    {
        //si l'arme rouge est active, on return false, true dans le cas contraire
        if (ArmeRouge1.activeSelf) return false;
        else return true;
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
                break;

            case 3:
                if (armeActive != 3 && (ArmeVerte1.activeSelf || ArmeVerte2.activeSelf)) armeActive = 3;
                break;

            case 4:
                if (armeActive != 4 && ArmeRouge1.activeSelf) armeActive = 4;
                break;
        }
    }
    #endregion

    #region gestion du field of view
    public float GetFieldOfView()
    {
        return nextFieldOfView;
    }

    public void SetNewFieldOfView(float newFOV, int playerID)
    {
        if(PhotonNetwork.IsMasterClient
            && Mathf.Abs(newFOV - nextFieldOfView) > float.Epsilon)
        {
            int i = 0;

            for(; i < PlayerNumbering.SortedPlayers.Length; i++)
            {
                if(PlayerNumbering.SortedPlayers[i].ActorNumber == playerID)
                {
                    photonView.RPC("SetNextFieldOfViewRPC", PlayerNumbering.SortedPlayers[i], newFOV);
                    break;
                }
            }
        }
    }

    [PunRPC]
    public void SetNextFieldOfViewRPC(float newFOV)
    {
        nextFieldOfView = newFOV;
    }

    private void AdaptToCurrentFieldOfView()
    {
        if (Mathf.Abs(ShipCamera.fieldOfView - nextFieldOfView) < float.Epsilon) return;

        if (Mathf.Abs(ShipCamera.fieldOfView - nextFieldOfView) < 0.2f)
        {
            ShipCamera.fieldOfView = nextFieldOfView;
        }
        else if (Mathf.Abs(ShipCamera.fieldOfView - nextFieldOfView) < 0.8f)
        {
            ShipCamera.fieldOfView = Mathf.Lerp(ShipCamera.fieldOfView, nextFieldOfView, 1.8f * Time.deltaTime);
        }
        else
        {
            ShipCamera.fieldOfView = Mathf.Lerp(ShipCamera.fieldOfView, nextFieldOfView, 1.3f * Time.deltaTime);
        }
    }
    #endregion

    private void Update()
    {
        AdaptToCurrentFieldOfView();
    }
}
