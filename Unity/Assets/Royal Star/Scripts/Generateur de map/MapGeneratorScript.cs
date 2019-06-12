using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MapGeneration
{
    //classe pour générer les tableaux des biomes
    public class MapGeneratorScript
    {
        // le tableau correspond au terrain, chaque case correspond à un carré de 1000 sur 1000
        private double[,] tableauBiome;
        private int tailleBiome;
        private int hauteurBiome;
        private double[] listeElementsDecor;

        //constructeur
        public MapGeneratorScript(int tailleBiome, int hauteurBiome, double[] listeDecor)
        {
            this.tailleBiome = tailleBiome;
            this.hauteurBiome = hauteurBiome;
            listeElementsDecor = listeDecor;

            tableauBiome = new double[tailleBiome, tailleBiome];
        }

        //fonction pour initialiser le tableau
        public void InitialiserTableau()
        {
            //toutes les cases sont mises à -1
            for(int i = 0; i < tailleBiome; i++)
            {
                for(int j = 0; j < tailleBiome; j++)
                {
                    tableauBiome[i, j] = -1d;
                }
            }
        }

        //rempli le tableau pour générer un décor
        public void creerDecor()
        {
            System.Random generate = new System.Random();

            //pour chaque case, on détermine si on y place un élément de décor
            for (int i = 0; i < tailleBiome; i++)
            {
                for (int j = 0; j < tailleBiome; j++)
                {
                    //on génère un random entre 0 et 4 exclu
                    float decor = generate.Next(0, 4);

                    //on place un décor avec une probabilité de 75%
                    if(decor != 0)
                    {
                        //selection d'un élément de décor dans la liste
                        int choix = generate.Next(0, listeElementsDecor.Length);

                        tableauBiome[i, j] = listeElementsDecor[choix];

                        //ajout d'une partie décimale pour décaler les éléments,  chiffres, 3 pour un décalage en x, 3 pour un décalage en z
                        string decimale = "0,";

                        //décalage en x
                        int decal = generate.Next(0, 500);
                        decimale += decal.ToString();

                        //décalage en z
                        decal = generate.Next(0, 500);
                        decimale += decal.ToString();

                        //conversion de la partie decimale en float
                        tableauBiome[i, j] += float.Parse(decimale);
                    }
                }
            }
        }

        public double[,] getTabBiome()
        {
            return tableauBiome;
        }
    }

    //monobehaviour utilisant la classe MapGenerator pour envoyer les tableaux aux clients
    public class MapGeneratorBehaviour : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject[] listePrefabDecors;
        [SerializeField] private int hauteurBiome;
        [SerializeField] private int tailleBiome;
        [SerializeField] private int nbBiomes;

        private double[] listeNumDecor;
        private int nbJoueurs;
        private int nbConfirmationBiomeGenere = 0;

        //fonction pour set le nombre de joueurs dans la partie
        public void setNbJoueur(int nbJoueurs)
        {
            this.nbJoueurs = nbJoueurs;
        }

        //fonction pour établir la liste des indices des décors
        public void initialiserListeNumDecor()
        {
            listeNumDecor = new double[listePrefabDecors.Length];

            for(int i = 0; i < listePrefabDecors.Length; i++)
            {
                listeNumDecor[i] = (double)i;
            }
        }

        //fonction pour générer les biomes
        public void DesignBiomes()
        {
            MapGeneratorScript generator = new MapGeneratorScript(tailleBiome, hauteurBiome, listeNumDecor);

            for(int i = 0; i < nbBiomes; i++)
            {
                //mise à zéro et remplissage du décor
                generator.InitialiserTableau();
                generator.creerDecor();

                //envoyer le tableau du biome i aux clients
                GenererDecor(generator.getTabBiome());

                do
                {
                    Debug.Log("joueurs ayant chargé le" + (i+1) + "biome : " + nbConfirmationBiomeGenere + " sur " + nbJoueurs);
                }
                while (nbConfirmationBiomeGenere != nbJoueurs);

                //diminution de la taille du prochain biome et reset des confirmations
                tailleBiome -= 1;
                nbConfirmationBiomeGenere = 0;
            }
        }

        //le masterclient envoie le tableau qu'il a générer pour que les clients puissent générer le décor
        public void GenererDecor(double[,] tabDecor)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("GenererDecorViaTableauRPC", RpcTarget.All, (double[,])tabDecor);
            }
        }

        //RPC pour que le client indique qu'il a bien généré le biome
        [PunRPC]
        private void confirmationClientRPC()
        {
            nbConfirmationBiomeGenere++;
        }

        //RPC pour générer le décor à partir du tableau reçu
        [PunRPC]
        private void GenererDecorViaTableauRPC(double[,] tabDecor)
        {
            //pour chaque valeur du tableau
            for(int i = 0; i < tailleBiome; i++)
            {
                for(int j = 0; j < tailleBiome; i++)
                {

                }
            }

            //une fois que le biome est généré, le client envoie sa confirmation
            photonView.RPC("confirmationClientRPC", RpcTarget.MasterClient);
        }
    }
}

