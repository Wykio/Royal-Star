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

    private void Awake()
    {
        for(int i = 0; i < typesBiomes.Length; i++)
        {
            typesBiomes[i] = -1;
        }
    }

    private void Update()
    {
        //seul le masterclient applique les effets des biomes sur les joueurs
        if (PhotonNetwork.IsMasterClient && gameController.getGameStarted())
        {
            //pour chaque vaisseau, on applique l'effet du biome correspondant à la hauteur à laquelle il est
            foreach (var vaisseau in vaisseaux)
            {
                //le vaisseau est dans le premier biome
                if(vaisseau.ShipTransform.position.y >-300 && vaisseau.ShipTransform.position.y < 5000)
                {
                    AppliquerEffetBiome(typesBiomes[0], vaisseau);
                }
                else
                {
                    //le vaisseau est dans le deuxième biome
                    if(vaisseau.ShipTransform.position.y > 4700 && vaisseau.ShipTransform.position.y < 10000)
                    {
                        AppliquerEffetBiome(typesBiomes[1], vaisseau);
                    }
                    else
                    {
                        //le vaisseau est dans le troisième biome
                        if(vaisseau.ShipTransform.position.y > 9700 && vaisseau.ShipTransform.position.y < 15000)
                        {
                            AppliquerEffetBiome(typesBiomes[2], vaisseau);
                        }
                        else
                        {
                            //le vaisseau est dans le dernier biome
                            AppliquerEffetBiome(typesBiomes[3], vaisseau);
                        }
                    }
                }
            }
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

                //dégradation du bouclier
                vaisseau.EffetFeu(1);
                break;

            //biome de radiation
            case 3:
                vaisseau.SetFacteurs(1.2f, 0.9f, 1f, 1.1f);

                //dégradation des PV si le vaisseau n'a pas de bouclier
                vaisseau.EffetRadiation(1);
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
            }
        }
    }
    
}
