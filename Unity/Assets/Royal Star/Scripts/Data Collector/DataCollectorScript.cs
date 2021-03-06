﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;

public class DataCollectorScript : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] ShipExposer[] joueurs = new ShipExposer[20];

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
    private List<String> statsData = new List<String>();
    private int mortParTir = 0;
    private int mortParBiome = 0;
    public String login = null;
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

    public String FloatToString(float f)
    {
        return f.ToString("0.00").Replace(',', '.');
    }

    public String Vector3ToString(Vector3 u)
    {
        return $"[{FloatToString(u.x)},{FloatToString(u.y)},{FloatToString(u.z)}]";
    }

    public String QuaternionToString(Quaternion q)
    {
        return $"[{FloatToString(q.w)},{FloatToString(q.x)},{FloatToString(q.y)},{FloatToString(q.z)}]";
    }

    public String ShootData(String type, Vector3 pos, Quaternion rot)
    {
        return $"{{\"login\":\"{login}\",\"type\":\"{type}\",\"position\":{Vector3ToString(pos)},\"rotation\":{QuaternionToString(rot)},\"date\":\"{DateTime.Now.ToString()}\"}}";
    }

    public String PositionData(String type, Vector3 pos)
    {
        return $"{{\"login\":\"{login}\",\"type\":\"{type}\",\"position\":{Vector3ToString(pos)},\"date\":\"{DateTime.Now.ToString()}\"}}";
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
        foreach (var joueur in joueurs)
            if (joueur.vaisseauActif)
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

        //lancement de la coroutine pour controller le temps en l'air
        StartCoroutine(TempsAerien());
    }

    //Coroutine qui controlle l'état Aerien de chaque vaisseau et incrémente son temps en l'air s'il l'est
    private IEnumerator TempsAerien()
    {
        while (gameController.getGameStarted())
        {
            foreach (var joueur in joueurs)
                if (joueur.isActiveAndEnabled && joueur.Aerien)
                    tempsPasseEnLAirParJoueur[joueur.playerID]++;

            yield return new WaitForSeconds(1);
        }
    }

    //le joueur idJoueur tire avec le laser de base, son compteur est incrémenté
    private void TirBasiqueJoueur(int idJoueur, Vector3 pos, Quaternion rot)
    {
        tirsLaserBasiqueParJoueur[idJoueur]++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(ShootData("POS_BASIC_SHOOT", pos, rot));
    }

    //le joueur idJoueur tire avec ses armes bleues, son compteur est incrémenté
    private void TirArmesBleues(int idJoueur, Vector3 pos, Quaternion rot)
    {
        tirsArmesBleuesParJoueur[idJoueur]++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(ShootData("POS_BLUE_SHOOT", pos, rot));
    }

    //le joueur idJoueur tire avec ses armes vertes, son compteur est incrémenté
    private void TirArmesVertes(int idJoueur, Vector3 pos, Quaternion rot)
    {
        tirsArmesVertesParJoueur[idJoueur]++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(ShootData("POS_GREEN_SHOOT", pos, rot));
    }

    //le joueur idJoueur tire avec ses armes rouges, son compteur est incrémenté
    private void TirArmesRouges(int idJoueur, Vector3 pos, Quaternion rot)
    {
        tirsArmesRougesParJoueur[idJoueur]++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(ShootData("POS_RED_SHOOT", pos, rot));
    }

    //Le joueur idJoueur passe un portail, son compteur est incrémenté
    public void PortailPasse(int idJoueur)
    {
        portailsPassesParJoueur[idJoueur]++;
    }

    //Lors qu'un joueur meurt par tir, le compteur est incrémenté
    public void MortParTir(int idJoueur, Vector3 pos)
    {
        mortParTir++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(PositionData("POS_SHOOT_DEATH", pos));
    }

    //lors qu'un joueur meurt par fermeture des portails ou condition environnementales, le compteur est incrémenté
    public void MortParBiome(int idJoueur, Vector3 pos)
    {
        mortParBiome++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(PositionData("POS_BIOME_DEATH", pos));
    }

    //quand un joueur fait un kill, son compteur est incrémenté
    private void KillParJoueur(int idJoueur, Vector3 pos)
    {
        killParJoueur[idJoueur]++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(PositionData("POS_KILL_PLAYER", pos));
    }

    //quand un joueur fait un kill en étant en l'air, son compteur est incrémenté
    private void KillAerienParJoueur(int idJoueur, Vector3 pos)
    {
        killAerienParJoueur[idJoueur]++;
        if (PhotonNetwork.LocalPlayer.ActorNumber == idJoueur)
            statsData.Add(PositionData("POS_AERIALKILL_PLAYER", pos));
    }

    //afficher pour test
    public void AfficherDico()
    {
        if (login == null) return;
        UnityWebRequest request = new UnityWebRequest("https://royalstar-api.herokuapp.com/stats", "POST");
        string rawBody = "[";

        for (int i = 0; i < statsData.Count; i++) {
            rawBody += statsData[i];
            if (i + 1 < statsData.Count)
                rawBody += ',';
        }
        rawBody += ']';
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(rawBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        statsData.Clear();
    }
}
