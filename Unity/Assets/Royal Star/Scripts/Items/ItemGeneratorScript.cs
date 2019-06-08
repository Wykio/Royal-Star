using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ItemGeneratorScript : MonoBehaviour
{
    [Header ("Références")]
    [SerializeField] private float hauteurTerrain;
    [SerializeField] private GameObject armeBleuePrefab;
    [SerializeField] private GameObject armeVertePrefab;
    [SerializeField] private GameObject armeRougePrefab;
    [SerializeField] private shipMotor controlleurVaisseauxScript;

    [Header("Pooling des items")]
    private GameObject[] ArmesRougesInstanciees;
    private GameObject[] ArmesVertesInstanciees;
    private GameObject[] ArmesBleuesInstanciees;

    public void Awake()
    {
        //instancier toutes les armes possibles au cours d'une partie
        //pour les armes rouges, il n'y en a qu'une par biome donc 5 armes rouges possibles
        ArmesRougesInstanciees = new GameObject[5];

        //génération des 5 armes rouges
        for(int i = 0; i < 5; i++)
        {
            ArmesRougesInstanciees[i] = (GameObject)Instantiate(armeRougePrefab);
        }
    }

    public void GenererArmeBleue()
    {
        //seul le masterClient pour générer des objets
        if(PhotonNetwork.IsMasterClient)
        {
            //faire spawn un objet

        }
    }

}
