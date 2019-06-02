using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;

public class InterfaceManager : MonoBehaviourPunCallbacks
{
    [Header("éléments de l'interface")]
    [SerializeField] private Text erreur;
    [SerializeField] private Text titre;
    [SerializeField] private Button creerRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Text titreLobby;
    [SerializeField] private Text decompteLobby;
    [SerializeField] private Text listeJoueurs;
    [SerializeField] private MenuPrincipalScript menuController;
    [SerializeField] private shipMotor ShipManager;
    [SerializeField] private Button quitterMenuPause;
    [SerializeField] private Button reprendreMenuPause;
    [SerializeField] private Canvas Ui;

    #region Events
    public event Action OnlinePret;
    public event Action<int> JoueurARejoint;
    public event Action<int> JoueurAQuitte;
    public event Action Deconnecte;
    public event Action MasterclientSwitch;
    public event Action FinDePartie;
    #endregion

    private List<GameObject> listeElements;

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
        ShipManager.AfficherMenuPause += AfficherMenuPause;
        ShipManager.MasquerMenuPause += MasquerMenuPause;

        listeElements = new List<GameObject>();
        listeElements.Add(erreur.gameObject);
        listeElements.Add(titre.gameObject);
        listeElements.Add(creerRoomButton.gameObject);
        listeElements.Add(joinRoomButton.gameObject);

        //les boutons sont désactivés au démarrage
        creerRoomButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);

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
        erreur.gameObject.SetActive(false);
    }

    //afficher les éléments du menu principal
    public void AfficherMenuPrincipal(int i, int j)
    {
        //affichage du titre et des boutons
        titre.gameObject.SetActive(true);
        creerRoomButton.gameObject.SetActive(true);
        creerRoomButton.interactable = true;
        joinRoomButton.gameObject.SetActive(true);
        joinRoomButton.interactable = true;
        erreur.text = "";

        //masquer les éléments du lobby
        titreLobby.gameObject.SetActive(false);
        decompteLobby.gameObject.SetActive(false);
        listeJoueurs.gameObject.SetActive(false);

        //curseur de la souris délocké et visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //active ou désactive un élément donné en fonction du booleen en paramètre
    public void MasquerActiverElement(GameObject g, bool value)
    {
        foreach (var e in listeElements)
        {
            if (e == g)
            {
                e.SetActive(value);
                break;
            }
        }
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
        resetInterface();
    }

    //afficher les boutons du menu pause
    public void AfficherMenuPause()
    {
        quitterMenuPause.gameObject.SetActive(true);
        quitterMenuPause.interactable = true;
        reprendreMenuPause.gameObject.SetActive(true);
        reprendreMenuPause.interactable = true;

        //curseur de la souris délocké et visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ActiverInterface();
    }

    public void MasquerMenuPause()
    {
        quitterMenuPause.gameObject.SetActive(false);
        quitterMenuPause.interactable = false;
        reprendreMenuPause.gameObject.SetActive(false);
        reprendreMenuPause.interactable = false;

        setInterfaceJeu();
    }

}
