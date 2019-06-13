using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

namespace MapGeneration
{
    public class MapGeneratorBehaviour : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject[] listePrefabDecors;
        [SerializeField] private int hauteurBiome;
        [SerializeField] private int tailleBiome;
        [SerializeField] private int nbBiomes;

        public event Action<string> majInterface;

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

            for (int i = 0; i < listePrefabDecors.Length; i++)
            {
                listeNumDecor[i] = (double)i;
            }
        }

        //fonction pour générer les biomes
        public void DesignBiomes()
        {
            initialiserListeNumDecor();

            for (int i = 0; i < nbBiomes; i++)
            {
                MapGeneratorScript generator = new MapGeneratorScript(tailleBiome, listeNumDecor);

                setNbJoueur(PhotonNetwork.PlayerList.Length);

                //mise à -1 et remplissage du décor
                generator.InitialiserTableau();
                generator.creerDecor();

                //envoyer le tableau du biome i aux clients
                //convertir le tableau en string avant de l'envoyer aux clients
                GenererDecor(generator.getTabBiome());

                do
                {
                    Debug.Log("joueurs ayant chargé le" + (i + 1) + "biome : " + nbConfirmationBiomeGenere + " sur " + nbJoueurs);
                    //majInterface.Invoke("joueurs ayant chargé le" + (i + 1) + "biome : " + nbConfirmationBiomeGenere + " sur " + nbJoueurs);
                    
                }
                while (nbConfirmationBiomeGenere != nbJoueurs);

                //diminution de la taille du prochain biome et reset des confirmations
                tailleBiome -= 1;
                hauteurBiome += 220;
                nbConfirmationBiomeGenere = 0;
            }
        }

        //le masterclient envoie le tableau qu'il a générer pour que les clients puissent générer le décor
        public void GenererDecor(string data)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("GenererDecorViaTableauRPC", RpcTarget.All, data, tailleBiome, hauteurBiome);
            }
        }

        //RPC pour que le client indique au masterclient qu'il a bien généré le biome
        [PunRPC]
        private void confirmationClientRPC()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                nbConfirmationBiomeGenere++;
            }
        }

        //RPC pour générer le décor à partir du tableau reçu
        [PunRPC]
        private void GenererDecorViaTableauRPC(string data, int tailleBiome, int hauteurBiome)
        {
            //remettre la data sous forme d'un tableau de float

            initialiserListeNumDecor();

            //générer le terrain
            GameObject terrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
            terrain.transform.localScale = new Vector3(1000 * tailleBiome, 1f, 1000 * tailleBiome);
            terrain.transform.position = new Vector3((1000 * tailleBiome)/2, hauteurBiome, (1000 * tailleBiome) / 2);
            terrain.name = "SolBiome";

            //pour chaque valeur du tableau
            for (int i = 0; i < tailleBiome; i++)
            {
                for (int j = 0; j < tailleBiome; i++)
                {
                    //si la case contient un décor
                    if(tabDecor[i,j] > -1)
                    {
                        string decorSTR = tabDecor[i, j].ToString();
                        var data = decorSTR.Split(',');

                        if(data.Length < 2)
                        {
                            data = decorSTR.Split('.');
                        }

                        //génération du décor
                        int indice = int.Parse(data[0]);
                        GameObject decor = listePrefabDecors[indice];
                        decor = (GameObject)Instantiate(decor);

                        //placement du décor
                        Vector3 position = new Vector3((i * 1000 + 500), hauteurBiome+2, (j * 1000 + 500));

                        //ajout du decalage en x
                        string decalXstr = data[1].Substring(0, 3);
                        int decalX = int.Parse(decalXstr);

                        if(decalX <= 250)
                        {
                            position.x -= decalX;
                        }
                        else
                        {
                            position.x += decalX;
                        }

                        //ajout du décalage en z
                        string decalZstr = data[1].Substring(3, 3);
                        int decalZ = int.Parse(decalZstr);

                        if (decalZ <= 250)
                        {
                            position.x -= decalZ;
                        }
                        else
                        {
                            position.x += decalZ;
                        }

                        decor.transform.position = position;

                        //rotation
                        float rotate = UnityEngine.Random.Range(0f, 100f);
                        decor.transform.rotation = new Quaternion(0f, 1f, 0f, rotate);
                    }
                }
            }

            //une fois que le biome est généré, le client envoie sa confirmation
            photonView.RPC("confirmationClientRPC", RpcTarget.MasterClient);
        }
    }
}


