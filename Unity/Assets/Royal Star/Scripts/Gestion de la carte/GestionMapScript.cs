using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GestionMapScript : MonoBehaviour
{
    [Header("Timers des biomes")]
    [SerializeField] private float dureeBiome;
    [SerializeField] private float dureeOuverturePortails;
    [SerializeField] public int biomeCourant;

    [Header("Références")]
    [SerializeField] private shipMotor ShipManager;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private ItemGeneratorScript itemGenerator;

    private List<List<GameObject>> listesPortailsParBiome = new List<List<GameObject>>();
    private float debutGame;

    public void SetDebutGame(float t)
    {
        photonView.RPC("SetDebutGameRPC", RpcTarget.All, t);
        Debug.Log("EVOLUTION MAP NB BIOME : " + listesPortailsParBiome.Count);
    }

    //initialise les listes de portails
    public void AddListePortailParBiome(List<GameObject> listePortails)
    {
        listesPortailsParBiome.Add(listePortails);
    }

    //Coroutine pour gérer les fermetures et ouvertures des portails
    public IEnumerator GestionMap()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < listesPortailsParBiome.Count; i++)
            {
                yield return new WaitForSeconds(dureeBiome - dureeOuverturePortails);
                ActiverPortailDuBiome(biomeCourant);

                yield return new WaitForSeconds(dureeOuverturePortails);
                DesactiverPortailDuBiome(biomeCourant);

                ShipManager.UpdateHauteurMort();

                biomeCourant++;
            }
        }
    }

    private void PlacerItemsSurBiome(int biomeCourant)
    {
        if(biomeCourant == 0)
        {

        }
    }

    //fonction pour activer les portails du biome passé en paramètre
    public void ActiverPortailDuBiome(int numBiome)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ActiverPortailDuBiomeRPC", RpcTarget.All, numBiome);
        }
    }

    //fonction pour désactiver les portails du biome passé en paramètre
    public void DesactiverPortailDuBiome(int numBiome)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DesactiverPortailDuBiomeRPC", RpcTarget.All, numBiome);
        }
    }

    [PunRPC]
    private void ActiverPortailDuBiomeRPC(int numBiome)
    {
        for (int i = 0; i < listesPortailsParBiome[numBiome].Count; i++)
        {
            listesPortailsParBiome[numBiome][i].SetActive(true);
        }
    }

    [PunRPC]
    private void DesactiverPortailDuBiomeRPC(int numBiome)
    {
        for (int i = 0; i < listesPortailsParBiome[numBiome].Count; i++)
        {
            listesPortailsParBiome[numBiome][i].SetActive(false);
        }
    }

    [PunRPC]
    private void SetDebutGameRPC(float t)
    {
        debutGame = t;
    }
}
