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
    private Queue<GameObject> armesRougesLibres;
    private List<GameObject> armesRougesPlacees;

    private GameObject[] ArmesVertesInstanciees;
    private Queue<GameObject> armesVertesLibres;
    private List<GameObject> armesVertesPlacees;

    private GameObject[] ArmesBleuesInstanciees;
    private Queue<GameObject> armesBleuesLibres;
    private List<GameObject> armesBleuesPlacees;

    //génération du pooling des items
    public void Awake()
    {
        //instancier toutes les armes possibles au cours d'une partie
        //pour les armes rouges, il n'y en a qu'une par biome donc 5 armes rouges possibles
        ArmesRougesInstanciees = new GameObject[5];

        //génération des 5 armes rouges
        for(int i = 0; i < 5; i++)
        {
            ArmesRougesInstanciees[i] = (GameObject)Instantiate(armeRougePrefab);
            armesRougesLibres.Enqueue(ArmesRougesInstanciees[i]);
        }

        //pour les armes vertes, il y en a 25 en tout
        ArmesVertesInstanciees = new GameObject[25];

        //génération des 25 armes vertes
        for(int i = 0; i < 25; i++)
        {
            ArmesVertesInstanciees[i] = (GameObject)Instantiate(armeVertePrefab);
            armesVertesLibres.Enqueue(ArmesVertesInstanciees[i]);
        }

        //pour les armes bleues, il y en a 30 en tout
        ArmesBleuesInstanciees = new GameObject[30];

        //génération des 30 armes bleues
        for(int i = 0; i < 30; i++)
        {
            ArmesBleuesInstanciees[i] = (GameObject)Instantiate(armeBleuePrefab);
            armesBleuesLibres.Enqueue(ArmesBleuesInstanciees[i]);
        }
    }

    public void GenererArmeBleue(Vector3 position)
    {
        //seul le masterClient pour générer des objets
        if(PhotonNetwork.IsMasterClient)
        {
            //faire spawn une arme bleue
            var armeBleue = armesBleuesLibres.Dequeue();

            //activation de l'item
            armeBleue.SetActive(true);
            armeBleue.transform.position = position;
        }
    }

}
