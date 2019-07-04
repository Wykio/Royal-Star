using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using MapGeneration;

public class InterfaceManager : MonoBehaviourPunCallbacks
{
    [Header("éléments de l'interface")]
    [SerializeField] private Text erreur;
    [SerializeField] private Text titre;
    [SerializeField] private Button creerRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button quitterJeuButton;
    [SerializeField] private Button OptionButton;
    [SerializeField] private Text titreLobby;
    [SerializeField] private Text decompteLobby;
    [SerializeField] private Text listeJoueurs;
    [SerializeField] private MenuPrincipalScript menuController;
    [SerializeField] private shipMotor ShipManager;
    [SerializeField] private MapGeneratorBehaviour mapGenerateur;
    [SerializeField] private Button quitterMenuPause;
    [SerializeField] private Button reprendreMenuPause;
    [SerializeField] private Canvas Ui;
    [SerializeField] private Text finDePartie;
    [SerializeField] private Image background;

    [Header("Références")]
    [SerializeField] private OptionsSonScript gestionMenuOptions;

    #region Events
    public event Action OnlinePret;
    public event Action<int> JoueurARejoint;
    public event Action<int> JoueurAQuitte;
    public event Action Deconnecte;
    public event Action MasterclientSwitch;
    public event Action FinDePartie;
    #endregion

    private void Awake()
    {
        menuController.OnlinePret += setInterfaceJeu;
        menuController.ConnectedToMaster += AfficherMenuPrincipal;
        menuController.JoueurAQuitte += AfficherMenuPrincipal;
        menuController.OnError += afficherErreur;
        menuController.OnClicCreer += connexionRoomEnCours;
        menuController.OnClicRejoindre += connexionRoomEnCours;
        menuController.MettreAJourLobby += MettreAJourLobby;
        menuController.FinDePartie += resetInterface;
        menuController.masquerMenuPause += MasquerMenuPause;
        menuController.LancementPartie += PartieLancee;
        menuController.debutGenerationMap += AfficherMessageInterface;
        menuController.finGenerationMap += MasquerMessageInterface;
        ShipManager.AfficherMenuPause += AfficherMenuPause;
        ShipManager.MasquerMenuPause += MasquerMenuPause;
        ShipManager.FinDePartiePourUnJoueur += AfficherEcranFinPartie;
        mapGenerateur.majInterface += AfficherMessageInterface;

        //les boutons sont désactivés au démarrage
        creerRoomButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);
        OptionButton.onClick.AddListener(AfficherMenuOptions);

