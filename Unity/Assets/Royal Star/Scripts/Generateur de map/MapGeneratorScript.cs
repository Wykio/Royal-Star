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
        private string tabPortail;
        private string tabRotation;
        private int tailleBiome;
        private double[] listeElementsDecor;
        private int numBiome;
        private int nbBiome;

        //constructeur
        public MapGeneratorScript(int tailleBiome, double[] listeDecor, int numBiome, int nbBiome)
        {
            this.tailleBiome = tailleBiome;
            this.numBiome = numBiome;
            this.nbBiome = nbBiome;
            listeElementsDecor = listeDecor;

            tableauBiome = new double[tailleBiome, tailleBiome];
            tabPortail = "";
            tabRotation = "";
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
            string data = "";

            System.Random generate = new System.Random();

            //pour chaque case, on détermine si on y place un élément de décor
            for (int i = 0; i < tailleBiome; i++)
            {
                for (int j = 0; j < tailleBiome; j++)
                {
                    //on génère un random entre 0 et 4 exclu
                    float decor = generate.Next(0, 5);

                    //on place un décor avec une probabilité de 80%
                    if(decor != 0)
                    {
                        //selection d'un élément de décor dans la liste
                        int choix = generate.Next(0, listeElementsDecor.Length);

                        tableauBiome[i, j] = listeElementsDecor[choix];

                        //ajout d'une partie décimale pour décaler les éléments,  chiffres, 3 pour un décalage en x, 3 pour un décalage en z
                        string decimale = "0,";

                        //décalage en x
                        int decal = generate.Next(0, 400);
                        decimale += decal.ToString();

                        //décalage en z
                        decal = generate.Next(0, 400);
                        decimale += decal.ToString();

                        //conversion de la partie decimale en float
                        double d = float.Parse(decimale);
                        tableauBiome[i, j] += d;
                    }
                    data += tableauBiome[i, j] + "_";
                }
            }
        }

        //Générer le tableau indiquant les positions des portails
        public void PlacerPortail()
        {
            //si la condition est validée, cela signifie que l'on est au dernier biome donc pas besoin de générer des portails
            if (((nbBiome - numBiome) - 1) == 0) return;

            //déterminer le nombre de portails à placer
            for (int i = 0; i < (nbBiome-numBiome)-1; i++)
            {
                //position x du portail
                int positionX = Random.Range(1000, (tailleBiome-1) * 1000) + Random.Range(-400, 400);
                tabPortail += positionX + "/";

                //Position z du portail
                int positionZ = Random.Range(1000, (tailleBiome-1) * 1000) + Random.Range(-400, 400);
                tabPortail += positionZ + "/";

                if(tailleBiome <= 2)
                {
                    //position x du portail de destination
                    int positionXDest = Random.Range(1000, (tailleBiome - 1) * 1000) + Random.Range(-400, 0);
                    tabPortail += positionXDest + "/";

                    //position z du portail de destination
                    int positionZDest = Random.Range(1000, (tailleBiome - 1) * 1000) + Random.Range(-400, 0);
                    tabPortail += positionZDest + "_";
                }
                else
                {
                    //position x du portail de destination
                    int positionXDest = Random.Range(1000, (tailleBiome - 1) * 1000) + Random.Range(-400, +400);
                    tabPortail += positionXDest + "/";

                    //position z du portail de destination
                    int positionZDest = Random.Range(1000, (tailleBiome - 1) * 1000) + Random.Range(-400, +400);
                    tabPortail += positionZDest + "_";
                }
                
            }

            //retirer le "_" en fin de string
            tabPortail = tabPortail.Substring(0, tabPortail.Length - 1);

            if (numBiome == 0) Debug.Log("DATA PORTAIL : " + tabPortail);
        }

        //Générer le tableau des rotations des objets du décor
        public void DeterminerRotationDecors()
        {
            //pour chaque élément de décor, on choisit une rotation random
            for (int i = 0; i < tableauBiome.Length; i++)
            {
                var value = Random.Range(-3.14f, 3.14f);
                tabRotation += value.ToString() + "_";
            }

            //retirer le "_" en fin de string
            tabRotation = tabRotation.Substring(0, tabRotation.Length - 1);
        }

        public double[,] getTabBiome()
        {
            return tableauBiome;
        }

        public string getTabPortail()
        {
            return tabPortail;
        }

        public string getTabRotation()
        {
            return tabRotation;
        }
    }
}

