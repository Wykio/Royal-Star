using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void TirBasiqueJoueur(int idJoueur)
    {
        tirsLaserBasiqueParJoueur[idJoueur] += 1;
    }

    //le joueur idJoueur tire avec ses armes bleues, son compteur est incrémenté
    private void TirArmesBleues(int idJoueur)
    {
        tirsArmesBleuesParJoueur[idJoueur] += 1;
    }

    //le joueur idJoueur tire avec ses armes vertes, son compteur est incrémenté
    private void TirArmesVertes(int idJoueur)
    {
        tirsArmesVertesParJoueur[idJoueur] += 1;
    }

    //le joueur idJoueur tire avec ses armes rouges, son compteur est incrémenté
    private void TirArmesRouges(int idJoueur)
    {
        tirsArmesRougesParJoueur[idJoueur] += 1;
    }

    //Le joueur idJoueur passe un portail, son compteur est incrémenté
    public void PortailPasse(int idJoueur)
    {
        portailsPassesParJoueur[idJoueur] += 1;
    }

    //Lors qu'un joueur meurt par tir, le compteur est incrémenté
    public void MortParTir()
    {
        mortParTir++;
    }

    //lors qu'un joueur meurt par fermeture des portails ou condition environnementales, le compteur est incrémenté
    private void MortParBiome()
    {
        mortParBiome++;
    }

    //quand un joueur fait un kill, son compteur est incrémenté
    private void KillParJoueur(int idJoueur)
    {
        killParJoueur[idJoueur] += 1;
    }

    //quand un joueur fait un kill en étant en l'air, son compteur est incrémenté
    private void KillAerienParJoueur(int idJoueur)
    {
        killAerienParJoueur[idJoueur] += 1;
    }

    //afficher pour test
    public void AfficherDico()
    {
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
    }
}
