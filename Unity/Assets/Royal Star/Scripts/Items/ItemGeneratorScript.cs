﻿using System.Collections;
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
    [SerializeField] private shipMotor controlleurVaisseauxScript;
    [SerializeField] private PhotonView photonView;

    [Header("Pooling des items")]
    private ItemExposerScript[] ArmesRougesInstanciees;
    private Queue<ItemExposerScript> armesRougesLibres = new Queue<ItemExposerScript>(8);
    private List<ItemExposerScript> armesRougesPlacees = new List<ItemExposerScript>(8);

    private ItemExposerScript[] ArmesVertesInstanciees;
    private Queue<ItemExposerScript> armesVertesLibres = new Queue<ItemExposerScript>(40);
    private List<ItemExposerScript> armesVertesPlacees = new List<ItemExposerScript>(40);

    private ItemExposerScript[] ArmesBleuesInstanciees;
    private Queue<ItemExposerScript> armesBleuesLibres = new Queue<ItemExposerScript>(50);
    private List<ItemExposerScript> armesBleuesPlacees = new List<ItemExposerScript>(50);

    private bool test = false;

    //génération du pooling des items
    public void Awake()
    {
        GameObject item;

        //instancier toutes les armes possibles au cours d'une partie
        //pour les armes rouges, il n'y en a qu'une par biome donc 5 armes rouges possibles
        ArmesRougesInstanciees = new ItemExposerScript[8];

        //génération des 5 armes rouges
        for(int i = 0; i < 8; i++)
        {
            item = (GameObject)Instantiate(armeRougePrefab);
            ArmesRougesInstanciees[i] = item.GetComponent<ItemExposerScript>();
            armesRougesLibres.Enqueue(ArmesRougesInstanciees[i]);
            item.SetActive(false);
        }

        //pour les armes vertes, il y en a 25 en tout
        ArmesVertesInstanciees = new ItemExposerScript[40];

        //génération des 25 armes vertes
        for(int i = 0; i < 40; i++)
        {
            item = (GameObject)Instantiate(armeVertePrefab);
            ArmesVertesInstanciees[i] = item.GetComponent<ItemExposerScript>();
            armesVertesLibres.Enqueue(ArmesVertesInstanciees[i]);
            item.SetActive(false);
        }

        //pour les armes bleues, il y en a 30 en tout
        ArmesBleuesInstanciees = new ItemExposerScript[50];

        //génération des 30 armes bleues
        for(int i = 0; i < 50; i++)
        {
            item = (GameObject)Instantiate(armeBleuePrefab);
            ArmesBleuesInstanciees[i] = item.GetComponent<ItemExposerScript>();
            armesBleuesLibres.Enqueue(ArmesBleuesInstanciees[i]);
            item.SetActive(false);
        }
    }

    public void Update()
    {
        if(controlleurVaisseauxScript.getGameStarted() && PhotonNetwork.IsMasterClient)
        {
            if (!test)
            {
                GenererArmeBleue(new Vector3(10, 20, 10));
                GenererArmeBleue(new Vector3(4, 3, 0));

                GenererArmeVerte(new Vector3(6, 3, 2));
                GenererArmeVerte(new Vector3(2, 3, 6));

                GenererArmeRouge(new Vector3(0, 3, 3));

                test = true;
            }
        }
        
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
}
