using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using MapGeneration;

//Script pour le menu principal : choix entre créer ou rejoindre une partie
public class MenuPrincipalScript : MonoBehaviourPunCallbacks
{
    #region ClassVariables
    [SerializeField] public bool waitForPlayersToPlay = false;
    [SerializeField] public int DureeMatchmaking = 30;
    [SerializeField] MapGeneratorBehaviour mapGenerator;
    [SerializeField] shipMotor gameController;
    [SerializeField] DataCollectorScript dataCollector;
    #endregion

    #region Interface
    [Header("Boutons Menu Principal")]
    [SerializeField] private Button boutonCreerRoom;
    [SerializeField] private Button boutonRejoindre;
    [SerializeField] private Button boutonQuitterJeu;

    [Header ("Zones de texte Menu Principal")]
    [SerializeField] private Text message;
    [SerializeField] private Text erreur;

    [Header("Boutons Menu Pause")]
    [SerializeField] private Button boutonReprendrePartie;
    [SerializeField] private Button boutonQuitterPartie;
    #endregion

    #region Events
    public event Action OnlinePret;
    public event Action<string> OnError;
    public event Action OnClicCreer;
    public event Action OnClicRejoindre;
    public event Action connexionRoom;
    public event Action LancementPartie;
    public event Action<int> MettreAJourLobby;
    public event Action<int, int> ConnectedToMaster;
    public event Action<int, int> JoueurARejoint;
    public event Action<int, int> JoueurAQuitte;
    public event Action Deconnecte;
    public event Action FinDePartie;
    public event Action decompteMatchmaking;
    public event Action masquerMenuPause;
    public event Action<string> debutGenerationMap;
    public event Action finGenerationMap;
    #endregion

    //Connexion à Photon et on ajoute les listeners aux boutons
    private void Awake()
    {
        //connexion à Photon, gestion de l'exception en cas d'absence de connexion Internet
        try
        { 
            PhotonNetwork.ConnectUsingSettings();
        }
        catch(Exception e)
        {
            //on affiche le message dans le Text d'erreur
            OnError.Invoke("Erreur à la connexion à Photon : " + e.ToString());
        }

        //ajout des listeners
        boutonCreerRoom.onClick.AddListener(CreerRoom);
        boutonRejoindre.onClick.AddListener(RejoindreRoom);
        boutonQuitterPartie.onClick.AddListener(QuitterPartie);
        boutonReprendrePartie.onClick.AddListener(ReprendrePartie);
        boutonQuitterJeu.onClick.AddListener(QuitterJeu);
        mapGenerator.mapGenereePourTous += LancerPartie;
    }

    //Quand on clique sur "Créer une partie"
    private void CreerRoom()
    {
        //création de la room
        PhotonNetwork.CreateRoom("Room" + UnityEngine.Random.Range(0, 9999).ToString(), new RoomOptions
        {
            MaxPlayers = 20,
            PlayerTtl = 10000
        });

        OnClicCreer.Invoke();
    }

    //Quand on clique sur "Rejoidre une partie"
    private void RejoindreRoom()
    {
        //connexion à la room
        PhotonNetwork.JoinRandomRoom();

        OnClicRejoindre.Invoke();
    }

    //Quand on clique sur Quitter dans le menu pause
    private void QuitterPartie()
    {
        //indiquer au masterclient que le client va quitter la room
        photonView.RPC("DeconnexionViaClientRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

        //traitement des données du dataCollector
        if(PhotonNetwork.IsMasterClient)
        {
            dataCollector.AfficherDico();
        }

        //quitter la room
        PhotonNetwork.LeaveRoom();

        //rechargement du jeu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //fonction du bouton Quitter qui ferme le jeu
    private void QuitterJeu()
    {
        Application.Quit();
    }

    //quand on clique sur reprendre, le menu pause est masqué ainsi que la souris
    private void ReprendrePartie()
    {
        masquerMenuPause.Invoke();

        gameController.EtatPauseJoueur(PhotonNetwork.LocalPlayer.ActorNumber, false);

        //curseur de la souris locké et non visible
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //callback quand le joueur rejoind une room, s'il est masterclient, il lance le lobby
    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient) StartCoroutine(GestionLobby());
    }

    //a la déconnexion du client local
    public override void OnDisconnected(DisconnectCause cause)
    {
        Deconnecte?.Invoke();
    }

    //quand un joueur quitte la room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //si le client local n'est pas MasterClient on ne fait rien 
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        //s'il est MasterClient, on appelle le delegate correspondant
        var i = 0;
        for (; i < PlayerNumbering.SortedPlayers.Length; i++)
        {
            if (otherPlayer.ActorNumber == PlayerNumbering.SortedPlayers[i].ActorNumber)
            {
                break;
            }
        }

        JoueurAQuitte?.Invoke(i, otherPlayer.ActorNumber);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(newMasterClient == PhotonNetwork.LocalPlayer)
        {
            // Debug.Log("NOUVEAU MASTERCLIENT : " + newMasterClient.ActorNumber + " Ce joueur est : " + PhotonNetwork.LocalPlayer.ActorNumber);
            //gameController.ActiverIntentReceivers();
        }
    }
    
