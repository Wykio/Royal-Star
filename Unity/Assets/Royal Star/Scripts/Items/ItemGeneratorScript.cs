using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ItemGeneratorScript : MonoBehaviour
{
    [Header ("Références")]
    [SerializeField] private float hauteurTerrain;
    [SerializeField] private GameObject armeBleuePrefab;
    [SerializeField] private GameObject armeVertePrefab;
    [SerializeField] private GameObject armeRougePrefab;
    [SerializeField] private GameObject bonusSoinPrefab;
    [SerializeField] private GameObject bonusBouclierPrefab;
    [SerializeField] private shipMotor controlleurVaisseauxScript;
    [SerializeField] private PhotonView photonView;

    [Header("Pooling des items")]
    [SerializeField] private int nbArmesBleues;
    [SerializeField] private int nbArmesVertes;
    [SerializeField] private int nbArmesRouges;

    [Header("Pooling des bonus")]
    [SerializeField] private int nbBonusSoins;
    [SerializeField] private int nbBonusBouclier;

    private ItemExposerScript[] ArmesRougesInstanciees;
    private Queue<ItemExposerScript> armesRougesLibres;
    private List<ItemExposerScript> armesRougesPlacees;

    private ItemExposerScript[] ArmesVertesInstanciees;
    private Queue<ItemExposerScript> armesVertesLibres;
    private List<ItemExposerScript> armesVertesPlacees;

    private ItemExposerScript[] ArmesBleuesInstanciees;
    private Queue<ItemExposerScript> armesBleuesLibres;
    private List<ItemExposerScript> armesBleuesPlacees;

    private BonusExposer[] BonusSoinsInstancies;
    private Queue<BonusExposer> bonusSoinsLibres;
    private List<BonusExposer> bonusSoinsPlaces;

    private BonusExposer[] BonusBouclierInstancies;
    private Queue<BonusExposer> bonusBouclierLibres;
    private List<BonusExposer> bonusBouclierPlaces;

    private bool test = false;

    //génération du pooling des items
    public void Awake()
    {
        //Initialisation des listes et queues
        armesRougesLibres = new Queue<ItemExposerScript>(nbArmesRouges);
        armesRougesPlacees = new List<ItemExposerScript>(nbArmesRouges);

        armesVertesLibres = new Queue<ItemExposerScript>(nbArmesVertes);
        armesVertesPlacees = new List<ItemExposerScript>(nbArmesVertes);

        armesBleuesLibres = new Queue<ItemExposerScript>(nbArmesBleues);
        armesBleuesPlacees = new List<ItemExposerScript>(nbArmesBleues);

        bonusSoinsLibres = new Queue<BonusExposer>(nbBonusSoins);
        bonusSoinsPlaces = new List<BonusExposer>(nbBonusSoins);

        bonusBouclierLibres = new Queue<BonusExposer>(nbBonusBouclier);
        bonusBouclierPlaces = new List<BonusExposer>(nbBonusBouclier);

        GameObject item;

        //instancier toutes les armes possibles au cours d'une partie
        //pour les armes rouges
        ArmesRougesInstanciees = new ItemExposerScript[nbArmesRouges];

        //génération des armes rouges
        for(int i = 0; i < 8; i++)
        {
            item = (GameObject)Instantiate(armeRougePrefab);
            item.SetActive(false);
            ArmesRougesInstanciees[i] = item.GetComponent<ItemExposerScript>();
            armesRougesLibres.Enqueue(ArmesRougesInstanciees[i]); 
        }

        //pour les armes vertes,
        ArmesVertesInstanciees = new ItemExposerScript[nbArmesVertes];

        //génération des armes vertes
        for(int i = 0; i < 40; i++)
        {
            item = (GameObject)Instantiate(armeVertePrefab);
            item.SetActive(false);
            ArmesVertesInstanciees[i] = item.GetComponent<ItemExposerScript>();
            armesVertesLibres.Enqueue(ArmesVertesInstanciees[i]);
        }

        //pour les armes bleues
        ArmesBleuesInstanciees = new ItemExposerScript[nbArmesBleues];

        //génération des armes bleues
        for(int i = 0; i < 50; i++)
        {
            item = (GameObject)Instantiate(armeBleuePrefab);
            item.SetActive(false);
            ArmesBleuesInstanciees[i] = item.GetComponent<ItemExposerScript>();
            armesBleuesLibres.Enqueue(ArmesBleuesInstanciees[i]);
        }

        //pour les bonus de soins
        BonusSoinsInstancies = new BonusExposer[nbBonusSoins];

        //génération des bonus de soin
        for(int i = 0; i < nbBonusSoins; i++)
        {
            item = (GameObject)Instantiate(bonusSoinPrefab);
            item.SetActive(false);
            BonusSoinsInstancies[i] = item.GetComponent<BonusExposer>();
            bonusSoinsLibres.Enqueue(BonusSoinsInstancies[i]);
        }

        //pour les bonus de bouclier
        BonusBouclierInstancies = new BonusExposer[nbBonusBouclier];

        //génération des bonus de bouclier
        for (int i = 0; i < nbBonusBouclier; i++)
        {
            item = (GameObject)Instantiate(bonusBouclierPrefab);
            item.SetActive(false);
            BonusBouclierInstancies[i] = item.GetComponent<BonusExposer>();
            bonusBouclierLibres.Enqueue(BonusBouclierInstancies[i]);
        }
    }

    public List<ItemExposerScript> GetArmesBleuesPlacees()
    {
        return armesBleuesPlacees;
    }

    public List<ItemExposerScript> GetArmesVertesPlacees()
    {
        return armesVertesPlacees;
    }

    public List<ItemExposerScript> GetArmesRougesPlacees()
    {
        return armesRougesPlacees;
    }

    public List<BonusExposer> GetBonusSoinsPlaces()
    {
        return bonusSoinsPlaces;
    }

    public List<BonusExposer> GetBonusBouclierPlaces()
    {
        return bonusBouclierPlaces;
    }

    private void FixedUpdate()
    {
        if (controlleurVaisseauxScript.getGameStarted() && PhotonNetwork.IsMasterClient)
        {
            //vérification du pooling des armes bleues
            for (int i = 0; i < armesBleuesPlacees.Count; i++)
            {
                //si une des armes bleues placées a été ramassée, on la remet dans le pooling
                if (armesBleuesPlacees[i].getRamasse())
                {
                    //remise de l'item dans la queue des armes libres
                    armesBleuesLibres.Enqueue(armesBleuesPlacees[i]);

                    //retrait de l'item de la liste des armes placées
                    armesBleuesPlacees.Remove(armesBleuesPlacees[i]);
                }
            }

            //vérification du pooling des armes vertes
            for (int i = 0; i < armesVertesPlacees.Count; i++)
            {
                //si une des armes vertes placées a été ramassée, on la remet dans le pooling
                if (armesVertesPlacees[i].getRamasse())
                {
                    //remise de l'item dans la queue des armes libres
                    armesVertesLibres.Enqueue(armesVertesPlacees[i]);

                    //retrait de l'item de la liste des armes placées
                    armesVertesPlacees.Remove(armesVertesPlacees[i]);
                }
            }

            //vérification du pooling des armes rouges
            for (int i = 0; i < armesRougesPlacees.Count; i++)
            {
                //si une des armes rouges placées a été ramassée, on la remet dans le pooling
                if (armesRougesPlacees[i].getRamasse())
                {
                    //remise de l'item dans la queue des armes libres
                    armesRougesLibres.Enqueue(armesRougesPlacees[i]);

                    //retrait de l'item de la liste des armes placées
                    armesRougesPlacees.Remove(armesRougesPlacees[i]);
                }
            }

            //vérification du pooling des bonus de soins
            for (int i = 0; i < bonusSoinsPlaces.Count; i++)
            {
                //si un des bonus de soins a été ramassé, on le remet dans le pooling
                if (bonusSoinsPlaces[i].getRamasse())
                {
                    //remise du bonus dans la queue des armes libres
                    bonusSoinsLibres.Enqueue(bonusSoinsPlaces[i]);

                    //retrait du bonus de la liste des armes placées
                    bonusSoinsPlaces.Remove(bonusSoinsPlaces[i]);
                }
            }

            //vérification du pooling des bonus de bouclier
            for (int i = 0; i < bonusBouclierPlaces.Count; i++)
            {
                //si un des bonus de soins a été ramassé, on le remet dans le pooling
                if (bonusBouclierPlaces[i].getRamasse())
                {
                    //remise du bonus dans la queue des armes libres
                    bonusBouclierLibres.Enqueue(bonusBouclierPlaces[i]);

                    //retrait du bonus de la liste des armes placées
                    bonusBouclierPlaces.Remove(bonusBouclierPlaces[i]);
                }
            }
        }
    }

    public void GenererArmeBleue(Vector3 position)
    {
        //seul le masterClient peut donner l'ordre de générer des objets aux clients
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GenererArmeBleueRPC", RpcTarget.All, position);
        }
    }

    [PunRPC]
    private void GenererArmeBleueRPC(Vector3 position)
    {
        //faire spawn une arme bleue
        var armeBleue = armesBleuesLibres.Dequeue();

        //activation de l'item
        armeBleue.ActivationItem();
        armeBleue.SetPosition(position);
        armeBleue.SetPose(false);

        //ajout de l'item dans la liste des items placés
        armesBleuesPlacees.Add(armeBleue);
    }

    public void GenererArmeVerte(Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GenererArmeVerteRPC", RpcTarget.All, position);
        }
    }

    [PunRPC]
    private void GenererArmeVerteRPC(Vector3 position)
    {
        //faire spawn une arme verte
        var armeVerte = armesVertesLibres.Dequeue();

        //activation de l'item
        armeVerte.ActivationItem();
        armeVerte.SetPosition(position);
        armeVerte.SetPose(false);

        //ajout de l'item dans la liste des items placés
        armesVertesPlacees.Add(armeVerte);
    }

    public void GenererArmeRouge(Vector3 position)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GenererArmeRougeRPC", RpcTarget.All, position);
        }
    }

    [PunRPC]
    private void GenererArmeRougeRPC(Vector3 position)
    {
        //faire spawn une arme verte
        var armeRouge = armesRougesLibres.Dequeue();

        //activation de l'item
        armeRouge.ActivationItem();
        armeRouge.SetPosition(position);
        armeRouge.SetPose(false);

        //ajout de l'item dans la liste des items placés
        armesRougesPlacees.Add(armeRouge);
    }

    public void GenererBonusSoins(Vector3 position)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GenererBonusSoinsRPC", RpcTarget.All, position);
        }
    }

    [PunRPC]
    private void GenererBonusSoinsRPC(Vector3 position)
    {
        //faire spawn un bonus de soin
        var bonus = bonusSoinsLibres.Dequeue();

        //activation du bonus
        bonus.ActivationItem();
        bonus.SetPosition(position);
        bonus.SetPose(false);

        //ajout du bonus dans la liste des bonus de soins placés
        bonusSoinsPlaces.Add(bonus);
    }

    public void GenererBonusBouclier(Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GenererBonusBouclierRPC", RpcTarget.All, position);
        }
    }

    [PunRPC]
    private void GenererBonusBouclierRPC(Vector3 position)
    {
        //faire spawn un bonus de soin
        var bonus = bonusBouclierLibres.Dequeue();

        //activation du bonus
        bonus.ActivationItem();
        bonus.SetPosition(position);
        bonus.SetPose(false);

        //ajout du bonus dans la liste des bonus de soins placés
        bonusBouclierPlaces.Add(bonus);
    }
}
