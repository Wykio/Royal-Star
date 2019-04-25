﻿using System.Collections;
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
    #region Interface
    [Header ("Boutons")]
    [SerializeField] private Button boutonCreerRoom;
    [SerializeField] private Button boutonRejoindre;

    [Header ("Zones de texte")]
    [SerializeField] private Text message;
    [SerializeField] private Text erreur;
    #endregion

    #region Events
    public event Action OnlinePret;
    public event Action<int> JoueurARejoint;
    public event Action<int> JoueurAQuitte;
    public event Action Deconnecte;
    public event Action MasterclientSwitch;
    #endregion

    //Connexion à Photon et on ajoute les listeners aux boutons
    private void Awake()
    {
        //connexion à Photon, gestion de l'exception en cas d'absence de connexion Internet
        try
        {
            erreur.gameObject.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
        }
        catch(Exception e)
        {
            //on affiche le message dans le Text d'erreur
            erreur.text = e.Message;
        }

        //ajout des listeners
        boutonCreerRoom.onClick.AddListener(CreerRoom);
        boutonRejoindre.onClick.AddListener(RejoindreRoom);
    }

    // Start is called before the first frame update
    void Start()
    {
        AfficherMenu();
    }

    //affichage des boutons du menu et des textes
    public void AfficherMenu()
    {
        boutonCreerRoom.gameObject.SetActive(false);
        boutonRejoindre.gameObject.SetActive(false);

        message.text = "Royal Star";
        erreur.text = "Connexion au Master...";
    }

    //dès qu'on est connecté au Master on rend les boutons Créer et Rejoindre cliquables
    public override void OnConnectedToMaster()
    {
        boutonCreerRoom.gameObject.SetActive(true);
        boutonRejoindre.gameObject.SetActive(true);
        erreur.text = "";
    }

    //Quand on clique sur "Créer une partie"
    private void CreerRoom()
    {
        //desactivation des boutons
        boutonCreerRoom.gameObject.SetActive(false);
        boutonCreerRoom.interactable = false;

        boutonRejoindre.gameObject.SetActive(false);
        boutonRejoindre.interactable = false;

        //création de la room
        PhotonNetwork.CreateRoom("Room1", new RoomOptions
        {
            MaxPlayers = 20,
            PlayerTtl = 10000
        });

        //désactivation des textes
        message.gameObject.SetActive(false);
        erreur.gameObject.SetActive(false);
    }

    //Quand on clique sur "Rejoidre une partie"
    private void RejoindreRoom()
    {
        //desactivation des boutons
        boutonCreerRoom.gameObject.SetActive(false);
        boutonCreerRoom.interactable = false;

        boutonRejoindre.gameObject.SetActive(false);
        boutonRejoindre.interactable = false;

        //connexion à la room
        PhotonNetwork.JoinRandomRoom();

        //désactivation des textes
        message.gameObject.SetActive(false);
        erreur.gameObject.SetActive(false);
    }

    //callback quand le joueur rejoind une room
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        StartCoroutine(SetWelcomeDebugAndSetReadyAtTheEndOfFrame());
    }

    //a la déconnexion du client local
    public override void OnDisconnected(DisconnectCause cause)
    {
        Deconnecte?.Invoke();
    }

    //a la connexion d'un joueur à la room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //si le client local est MasterClient
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(InformPlayerJoinedEndOfFrame(newPlayer.ActorNumber));
        }
    }

    //methode du MasterClient pour gérer l'arrivée d'un nouveau joueur
    private IEnumerator InformPlayerJoinedEndOfFrame(int actorNumber)
    {
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
        //si le client local n'est pas MasterClient on ne fait rien 
        if(!PhotonNetwork.IsMasterClient)
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
        MasterclientSwitch?.Invoke();

    }

    private IEnumerator SetWelcomeDebugAndSetReadyAtTheEndOfFrame()
    {
        yield return new WaitForSeconds(2f);

        var i = 0;
        for (; i < PlayerNumbering.SortedPlayers.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerNumbering.SortedPlayers[i].ActorNumber)
            {
                break;
            }
        }

        Debug.Log( $"You are Actor : {PhotonNetwork.LocalPlayer.ActorNumber}\n " + $"You are controlling Avatar {i}, Let's Play !");

        OnlinePret?.Invoke();

        if (PhotonNetwork.IsMasterClient)
        {
            JoueurARejoint?.Invoke(i);
        }
    }
}