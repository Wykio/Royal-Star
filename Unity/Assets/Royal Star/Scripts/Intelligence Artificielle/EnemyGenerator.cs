using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class EnemyGenerator : MonoBehaviour
{
    [Header ("Référence")]
    [SerializeField] private GameObject botPrefab;
    [SerializeField] private PhotonView photonView;
    
    [Header("Pooling des bots")]
    [SerializeField] private int nbBots;
    
    private EnemyExposer[] botsInstancies;
    private Queue<EnemyExposer> botsLibres;
    private List<EnemyExposer> botsPlacees;
    
    private GameObject bot;
    
    public void Awake()
    {
        //Initialisation des listes et queues 
        botsLibres = new Queue<EnemyExposer>(nbBots);
        botsPlacees = new List<EnemyExposer>(nbBots);
        
        //instancier tout les bots possible au cours d'une partie
        botsInstancies = new EnemyExposer[nbBots];

        //génération des armes rouges
        for(int i = 0; i < nbBots; i++)
        {
            bot = (GameObject)Instantiate(botPrefab);
            bot.SetActive(false);
            botsInstancies[i] = bot.GetComponent<EnemyExposer>();
            botsLibres.Enqueue(botsInstancies[i]); 
        }
    }
    
    public List<EnemyExposer> getBotsPlacees()
    {
        return botsPlacees;
    }
    
    public void GenererBot(Vector3 position)
    {
        //seul le masterClient peut donner l'ordre de générer des bots aux clients
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GenererBotRPC", RpcTarget.All, position);
            // Debug.Log("generating bot");
        }
    }

    [PunRPC]
    private void GenererBotRPC(Vector3 position)
    {
        //faire spawn un bot
        var bot = botsLibres.Dequeue();

        //activation du bot
        bot.ActivationBot();
        bot.SetPosition(position);

        //ajout du bot dans la liste des bots placés
        botsPlacees.Add(bot);
    }
}
