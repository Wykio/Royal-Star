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
        private double[] listeElementsDecor;

        //constructeur
        public MapGeneratorScript(int tailleBiome, double[] listeDecor)
        {
            this.tailleBiome = tailleBiome;
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
}

