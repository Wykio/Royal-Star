using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ImpactBiomeScript : MonoBehaviour
{
    [SerializeField] private ShipExposer[] vaisseaux;

    [Header("Références")]
    [SerializeField] private shipMotor gameController;

    private int[] typesBiomes = new int[4];
    private List<ShipExposer> vaisseauxDansBiome1 = new List<ShipExposer>(20);
    private List<ShipExposer> vaisseauxDansBiome2 = new List<ShipExposer>(20);
    private List<ShipExposer> vaisseauxDansBiome3 = new List<ShipExposer>(20);
    private List<ShipExposer> vaisseauxDansBiome4 = new List<ShipExposer>(20);
    private bool routineLancee = false;

    private void Awake()
    {
        for(int i = 0; i < typesBiomes.Length; i++)
        {
            typesBiomes[i] = -1;
        }
    }

    //trier les vaisseaux selon le biome dans lequel ils sont pour leur appliquer les effets correspondants
    private void Update()
    {
        //seul le masterclient applique les effets des biomes sur les joueurs
        if (PhotonNetwork.IsMasterClient && gameController.getGameStarted())
        {
            if (!routineLancee)
            {
                StartCoroutine(AppliquerDegatsParSecondeBiomes());
                routineLancee = true;
                Debug.Log("IMPACT BIOME LANCEMENT COROUTINE");
                Debug.Log("TYPES DES BIOMES : " + typesBiomes[0] + " " + typesBiomes[1] + " " + typesBiomes[2] + " " + typesBiomes[3]);
            }

            //pour chaque vaisseau, on applique l'effet du biome correspondant à la hauteur à laquelle il est
            foreach (var vaisseau in vaisseaux)
            {
                if(vaisseau.isActiveAndEnabled)
                {
                    //le vaisseau est dans le premier biome
                    if (vaisseau.ShipTransform.position.y > -300 && vaisseau.ShipTransform.position.y < 5000)
                    {
                        if (!vaisseauxDansBiome1.Contains(vaisseau))
                        {
                            AppliquerEffetBiome(typesBiomes[0], vaisseau);
                            vaisseauxDansBiome1.Add(vaisseau);
                        }
                    }
                    else
                    {
                        //le vaisseau est dans le deuxième biome
                        if (vaisseau.ShipTransform.position.y > 4700 && vaisseau.ShipTransform.position.y < 10000)
                        {
                            if (!vaisseauxDansBiome2.Contains(vaisseau))
                            {
                                Debug.Log("CHANGEMENT DE BIOME");
                                AppliquerEffetBiome(typesBiomes[1], vaisseau);
                                vaisseauxDansBiome2.Add(vaisseau);

                                //on le retire le la liste du biome 1
                                if (vaisseauxDansBiome1.Remove(vaisseau))
                                {
                                    Debug.Log("RETRAIT LISTE BIOME 1");
                                }
                            }
                        }
                        else
                        {
                            //le vaisseau est dans le troisième biome
                            if (vaisseau.ShipTransform.position.y > 9700 && vaisseau.ShipTransform.position.y < 15000)
                            {
                                if (!vaisseauxDansBiome3.Contains(vaisseau))
                                {
                                    AppliquerEffetBiome(typesBiomes[2], vaisseau);
                                    vaisseauxDansBiome3.Add(vaisseau);

                                    //on le retire le la liste du biome 2
                                    vaisseauxDansBiome2.Remove(vaisseau);
                                }
                            }
                            else
                            {
                                if (!vaisseauxDansBiome4.Contains(vaisseau))
                                {
                                    //le vaisseau est dans le dernier biome
                                    AppliquerEffetBiome(typesBiomes[3], vaisseau);
                                    vaisseauxDansBiome4.Add(vaisseau);

                                    //on le retire le la liste du biome 3
                                    vaisseauxDansBiome3.Remove(vaisseau);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator AppliquerDegatsParSecondeBiomes()
    {
        while (gameController.getGameStarted())
        {
            
            //effet du biome 1
            if (typesBiomes[0] == 2)
            {
                for(int i = 0; i < vaisseauxDansBiome1.Count; i++)
                {
                    Debug.Log("BIOME 1 Effet de feu sur le vaisseau " + vaisseauxDansBiome1[i].playerID);
                    vaisseauxDansBiome1[i].EffetFeu(1);
                }
            }
            else
            {
                if (typesBiomes[0] == 3)
                {
                    for (int i = 0; i < vaisseauxDansBiome1.Count; i++)
                    {
                        Debug.Log("BIOME 1 Effet de radiation sur le vaisseau " + vaisseauxDansBiome1[i].playerID);
                        vaisseauxDansBiome1[i].EffetRadiation(1);
                    }
                }
            }

            //effet du biome 2
            if (typesBiomes[1] == 2)
            {
                for (int i = 0; i < vaisseauxDansBiome2.Count; i++)
                {
                    Debug.Log("BIOME 2 Effet de feu sur le vaisseau " + vaisseauxDansBiome2[i].playerID);
                    vaisseauxDansBiome2[i].EffetFeu(1);
                }
            }

            else
            {
                if (typesBiomes[1] == 3)
                {
                    for (int i = 0; i < vaisseauxDansBiome2.Count; i++)
                    {
                        Debug.Log("BIOME 2 Effet de radiation sur le vaisseau " + vaisseauxDansBiome2[i].playerID);
                        vaisseauxDansBiome2[i].EffetRadiation(1);
                    }
                }
            }

            //effet du biome 3
            if (typesBiomes[2] == 2)
            {
                for (int i = 0; i < vaisseauxDansBiome3.Count; i++)
                {
                    Debug.Log("BIOME 3 Effet de feu sur le vaisseau " + vaisseauxDansBiome3[i].playerID);
                    vaisseauxDansBiome3[i].EffetFeu(1);
                }
            }
            else
            {
                if (typesBiomes[2] == 3)
                {
                    for (int i = 0; i < vaisseauxDansBiome3.Count; i++)
                    {
                        Debug.Log("BIOME 3 Effet de radiation sur le vaisseau " + vaisseauxDansBiome3[i].playerID);
                        vaisseauxDansBiome3[i].EffetRadiation(1);
                    }
                }
            }

            //effet du biome 4
            if (typesBiomes[3] == 2)
            {
                for (int i = 0; i < vaisseauxDansBiome4.Count; i++)
                {
                    Debug.Log("BIOME 4 Effet de feu sur le vaisseau " + vaisseauxDansBiome4[i].playerID);
                    vaisseauxDansBiome4[i].EffetRadiation(1);
                }
            }
            else
            {
                if (typesBiomes[3] == 3)
                {
                    for (int i = 0; i < vaisseauxDansBiome4.Count; i++)
                    {
                        Debug.Log("BIOME 4 Effet de radiation sur le vaisseau " + vaisseauxDansBiome4[i].playerID);
                        vaisseauxDansBiome4[i].EffetRadiation(1);
                    }
                }
            }
        
            yield return new WaitForSeconds(1f);
        }
    }

    private void AppliquerEffetBiome(int type, ShipExposer vaisseau)
    {
        switch(type)
        {
            //biome normal, tous les facteurs sont à 1
            case 0:
                vaisseau.SetFacteurs(1f, 1f, 1f, 1f);
                break;

            //biome de glace
            case 1:
                vaisseau.SetFacteurs(1f, 0.7f, 0.9f, 1.2f);
                break;
            
            //biome de feu
            case 2:
                vaisseau.SetFacteurs(1.1f, 1.3f, 1.4f, 0.9f);
                break;

            //biome de radiation
            case 3:
                vaisseau.SetFacteurs(1.2f, 0.9f, 1f, 1.1f);
                break;
        }
    }

    //fonction pour ajouter un type de biome au tableau
    public void AjouterTypeBiome(int type)
    {
        for(int i = 0; i < typesBiomes.Length; i++)
        {
            if(typesBiomes[i] == -1)
            {
                typesBiomes[i] = type;
                break;
            }
        }
    }
    
}
