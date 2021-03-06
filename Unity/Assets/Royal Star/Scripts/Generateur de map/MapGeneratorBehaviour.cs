﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.AI;

namespace MapGeneration
{
    public class MapGeneratorBehaviour : MonoBehaviour
    {
        [Header("Modèles de décors")]
        [SerializeField] private GameObject[] listePrefabDecors;
        [SerializeField] private GameObject portailPrefab;
        [SerializeField] private GameObject sol;

        [Header("Matériaux")]
        [SerializeField] private Material[] listeMateriauxSol = new Material[4];
        [SerializeField] private Material[] listeMateriauxBiomeNormal;
        [SerializeField] private Material[] listeMateriauxBiomeGlace;
        [SerializeField] private Material[] listeMateriauxBiomeFeu;
        [SerializeField] private Material[] listeMateriauxBiomeRadiation;

        [Header("Paramétrage")]
        [SerializeField] private int hauteurBiome;
        [SerializeField] private int tailleBiome;
        [SerializeField] private int nbBiomes;

        [Header("Références")]
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GestionMapScript gestionnaireMap;
        [SerializeField] private GestionLumièreScript gestionLumieres;
        [SerializeField] private ImpactBiomeScript gestionEffetsBiomes;
        [SerializeField] private OptionsSonScript gestionSon;
        [SerializeField] private DataCollectorScript dataCollector;

        public event Action<string> majInterface;
        public event Action mapGenereePourTous;

        private double[] listeNumDecor;
        private int typeBiome;  //0 = normal 1 = glace 2 = feu 3 = radiation
        private int nbJoueurs;
        private int nbConfirmationBiomeGenere = 0;

        //fonction pour set le nombre de joueurs dans la partie
        public void setNbJoueur(int nbJoueurs)
        {
            this.nbJoueurs = nbJoueurs;
        }

        public int getTailleBiome()
        {
            return tailleBiome;
        }

        //fonction pour établir la liste des indices des décors
        public void initialiserListeNumDecor()
        {
            listeNumDecor = new double[listePrefabDecors.Length];

            for (int i = 0; i < listePrefabDecors.Length; i++)
            {
                listeNumDecor[i] = (double)i;
            }
        }

        //fonction du masterclient pour générer les biomes
        public IEnumerator DesignBiomes()
        {
            if(listeNumDecor == null || listeNumDecor.Length ==0) initialiserListeNumDecor();

            for (int i = 0; i < nbBiomes; i++)
            {
                bool ok = false;

                MapGeneratorScript generator = new MapGeneratorScript(tailleBiome, listeNumDecor, i, nbBiomes);

                setNbJoueur(PlayerNumbering.SortedPlayers.Length);

                //mise à -1 et remplissage du décor
                generator.InitialiserTableau();
                generator.creerDecor();
                generator.PlacerPortail();
                generator.DeterminerRotationDecors();

                //envoyer le tableau du biome i aux clients
                //convertir le tableau en string avant de l'envoyer aux clients
                var tab = generator.getTabBiome();
                string data = "";

                for(int j = 0; j < tailleBiome; j++)
                {
                    for(int k = 0; k < tailleBiome; k++)
                    {
                        data += tab[j, k].ToString() + "_";
                    }
                }
                //retirer le "_" en fin de string
                data = data.Substring(0, data.Length - 1);

                //choix du type de biome au hasard et envoi aux autres clients pour la gestion des lumières
                typeBiome = UnityEngine.Random.Range(0, 4);
                EnvoyerTypeBiome(typeBiome);

                //envoi de la RPC aux clients
                GenererDecor(data, generator.getTabRotation(), generator.getTabPortail(), typeBiome);

                while(!ok)
                {
                    //attente de la confirmation de tous les joueurs avant de générer le prochain biome
                    if (nbConfirmationBiomeGenere < nbJoueurs)
                    {
                        yield return new WaitForSeconds(0.001f);
                    }
                    else ok = true;
                }

                //diminution de la taille du prochain biome et reset des confirmations
                tailleBiome -= 1;
                hauteurBiome += 5000;
                nbConfirmationBiomeGenere = 0;
            }

            mapGenereePourTous.Invoke();
        }

