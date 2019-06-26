using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GestionLumièreScript : MonoBehaviour
{
    [SerializeField] Light[] lumieresDesBiomes;
    [SerializeField] ShipExposer[] joueurs;
    private Queue<int> typesBiomesEnJeu = new Queue<int>(4);
    private int hauteurCourante = 0;
    private ShipExposer joueurLocal;
    bool premiereLumiereActive = false;

    private void OnEnable()
    {
        foreach(var vaisseau in joueurs)
        {
            if(vaisseau.playerID == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                joueurLocal = vaisseau;
                break;
            }
        }
    }

    private void Update()
    {
        if(joueurLocal != null)
        {
            if (hauteurCourante == 0 && !premiereLumiereActive)
            {
                ActiverLumiere();
                premiereLumiereActive = true;
            }
            else
            {
                if (joueurLocal.ShipTransform.position.y > hauteurCourante + 5000)
                {
                    hauteurCourante += 5000;
                    ActiverLumiere();
                }
            }   
        }
    }

    //ajoute un type de biome à la queue des biomes qui seront actifs au court de la partie
    public void AjouterTypeBiome(int i)
    {
        typesBiomesEnJeu.Enqueue(i);
    }

    //activer la lumière indiquée par la
    public void ActiverLumiere()
    {
        foreach(var lumiere in lumieresDesBiomes)
        {
            lumiere.gameObject.SetActive(false);
        }

        lumieresDesBiomes[typesBiomesEnJeu.Dequeue()].gameObject.SetActive(true);
    }
}
