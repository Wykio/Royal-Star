using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System;

//Script pour le menu principal : choix entre créer ou rejoindre une partie
public class MenuPrincipalScript : MonoBehaviourPunCallbacks
{
    #region ClassVariables
    [SerializeField] public bool waitForPlayersToPlay = false;
    
    #endregion

    #region Interface
    [Header("Boutons Menu Principal")]
    [SerializeField] private Button boutonCreerRoom;
    [SerializeField] private Button boutonRejoindre;

    [Header ("Zones de texte Menu Principal")]
    [SerializeField] private Text message;
    [SerializeField] private Text erreur;
    #endregion

    #region Events
    public event Action OnlinePret;
    public event Action<string> OnError;
    public event Action OnClicCreer;
    public event Action OnClicRejoindre;
    public event Action connexionRoom;
    public event Action<int> ConnectedToMaster;
    public event Action<int> JoueurARejoint;
    public event Action<int> JoueurAQuitte;
    public event Action Deconnecte;
    public event Action MasterclientSwitch;
    public event Action FinDePartie;
    #endregion

    //Connexion à Photon et on ajoute les listeners aux boutons
    private void Awake()
    {
        Debug.Log("MenuPrincipal Awake");
        //connexion à Photon, gestion de l'exception en cas d'absence de connexion Internet
        try
        { 
            PhotonNetwork.ConnectUsingSettings();
        }
        catch(Exception e)
        {
            //on affiche le message dans le Text d'erreur
            OnError.Invoke("Erreur à la connexion à Photon");
        }

        //ajout des listeners
        boutonCreerRoom.onClick.AddListener(CreerRoom);
        boutonRejoindre.onClick.AddListener(RejoindreRoom);
    }

    //Quand on clique sur "Créer une partie"
    private void CreerRoom()
    {
        Debug.Log("MenuPrincipal creerRoom");

        
        //création de la room
        PhotonNetwork.CreateRoom("Room1", new RoomOptions
        {
            MaxPlayers = 20,
            PlayerTtl = 10000
        });

        OnClicCreer.Invoke();
    }

    //Quand on clique sur "Rejoidre une partie"
    private void RejoindreRoom()
    {
        Debug.Log("MenuPrincipal RejoindreRoom");

        //connexion à la room
        PhotonNetwork.JoinRandomRoom();

        OnClicRejoindre.Invoke();
    }

    //callback quand le joueur rejoind une room
    public override void OnJoinedRoom()
    {
        Debug.Log("MenuPrincipal OnJoinedRoom");
        StartCoroutine(SetWelcomeDebugAndSetReadyAtTheEndOfFrame());
        //StartCoroutine(WaitForOtherPlayerToLaunchGame());
    }

    //a la déconnexion du client local
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("MenuPrincipal OnDisconnected");
        Deconnecte?.Invoke();
    }

    //a la connexion d'un joueur à la room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("MenuPrincipal On Player enter room");
        //si le client local est MasterClient
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(InformPlayerJoinedEndOfFrame(newPlayer.ActorNumber));
        }
    }

    //methode du MasterClient pour gérer l'arrivée d'un nouveau joueur
    private IEnumerator InformPlayerJoinedEndOfFrame(int actorNumber)
    {
        Debug.Log("MenuPrincipal InformPlayerJoinedEndOfFrame");
        yield return new WaitForSeconds(2f);
        var i = 0;
        for (; i < PlayerNumbering.SortedPlayers.Length; i++)
        {
            if (actorNumber == PlayerNumbering.SortedPlayers[i].ActorNumber)
            {
                break;
            }
        }

        JoueurARejoint?.Invoke(i);
    }

    //quand un joueur quitte la room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("MenuPrincipal OnPlayerLeftRoom");
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

        JoueurAQuitte?.Invoke(i);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("MenuPrincipal OnMasterClientSwitched");
        MasterclientSwitch?.Invoke();

    }
    
    private IEnumerator SetWelcomeDebugAndSetReadyAtTheEndOfFrame()
    {
        Debug.Log("MenuPrincipal SetWelcomeDebugAndSetReadyAtTheEndOfFrame");

        if (waitForPlayersToPlay)
        {
            yield return new WaitForSeconds(30f);

            //s'il n'y a qu'un seul joueur dans la room, on quitte et retour au menu, sinon on lance la partie
            if (PlayerNumbering.SortedPlayers.Length <= 1)
            {
                erreur.text = "Partie annulée retour au menu";
                yield return new WaitForSeconds(2f);
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                StartCoroutine(SetPlayerReady());
            }
        }
        else
        {
            StartCoroutine(SetPlayerReady());
        }
    }
    
    private IEnumerator SetPlayerReady()
    {
        Debug.Log("MenuPrincipal SetPlayerReady");
        //desactivation des textes
        //message.gameObject.SetActive(false);
        //erreur.gameObject.SetActive(false);

        //Debug.Log($"Nombre de joueur : {PlayerNumbering.SortedPlayers.Length}");
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

        //Debug.Log( $"You are Actor : {PhotonNetwork.LocalPlayer.ActorNumber}\n " + $"You are controlling Avatar {i}, Let's Play !");

        OnlinePret?.Invoke();

        if (PhotonNetwork.IsMasterClient)
        {
            JoueurARejoint?.Invoke(i);
        }
    }
    
    /*
    // coroutine pour faire revenir le joueur au menu si personne rejoins la room
    private IEnumerator WaitForOtherPlayerToLaunchGame()
    {
        yield return new WaitForSeconds(10f);
        Debug.Log("Partie annulée retour au menu");
        FinDePartie?.Invoke();
        AfficherMenu();
    }
    */
}
