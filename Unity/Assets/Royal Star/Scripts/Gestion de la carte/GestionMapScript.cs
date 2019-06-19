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

    private List<List<GameObject>> listesPortailsParBiome = new List<List<GameObject>>();
    private float debutGame;

    public void SetDebutGame(float t)
    {
        photonView.RPC("SetDebutGameRPC", RpcTarget.All, t);
    }

    //initialise les listes de portails
    public void AddListePortailParBiome(List<GameObject> listePortails)
    {
        listesPortailsParBiome.Add(listePortails);
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

    private void Update()
    {
        //seul le masterclient gère les changements de biomes
        if(ShipManager.getGameStarted() && PhotonNetwork.IsMasterClient)
        {
            //si le moment d'ouvrir les portails du biome courant arrive, on lance leur ouverture
            if(Time.time - debutGame > debutGame + (dureeBiome-dureeOuverturePortails) && biomeCourant < listesPortailsParBiome.Count)
            {
                Debug.Log("OUVERTURE PORTAILS BIOME " + biomeCourant);
                //activer les portails du biomes
                ActiverPortailDuBiome(biomeCourant);

                //les fermer au bout de {dureeOuverture] secondes
                StartCoroutine(FermeturePortail(dureeOuverturePortails, biomeCourant));

                //mettre à jour le timer pour le prochain biome
                debutGame = Time.time + dureeOuverturePortails;

                //passage au biome suivant
                biomeCourant += 1;
            }
        }
    }

    private IEnumerator FermeturePortail(float duree, int nbBiome)
    {
        //les portails se referment au bout de la duree définie
        yield return new WaitForSeconds(duree);

        DesactiverPortailDuBiome(nbBiome);
        ShipManager.UpdateHauteurMort();
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
