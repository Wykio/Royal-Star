﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataCollectorScript : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] ShipExposer[] joueurs = new ShipExposer[20];
    [SerializeField] string cheminFichierData;

    [Header("Références")]
    [SerializeField] shipMotor gameController;

    #region Données à collecter
    private Dictionary<int, int> tirsLaserBasiqueParJoueur;
    private Dictionary<int, int> tirsArmesBleuesParJoueur;
    private Dictionary<int, int> tirsArmesVertesParJoueur;
    private Dictionary<int, int> tirsArmesRougesParJoueur;
    private Dictionary<int, int> portailsPassesParJoueur;
    private Dictionary<int, int> tempsPasseEnLAirParJoueur;
    private Dictionary<int, int> killParJoueur;
    private Dictionary<int, int> killAerienParJoueur;
    private List<Vector3> positionsTirsBasique = new List<Vector3>();
    private List<Vector3> positionsTirsArmeBleue = new List<Vector3>();
    private List<Vector3> positionsTirsArmeVerte = new List<Vector3>();
    private List<Vector3> positionsTirsArmeRouge = new List<Vector3>();
    private List<Vector3> positionsMort = new List<Vector3>();
    private List<Vector3> positionsKill = new List<Vector3>();
    private int mortParTir = 0;
    private int mortParBiome = 0;

    #endregion

    private void Awake()
    {
        foreach (var joueur in joueurs)
        {
            joueur.dataCollector = this;
        }

        gameController.tirLaserBasique += TirBasiqueJoueur;
        gameController.tirArmeBleue += TirArmesBleues;
        gameController.tirArmeVerte += TirArmesVertes;
        gameController.tirArmeRouge += TirArmesRouges;
        gameController.mortParBiomeDeLaPartie += MortParBiome;
    }

    public void InitialiserLesDictionnaires()
    {
        //initialisation des dictionnaires
        tirsLaserBasiqueParJoueur = new Dictionary<int, int>(20);
        tirsArmesBleuesParJoueur = new Dictionary<int, int>(20);
        tirsArmesBleuesParJoueur = new Dictionary<int, int>(20);
        tirsArmesVertesParJoueur = new Dictionary<int, int>(20);
        tirsArmesRougesParJoueur = new Dictionary<int, int>(20);
        tempsPasseEnLAirParJoueur = new Dictionary<int, int>(20);
        portailsPassesParJoueur = new Dictionary<int, int>(20);
        killParJoueur = new Dictionary<int, int>(20);
        killAerienParJoueur = new Dictionary<int, int>(20);

        //on crée une nouvelle entrée dans tous les dictionnaire pour chaque joueurs dans la partie
        foreach(var joueur in joueurs)
        {
            if(joueur.vaisseauActif)
            {
                tirsLaserBasiqueParJoueur.Add(joueur.playerID, 0);
                tirsArmesBleuesParJoueur.Add(joueur.playerID, 0);
                tirsArmesVertesParJoueur.Add(joueur.playerID, 0);
                tirsArmesRougesParJoueur.Add(joueur.playerID, 0);
                portailsPassesParJoueur.Add(joueur.playerID, 0);
                tempsPasseEnLAirParJoueur.Add(joueur.playerID, 0);
                killParJoueur.Add(joueur.playerID, 0);
                killAerienParJoueur.Add(joueur.playerID, 0);
            }
        }

        //lancement de la coroutine pour controller le temps en l'air
        StartCoroutine(TempsAerien());
    }

    //Coroutine qui controlle l'état Aerien de chaque vaisseau et incrémente son temps en l'air s'il l'est
    private IEnumerator TempsAerien()
    {
        while(gameController.getGameStarted())
        {
            foreach(var joueur in joueurs)
            {
                if(joueur.isActiveAndEnabled && joueur.Aerien)
                {
                    tempsPasseEnLAirParJoueur[joueur.playerID] += 1;
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    //le joueur idJoueur tire avec le laser de base, son compteur est incrémenté
    private void TirBasiqueJoueur(int idJoueur, Vector3 pos)
    {
        tirsLaserBasiqueParJoueur[idJoueur] += 1;
        positionsTirsBasique.Add(pos);
    }

    //le joueur idJoueur tire avec ses armes bleues, son compteur est incrémenté
    private void TirArmesBleues(int idJoueur, Vector3 pos)
    {
        tirsArmesBleuesParJoueur[idJoueur] += 1;
        positionsTirsArmeBleue.Add(pos);
    }

    //le joueur idJoueur tire avec ses armes vertes, son compteur est incrémenté
    private void TirArmesVertes(int idJoueur, Vector3 pos)
    {
        tirsArmesVertesParJoueur[idJoueur] += 1;
        positionsTirsArmeVerte.Add(pos);
    }

    //le joueur idJoueur tire avec ses armes rouges, son compteur est incrémenté
    private void TirArmesRouges(int idJoueur, Vector3 pos)
    {
        tirsArmesRougesParJoueur[idJoueur] += 1;
        positionsTirsArmeRouge.Add(pos);
    }

    //Le joueur idJoueur passe un portail, son compteur est incrémenté
    public void PortailPasse(int idJoueur)
    {
        portailsPassesParJoueur[idJoueur] += 1;
    }

    //Lors qu'un joueur meurt par tir, le compteur est incrémenté
    public void MortParTir(Vector3 pos)
    {
        mortParTir++;
        positionsMort.Add(pos);
    }

    //lors qu'un joueur meurt par fermeture des portails ou condition environnementales, le compteur est incrémenté
    private void MortParBiome()
    {
        mortParBiome++;
    }

    //quand un joueur fait un kill, son compteur est incrémenté
    private void KillParJoueur(int idJoueur, Vector3 pos)
    {
        killParJoueur[idJoueur] += 1;
        positionsKill.Add(pos);
    }

    //quand un joueur fait un kill en étant en l'air, son compteur est incrémenté
    private void KillAerienParJoueur(int idJoueur, Vector3 pos)
    {
        killAerienParJoueur[idJoueur] += 1;
        positionsKill.Add(pos);
    }

    //afficher pour test
    public void AfficherDico()
    {
        List<string> dataToFile;
        try
        {
            string[] dataFromFile = File.ReadAllLines(cheminFichierData);
            dataToFile = new List<string>(dataFromFile);
        }
        catch(Exception e)
        {
            dataToFile = new List<string>();
        }

        foreach (var data in tirsLaserBasiqueParJoueur)
        {
            Debug.Log("DATA COLLECTED : joueur " + data.Key + " a tiré " + data.Value + " fois au laser de base");
        }

        foreach (var data in tirsArmesBleuesParJoueur)
        {
            Debug.Log("DATA COLLECTED : joueur " + data.Key + " a tiré " + data.Value + " fois aux armes bleues");
        }

        foreach (var data in tirsArmesVertesParJoueur)
        {
            Debug.Log("DATA COLLECTED : joueur " + data.Key + " a tiré " + data.Value + " fois aux armes vertes");
        }

        foreach (var data in tirsArmesRougesParJoueur)
        {
            Debug.Log("DATA COLLECTED : joueur " + data.Key + " a tiré " + data.Value + " fois aux armes rouges");
        }

        foreach (var data in portailsPassesParJoueur)
        {
            Debug.Log("DATA COLLECTED : joueur " + data.Key + " a atteint le  " + (data.Value + 1) + " biome");
        }

        foreach (var data in tempsPasseEnLAirParJoueur)
        {
            Debug.Log("DATA COLLECTED : joueur " + data.Key + " a passé " + data.Value + " secondes en l'air");
        }

        //nouvelle game, on le précise dans le fichier de data
        dataToFile.Add("NEWGAME");

        //écriture des positions des tirs basiques
        foreach (var positionData in positionsTirsBasique)
        {
            dataToFile.Add("basic_" + positionData.x + "_" + positionData.y + "_" + positionData.z);
        }
       
        //écriture des positions des tirs d'armes bleues
        foreach (var positionData in positionsTirsArmeBleue)
        {
            dataToFile.Add("bleue_" + positionData.x + "_" + positionData.y + "_" + positionData.z);
        }

        //écriture des positions des tirs d'armes vertes
        foreach (var positionData in positionsTirsArmeVerte)
        {
            dataToFile.Add("verte_" + positionData.x + "_" + positionData.y + "_" + positionData.z);
        }

        //écriture des positions des tirs d'armes bleues
        foreach (var positionData in positionsTirsArmeRouge)
        {
            dataToFile.Add("rouge_" + positionData.x + "_" + positionData.y + "_" + positionData.z);
        }


        //écriture des positions des joueurs au moment où ils font un kill
        foreach (var positionData in positionsKill)
        {
            dataToFile.Add("kill_" + positionData.x + "_" + positionData.y + "_" + positionData.z);
        }

        //écriture des positions des joueurs au moment de leur mort
        foreach (var positionData in positionsMort)
        {
            dataToFile.Add("mort_" + positionData.x + "_" + positionData.y + "_" + positionData.z);
        }

        File.WriteAllLines(@cheminFichierData, dataToFile);
        dataToFile.Clear();
    }
}
