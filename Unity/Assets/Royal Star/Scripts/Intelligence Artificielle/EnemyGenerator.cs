using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class EnemyGenerator : MonoBehaviour
{
    [Header ("Référence")]
    [SerializeField] private PhotonView photonView;
    
    [Header("Pooling des bots")]
    [SerializeField] private int nbBots;
    [SerializeField] public EnemyExposer[] botsExposers;

    
    private GameObject bot;
    
    public void LancementBot(float hauteur, int indiceBiome, int nbBots)
    {
        int i = 0;

        foreach(var bot in botsExposers)
        {
            if (i == nbBots) break;

            Vector3 position = new Vector3(Random.Range(0, (4-indiceBiome) * 1000), hauteur, Random.Range(0, (4 - indiceBiome) * 1000));
            PlacerBot(position, i);
            i++;
        }
    }

    //désactiver tous les bots
    public void DesactiverTousLesBots()
    {
        foreach(var bot in botsExposers)
        {
            bot.DesactivationBot();
        }
    }
    
    public void PlacerBot(Vector3 position, int indice)
    {
        //seul le masterClient peut donner l'ordre de générer des bots aux clients
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PlacerBotRPC", RpcTarget.All, position, indice);
        }
    }

    [PunRPC]
    private void PlacerBotRPC(Vector3 position, int indice)
    {
        //activation du bot
        botsExposers[indice].ActivationBot();
        botsExposers[indice].SetPosition(position);
    }
}
