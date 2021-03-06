﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class IngameInterfaceManagerScript : MonoBehaviour
{
    //script tournant sur tous les clients pour qu'ils puissent mettre à jour les interfaces avec les stats gérées par le MasterClient

    [SerializeField] private ShipExposer[] vaisseaux;
    [SerializeField] private PhotonView photonView;

    private bool ingame = false;

    // Update is called once per frame
    void Update()
    {
        if (!ingame) return;

        if(PhotonNetwork.IsMasterClient)
        {
            int indice = 0;
            int nbJoueursVivants = 0;

            //déterminer le nombre de vaisseaux encore en vie
            foreach(var ship in vaisseaux)
            {
                if (ship.gameObject.activeSelf) nbJoueursVivants++;
            }

            //pour chaque vaisseau, on envoie les stats que le masterclient dispose aux clients
            foreach (var ship in vaisseaux)
            {
                int i = 0;
                //on recherche le joueur rattaché au vaisseau
                for( ; i < PlayerNumbering.SortedPlayers.Length; i++)
                {
                    if(ship.playerID == PlayerNumbering.SortedPlayers[i].ActorNumber)
                    {
                        //envoi de la RPC pour le joueur avec les stats à mettre à jour
                        photonView.RPC("UpdateInterfaceRPC", PlayerNumbering.SortedPlayers[i], indice, ship.getPV(), ship.getBouclier(), ship.getBoost(), ship.getArmeActive(), nbJoueursVivants);
                        break;
                    }
                }

                indice++;
                if (indice >= PlayerNumbering.SortedPlayers.Length) break;
            }
        }
    }

    public void ActivationUpdateInterfaceToggle(bool b)
    {
        ingame = b;
    }

    [PunRPC]
    private void UpdateInterfaceRPC(int indice, int pv, int bouclier, float boost, int armeActive, int nbJoueurs)
    {
        vaisseaux[indice].MiseAJourStats(pv, bouclier, boost, nbJoueurs);

        if(!vaisseaux[indice].interfaceJoueur.isActiveAndEnabled)
        {
            vaisseaux[indice].interfaceJoueur.gameObject.SetActive(true);
        }
    }
}