        //le masterclient envoie le tableau qu'il a générer pour que les clients puissent générer le décor
        public void GenererDecor(string data, string dataRotation, string dataPortail, int typeBiome)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("GenererDecorViaTableauRPC", RpcTarget.All, data, dataRotation, dataPortail, typeBiome, tailleBiome, hauteurBiome);
            }
        }

        //fonction pour envoyer le type de biome aux clients pour la gestion des lumières
        public void EnvoyerTypeBiome(int type)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("EnvoyerTypeBiomeRPC", RpcTarget.All, type);
            }
        }

        //RPC pour que le client indique au masterclient qu'il a bien généré le biome
        [PunRPC]
        private void confirmationClientRPC()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                nbConfirmationBiomeGenere++;
            }
        }

        //RPC pour générer le décor à partir du tableau reçu
        [PunRPC]
        private void GenererDecorViaTableauRPC(string data, string dataRotation, string dataPortail, int typeBiome, int tailleBiome, int hauteurBiome)
        {
            //définir le type du biome

            //remettre la data sous forme d'un tableau de float
            data.Replace(".", ",");
            var dataDecor = data.Split('_');
            double[,] tabDecor = new double[tailleBiome, tailleBiome];

            //split la dataRotation en la séparant selon le "_"
            string[] dataRotationArray = dataRotation.Split('_');

            int a = 0;
            int b = 0;

            foreach(var num in dataDecor)
            {
                if(b >= tailleBiome)
                {
                    a++;
                    b = 0;
                }

                tabDecor[a, b] = Convert.ToDouble(num);

                b++;
            }

            if(listeNumDecor == null || listeNumDecor.Length == 0) initialiserListeNumDecor();

            //générer le terrain
            GameObject terrain = (GameObject)Instantiate(sol);
            terrain.transform.localScale = new Vector3(1000 * tailleBiome, 3f, 1000 * tailleBiome);
            terrain.transform.position = new Vector3((1000 * tailleBiome)/2, hauteurBiome, (1000 * tailleBiome) / 2);
            terrain.name = "SolBiome";
            

            //application du matérial du sol en fonction du type de biome
            DecorExposerScript solExposer = terrain.GetComponent<DecorExposerScript>();
            solExposer.setRenderer(listeMateriauxSol[typeBiome]);

            //pour chaque valeur du tableau
            for (int i = 0; i < tailleBiome; i++)
            {
                for (int j = 0; j < tailleBiome; j++)
                {
                    //si la case contient un décor
                    if(tabDecor[i,j] > -1)
                    {
                        string decorSTR = tabDecor[i, j].ToString();
                        var dataNum = decorSTR.Split(',');

                        if(dataNum.Length < 2)
                        {
                            dataNum = decorSTR.Split('.');
                        }

                        //génération du décor
                        int indice = int.Parse(dataNum[0]);
                        GameObject decor = listePrefabDecors[indice];
                        decor = (GameObject)Instantiate(decor);

                        //attribution du matérial
                        DecorExposerScript decorExposer = decor.GetComponent<DecorExposerScript>();
                        
                        switch(typeBiome)
                        {
                            case 0:
                                decorExposer.setRenderer(listeMateriauxBiomeNormal[UnityEngine.Random.Range(0, listeMateriauxBiomeNormal.Length)]);
                                break;
                            case 1:
                                decorExposer.setRenderer(listeMateriauxBiomeGlace[UnityEngine.Random.Range(0, listeMateriauxBiomeGlace.Length)]);
                                break;
                            case 2:
                                decorExposer.setRenderer(listeMateriauxBiomeFeu[UnityEngine.Random.Range(0, listeMateriauxBiomeFeu.Length)]);
                                break;
                            case 3:
                                decorExposer.setRenderer(listeMateriauxBiomeRadiation[UnityEngine.Random.Range(0, listeMateriauxBiomeRadiation.Length)]);
                                break;
                        }

                        //placement du décor
                        Vector3 position = new Vector3((i * 1000 + 500), hauteurBiome+1.5f, (j * 1000 + 500));

                        //ajout du decalage en x
                        string decalXstr = dataNum[1].Substring(0, 3);
                        int decalX = int.Parse(decalXstr);
                        int decalXcopie = decalX;
                        
                        if(decalX <= 200)
                        {
                            position.x -= decalX;
                        }
                        else
                        {
                            //si decalX > 500, ca veut dire que la donnée initial est inférieure à 0, donc on la divise par 10 (.964 correspond à 96, le 4 provient du décalage en z)
                            if(decalX > 400)
                            {
                                decalX = decalX / 10;
                                position.x -= decalX;
                            }
                            else
                            {
                                position.x += decalX;
                            }
                        }
                        
                        //ajout du décalage en z
                        string decalZstr;

                        if (decalXcopie > 400)
                        {
                            decalZstr = dataNum[1].Substring(2, 3);
                        }
                        else
                        {
                            decalZstr = dataNum[1].Substring(3, 3);
                        }

                        int decalZ = int.Parse(decalZstr);

                        if (decalZ <= 200)
                        {
                            position.z -= decalZ;
                        }
                        else
                        {
                            if (decalZ > 400)
                            {
                                decalZ = decalZ / 10;
                                position.z += decalZ;
                            }
                            else
                            {
                                position.z -= decalZ;
                            }
                        }

                        decor.transform.position = position;

                        //rotation
                        float rotate = float.Parse(dataRotationArray[((i+1)*(j+1))-1]);
                        decor.transform.rotation = new Quaternion(0f, 1f, 0f, rotate);
                    }
                }
            }

            if(dataPortail != "")
            {
                //placement des portails, la donnée se présente ainsi : "positionX/positionZ/PositionDestinationX/positionDestinationZ_etc..."
                var positionPortails = dataPortail.Split('_');

                //liste pour contenir tous les portails du biomes qu'on va envoyer ensuite au script de gestion de la map
                List<GameObject> listePortail = new List<GameObject>();

                //pour chaque portail
                for (int p = 0; p < positionPortails.Length; p++)
                {
                    var extract = positionPortails[p].Split('/');
                    GameObject portail = portailPrefab;
                    portail = (GameObject)Instantiate(portail);
                    portail.transform.position = new Vector3(float.Parse(extract[0]), hauteurBiome + 18, float.Parse(extract[1]));
                    portail.SetActive(false);
                    listePortail.Add(portail);

                    GameObject portailDestination = portailPrefab;
                    portailDestination = (GameObject)Instantiate(portailDestination);
                    portailDestination.name = "Portail Destination";
                    portailDestination.transform.position = new Vector3(float.Parse(extract[2]), hauteurBiome + 5400, float.Parse(extract[3]));

                    TeleporterController transport = portail.GetComponent<TeleporterController>();
                    transport.connectedTeleport = portailDestination;
                    transport.gestionSon = gestionSon;
                    transport.dataCollector = dataCollector;
                }

                //envoi de la liste au gestionnaire de map
                gestionnaireMap.AddListePortailParBiome(listePortail);
            }

            //une fois que le biome est généré, le client envoie sa confirmation
            photonView.RPC("confirmationClientRPC", RpcTarget.MasterClient);
        }

        [PunRPC]
        private void EnvoyerTypeBiomeRPC(int type)
        {
            gestionLumieres.AjouterTypeBiome(type);
            gestionEffetsBiomes.AjouterTypeBiome(type);
        }
    }
}


