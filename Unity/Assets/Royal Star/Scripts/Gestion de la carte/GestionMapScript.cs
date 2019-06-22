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
    [SerializeField] private int tailleBiome = 4000;

    [Header("Paramétrage des items")]
    [SerializeField] private int nbArmesBleuesBiome1;
    [SerializeField] private int nbArmesVertesBiome1;
    [SerializeField] private int nbArmesRougesBiome1;

    [SerializeField] private int nbArmesBleuesBiome2;
    [SerializeField] private int nbArmesVertesBiome2;
    [SerializeField] private int nbArmesRougesBiome2;

    [SerializeField] private int nbArmesBleuesBiome3;
    [SerializeField] private int nbArmesVertesBiome3;
    [SerializeField] private int nbArmesRougesBiome3;

    [SerializeField] private int nbArmesBleuesBiome4;
    [SerializeField] private int nbArmesVertesBiome4;
    [SerializeField] private int nbArmesRougesBiome4;

    [Header("Références")]
    [SerializeField] private shipMotor ShipManager;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private ItemGeneratorScript itemGenerator;

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

    //Coroutine pour gérer les fermetures et ouvertures des portails ainsi que les apparitions des items
    public IEnumerator GestionMap()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //placer les items du premier biome
            PlacerItemsSurBiome(nbArmesBleuesBiome1, nbArmesVertesBiome1, nbArmesRougesBiome1, 250, tailleBiome, tailleBiome);

            for (int i = 0; i < listesPortailsParBiome.Count; i++)
            {
                //attendre le temps de la durée du biome avant d'ouvrir les portails
                yield return new WaitForSeconds(dureeBiome - dureeOuverturePortails);
                ActiverPortailDuBiome(biomeCourant);

                //activer items du prochain biome
                switch(biomeCourant)
                {
                    //les items du premier biomes sont déjà placés donc on ajoute les items du biome suivant
                    case 0:
                        PlacerItemsSurBiome(nbArmesBleuesBiome2, nbArmesVertesBiome2, nbArmesRougesBiome2, (i + 1) * 5000 + 250, tailleBiome - (i + 1) * 1000, tailleBiome - (i + 1) * 1000);
                        break;

                    //placer les items du 3ème biome
                    case 1:
                        PlacerItemsSurBiome(nbArmesBleuesBiome3, nbArmesVertesBiome3, nbArmesRougesBiome3, (i + 1) * 5000 + 250, tailleBiome - (i + 1) * 1000, tailleBiome - (i + 1) * 1000);
                        break;

                    //placer les items du dernier biome
                    case 2:
                        PlacerItemsSurBiome(nbArmesBleuesBiome4, nbArmesVertesBiome4, nbArmesRougesBiome4, (i + 1) * 5000 + 250, tailleBiome - (i + 1) * 1000, tailleBiome - (i + 1) * 1000);
                        break;
                    default:
                        break;
                }

                yield return new WaitForSeconds(dureeOuverturePortails);
                DesactiverPortailDuBiome(biomeCourant);

                //désactiver items du biome courant
                DesactiverItemsDuBiome(5000 * biomeCourant + 1000);

                ShipManager.UpdateHauteurMort();

                biomeCourant++;
            }
        }
    }

    //fonction pour ordonner aux clients de désavtiver tous les objets sous la hauteur donnée en paramètre
    private void DesactiverItemsDuBiome(int hauteurLimite)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DesactiverItemsDuBiomeRPC", RpcTarget.All, hauteurLimite);
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

    //fonction pour ordonner aux clients de placer les items dans le biome
    public void PlacerItemsSurBiome(int nbArmesBleues, int nbArmesVertes, int nbArmesRouges, int hauteurSpawn, int longueur, int largeur)
    {
        string positionsArmesBleuesData = "";

        for (int i = 0; i < nbArmesBleues; i++)
        {
            positionsArmesBleuesData += (Random.Range(0, longueur).ToString() + "/" + hauteurSpawn + "/" + Random.Range(0, longueur).ToString() + "_");
        }
        positionsArmesBleuesData = positionsArmesBleuesData.Substring(0, positionsArmesBleuesData.Length - 1);

        string positionsArmesVertesData = "";

        for (int i = 0; i < nbArmesVertes; i++)
        {
            positionsArmesVertesData += (Random.Range(0, longueur).ToString() + "/" + hauteurSpawn + "/" + Random.Range(0, longueur).ToString() + "_");
        }

        positionsArmesVertesData = positionsArmesVertesData.Substring(0, positionsArmesVertesData.Length - 1);

        string positionsArmesRougesData = "";

        for (int i = 0; i < nbArmesRouges; i++)
        {
            positionsArmesRougesData += (Random.Range(0, longueur).ToString() + "/" + hauteurSpawn + "/" + Random.Range(0, longueur).ToString() + "_");
        }

        positionsArmesRougesData = positionsArmesRougesData.Substring(0, positionsArmesRougesData.Length - 1);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PlacerItemsSurBiomeRPC", RpcTarget.All, positionsArmesBleuesData, positionsArmesVertesData, positionsArmesRougesData);
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

    [PunRPC]
    private void DesactiverItemsDuBiomeRPC(int hauteurLimite)
    {
        var armesPlacees = itemGenerator.GetArmesBleuesPlacees();

        foreach (var arme in armesPlacees)
        {
            //pour chaque arme placées, si sa hauteur est inférieure à celle donnée, on la considère comme ramassée et elle est remise dans le pooling
            if (arme.GetItemTransform().position.y < hauteurLimite)
            {
                arme.SetRamasse(true);
            }
        }

        armesPlacees = itemGenerator.GetArmesVertesPlacees();

        foreach (var arme in armesPlacees)
        {
            if (arme.GetItemTransform().position.y < hauteurLimite)
            {
                arme.SetRamasse(true);
            }
        }

        armesPlacees = itemGenerator.GetArmesRougesPlacees();

        foreach (var arme in armesPlacees)
        {
            if (arme.GetItemTransform().position.y < hauteurLimite)
            {
                arme.SetRamasse(true);
            }
        }
    }

    [PunRPC]
    private void PlacerItemsSurBiomeRPC(string positionsArmesBleuesData, string positionsArmesVertesData, string positionsArmesRougesData)
    {
        //split des datas en listes pour en extraire les coordonnées
        var positionsArmesBleues = positionsArmesBleuesData.Split('_');
        var positionsArmesVertes = positionsArmesVertesData.Split('_');
        var positionsArmesRouges = positionsArmesRougesData.Split('_');

        foreach (var position in positionsArmesBleues)
        {
            var extract = position.Split('/');

            Vector3 pos = new Vector3(float.Parse(extract[0]), float.Parse(extract[1]), float.Parse(extract[2]));

            itemGenerator.GenererArmeBleue(pos);
        }

        foreach(var position in positionsArmesVertes)
        {
            var extract = position.Split('/');

            Vector3 pos = new Vector3(float.Parse(extract[0]), float.Parse(extract[1]), float.Parse(extract[2]));

            itemGenerator.GenererArmeVerte(pos);
        }

        foreach(var position in positionsArmesRouges)
        {
            var extract = position.Split('/');

            Vector3 pos = new Vector3(float.Parse(extract[0]), float.Parse(extract[1]), float.Parse(extract[2]));

            itemGenerator.GenererArmeRouge(pos);
        }
    }
}
