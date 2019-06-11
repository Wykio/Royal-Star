using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    //classe pour générer les tableaux des biomes
    public class MapGeneratorScript
    {
        // le tableau correspond au terrain, chaque case correspond à un carré de 100 sur 100
        private float[,] tableauBiome;
        private int tailleBiome;
        private int hauteurBiome;
        private List<float> listeElementsDecor;

        //constructeur
        public MapGeneratorScript(int tailleBiome, int hauteurBiome, List<float> listeDecor)
        {
            this.tailleBiome = tailleBiome;
            this.hauteurBiome = hauteurBiome;
            listeElementsDecor = new List<float>(listeDecor);

            tableauBiome = new float[tailleBiome, tailleBiome];
        }

        //fonction pour initialiser le tableau
        public void InitialiserTableau()
        {
            //toutes les cases sont mises à 0
            for(int i = 0; i < tailleBiome; i++)
            {
                for(int j = 0; j < tailleBiome; j++)
                {
                    tableauBiome[i, j] = 0;
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
                    //on génère un random entre 0 et 1
                    int decor = generate.Next(0, 1);

                    //si celui-ci est égal à 1 on y met un élément de décor
                    if(decor != 0)
                    {
                        int choix = generate.Next(0, listeElementsDecor.Count);
                        tableauBiome[i, j] = listeElementsDecor[choix];

                        //ajout d'une partie décimale pour décaler les éléments
                    }
                }
            }
        }
    }
}