    private IEnumerator GestionLobby()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(2f);

            //si le lobby est activé, on attends de nouveaux joueurs pendant 30 secondes
            if (waitForPlayersToPlay)
            {
                for (int i = 0; i < DureeMatchmaking; i++)
                {
                    MettreAJourLobby.Invoke(DureeMatchmaking - i);

                    yield return new WaitForSeconds(1f);
                }

                //s'il n'y a qu'un seul joueur dans la room, on quitte et retour au menu, sinon on lance la partie
                if (PlayerNumbering.SortedPlayers.Length <= 1)
                {
                    erreur.gameObject.SetActive(true);
                    erreur.text = "Pas assez de pilote - Partie annulée - Retour au menu";
                    yield return new WaitForSeconds(2f);
                    erreur.gameObject.SetActive(false);
                    PhotonNetwork.LeaveRoom();
                }
                else
                {
                    //quand la partie est lancée, la room est fermée pour éviter que d'autres joueurs rejoignent en cours
                    PhotonNetwork.CurrentRoom.IsOpen = false;

                    //générer la map et envoyer le tableau aux clients pour qu'ils la génèrent aussi
                    debutGenerationMap.Invoke("Génération de l'arène");
                    StartCoroutine(mapGenerator.DesignBiomes());

                    //la suite est lancée par la coroutine DesignBiomes quand celle-ci aura créé toute la map
                }
            }
            else
            {
                //générer la map et envoyer le tableau aux clients pour qu'ils la génèrent aussi
                debutGenerationMap.Invoke("Génération de l'arène");

                StartCoroutine(mapGenerator.DesignBiomes());
            }
        } 
    } 
    
    private IEnumerator SetPlayerReady()
    {
        yield return new WaitForSeconds(2f);
        var i = 0;

        //on recherche le joueur local dans la liste des joueurs, dès qu'on le trouve, on sort de la boucle
        for (; i < PlayerNumbering.SortedPlayers.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerNumbering.SortedPlayers[i].ActorNumber)
            {
                break;
            }
        }

        OnlinePret?.Invoke();
    }

    private void LancerPartie()
    {
        finGenerationMap.Invoke();

        //tous les clients connectés lancent SetPlayerReady
        photonView.RPC("SetPlayerReadyRPC", RpcTarget.All);

        //masquer l'interface du lobby
        LancementPartie.Invoke();

        //le masterclient s'occupe d'activer les vaisseaux pour tous les joueurs
        for (int i = 0; i < PlayerNumbering.SortedPlayers.Length; i++)
        {
            JoueurARejoint?.Invoke(i, PlayerNumbering.SortedPlayers[i].ActorNumber);
        }
        photonView.RPC("MasquerSourisRPC", RpcTarget.All);
    }

    [PunRPC]
    private void MasquerSourisRPC()
    {
        //on bloque et cache le curseur de la souris
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    [PunRPC]
    private void SetPlayerReadyRPC()
    {
        StartCoroutine(SetPlayerReady());
    }

    //RPC envoyé par un client au masterclient pour se déconnecter de la partie
    [PunRPC]
    private void DeconnexionViaClientRPC(int playerActorNumber)
    {
        //si le client local n'est pas MasterClient on ne fait rien 
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        //s'il est MasterClient, on appelle le delegate correspondant
        var i = 0;
        for (; i < PlayerNumbering.SortedPlayers.Length; i++)
        {
            if (playerActorNumber == PlayerNumbering.SortedPlayers[i].ActorNumber)
            {
                break;
            }
        }

        JoueurAQuitte?.Invoke(i, playerActorNumber);
    }
}