        //on active le message d'erreur (vide) pour le menu principal
        erreur.gameObject.SetActive(true);
        erreur.text = "Connexion au serveur Photon";
        titre.text = "Royal Star";
    }

    //quand on clique sur une des boutons, le menu principal est masqué et le message de connexion est affiché
    private void connexionRoomEnCours()
    {
        masquerMenuPrincipal();

        erreur.gameObject.SetActive(true);
        erreur.text = "Connexion à la room";
    }

    //dès qu'on est connecté à Photon, on affiche les boutons
    public override void OnConnectedToMaster()
    {
        AfficherMenuPrincipal(0, 0);
        erreur.text = "";
    }

    //quand on rejoint une room, on affiche les éléments correspondants
    public override void OnJoinedRoom()
    {
        //on désactive le message de connexion
        erreur.gameObject.SetActive(false);

        //si le lobby est activé, le masterClient envoie une RPC pour que les clients affichent l'interface du lobby
        if (menuController.waitForPlayersToPlay && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("AfficherLobbyRPC", RpcTarget.AllBuffered);
        }
    }

    //le masterclient lance cette fonction au lancement de la partie pour que tout les clients masquent l'interface de lobby
    public void PartieLancee()
    {
        photonView.RPC("MasquerLobbyRPC", RpcTarget.All);
    }

    //masquer les éléments de l'interface du menu
    public void masquerMenuPrincipal()
    {
        titre.gameObject.SetActive(false);
        creerRoomButton.gameObject.SetActive(false);
        creerRoomButton.interactable = false;
        joinRoomButton.gameObject.SetActive(false);
        joinRoomButton.interactable = false;
        quitterJeuButton.gameObject.SetActive(false);
        quitterJeuButton.interactable = false;
        OptionButton.interactable = false;
        OptionButton.gameObject.SetActive(false);
        erreur.gameObject.SetActive(false);
    }

    //afficher les éléments du menu principal, le paramètre i sert à réutiliser l'event qui nécessite un entier. le paramètre playerActorNumber sert à savoir quel joueur doit afficher le menu
    public void AfficherMenuPrincipal(int i, int playerActorNumber)
    {
        //condition pour que seulement le client qui quitte la room ait l'affichage du menu. dans le cas où playerActorNumber est à 0, c'est que tous les clients doivent afficher le menu
        if (PhotonNetwork.LocalPlayer.ActorNumber != playerActorNumber && playerActorNumber != 0) return;

        //affichage du titre et des boutons
        titre.gameObject.SetActive(true);
        creerRoomButton.gameObject.SetActive(true);
        creerRoomButton.interactable = true;
        joinRoomButton.gameObject.SetActive(true);
        joinRoomButton.interactable = true;
        quitterJeuButton.gameObject.SetActive(true);
        quitterJeuButton.interactable = true;
        OptionButton.interactable = true;
        OptionButton.gameObject.SetActive(true);
        erreur.text = "";
        background.gameObject.SetActive(true);

        //masquer les éléments du lobby
        titreLobby.gameObject.SetActive(false);
        decompteLobby.gameObject.SetActive(false);
        listeJoueurs.gameObject.SetActive(false);

        //curseur de la souris délocké et visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //Masque l'interface du menu
    public void setInterfaceJeu()
    {
        Ui.gameObject.SetActive(false);
    }

    //afficher l'erreur
    public void afficherErreur(string err)
    {
        erreur.text = err;
        erreur.gameObject.SetActive(true);
    }

    //fonction de mise à jour du lobby appelée par l'event du MenuPrincipal
    public void MettreAJourLobby(int dureeRestante)
    {
        photonView.RPC("MettreAJourLobbyRPC", RpcTarget.All, dureeRestante);
    }

    //fonction appelée en fin de partie ou en cas de partie annulée
    public void resetInterface()
    {
        titreLobby.text = "";
        titreLobby.gameObject.SetActive(false);

        decompteLobby.text = "";
        decompteLobby.gameObject.SetActive(false);

        listeJoueurs.text = "";
        listeJoueurs.gameObject.SetActive(false);
    }

    public void ActiverInterface()
    {
        Ui.gameObject.SetActive(true);
    }

    //afficher les boutons du menu pause
    public void AfficherMenuPause()
    {
        quitterMenuPause.gameObject.SetActive(true);
        quitterMenuPause.interactable = true;
        reprendreMenuPause.gameObject.SetActive(true);
        reprendreMenuPause.interactable = true;
        OptionButton.interactable = true;
        OptionButton.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        //curseur de la souris délocké et visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ActiverInterface();
        
    }

    //masquer les boutons du menu pause
    public void MasquerMenuPause()
    {
        quitterMenuPause.gameObject.SetActive(false);
        quitterMenuPause.interactable = false;
        reprendreMenuPause.gameObject.SetActive(false);
        reprendreMenuPause.interactable = false;
        OptionButton.interactable = false;
        OptionButton.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        setInterfaceJeu();
    }

    public void AfficherMenuOptions()
    {
        masquerMenuPrincipal();
        gestionMenuOptions.AfficherMenuOptions();
        OptionButton.interactable = false;
        OptionButton.gameObject.SetActive(false);
    }

    //fonction pour afficher une information sur l'interface des joueurs
    public void AfficherMessageInterface(string msg)
    {
        //seul le masterclient peut envoyer aux clients
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("AfficherMessageInterfaceRPC", RpcTarget.All, msg);
        }
    }

    //fonction pour masquer le message sur l'interface des joueurs
    public void MasquerMessageInterface()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("MasquerMessageInterfaceRPC", RpcTarget.All);
        }
    }

    //fonction du masterclient pour ordonner aux clients d'afficher le résultat de la partie
    public void AfficherEcranFinPartie(int playerActorNumber, bool victoire)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        //récupérer le player défini par playerActorNumber
        int i = 0;
        for(; i < PlayerNumbering.SortedPlayers.Length; i++)
        {
            if(PlayerNumbering.SortedPlayers[i].ActorNumber == playerActorNumber)
            {
                break;
            }
        }

        //envoie de la RPC au client avec son résultat
        photonView.RPC("AfficherEcranFinPartieRPC", PlayerNumbering.SortedPlayers[i], victoire);
    }

    //code exécuté par le client sous ordre du masterclient pour afficher le résultat de fin de partie donné en paramètre
    [PunRPC]
    private void AfficherEcranFinPartieRPC(bool victoire)
    {
        Ui.gameObject.SetActive(true);

        //activation du texte de fin de partie
        finDePartie.gameObject.SetActive(true);

        //en fonction du résultat, le texte prend la valeur adéquate
        if(victoire)
        {
            finDePartie.text = "Victoire !";
            finDePartie.color = Color.green;
        }
        else
        {
            finDePartie.text = "Défaite !";
            finDePartie.color = Color.red;
        }

        //réutilisation du bouton retour menu principal du menu pause pour revenir au menu
        quitterMenuPause.gameObject.SetActive(true);
        quitterMenuPause.interactable = true;

        //curseur de la souris délocké et visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    [PunRPC]
    private void AfficherMessageInterfaceRPC(string msg)
    {
        erreur.gameObject.SetActive(true);
        erreur.text = msg;
    }

    [PunRPC]
    private void MasquerMessageInterfaceRPC()
    {
        erreur.text = "";
        erreur.gameObject.SetActive(false);
    }

    [PunRPC]
    private void MettreAJourLobbyRPC(int dureeRestante)
    {
        titreLobby.text = "En attente d'autres pilotes :" + PhotonNetwork.PlayerList.Length.ToString() + "/20";
        decompteLobby.text = dureeRestante.ToString() + " secondes avant l'entrée dans l'arène";
    }

    [PunRPC]
    private void AfficherLobbyRPC()
    {
        titreLobby.gameObject.SetActive(true);
        listeJoueurs.gameObject.SetActive(true);
        decompteLobby.gameObject.SetActive(true);
    }

    [PunRPC]
    private void MasquerLobbyRPC()
    {
        titreLobby.gameObject.SetActive(false);
        listeJoueurs.gameObject.SetActive(false);
        decompteLobby.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        resetInterface();
    }


}
