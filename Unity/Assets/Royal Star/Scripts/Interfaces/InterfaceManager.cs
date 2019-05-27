using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class InterfaceManager : MonoBehaviourPunCallbacks
{
    [Header("éléments de l'interface")]
    [SerializeField] private Image background;
    [SerializeField] private Text erreur;
    [SerializeField] private Text titre;
    [SerializeField] private Text compteurJoueursLobby;
    [SerializeField] private Button creerRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private MenuPrincipalScript menuController;

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
        menuController.nouveauJoueurDansRoom += MiseAJourCompteurJoueur;

        listeElements = new List<GameObject>();
        listeElements.Add(erreur.gameObject);
        listeElements.Add(titre.gameObject);
        listeElements.Add(creerRoomButton.gameObject);
        listeElements.Add(joinRoomButton.gameObject);

        //les boutons sont désactivés au démarrage
        creerRoomButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);

        //on active le message d'erreur (vide) pour le menu principal ainsi que le background
        background.gameObject.SetActive(true);
        erreur.gameObject.SetActive(true);
        erreur.text = "Connexion au serveur Photon";
        titre.text = "Royal Star";
    }

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

        //si le lobby est activé, on affiche les éléments
        if (menuController.waitForPlayersToPlay)
        {
            compteurJoueursLobby.gameObject.SetActive(true);

            if(PhotonNetwork.PlayerList.Length == 1) compteurJoueursLobby.text = PhotonNetwork.PlayerList.Length.ToString() + " joueur";
            else compteurJoueursLobby.text = PhotonNetwork.PlayerList.Length.ToString() + " joueurs";
        }
    }

    public void MiseAJourCompteurJoueur()
    {
        compteurJoueursLobby.text = PhotonNetwork.PlayerList.Length.ToString() + " joueurs";
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

        //curseur de la souris délocké et visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //masquer le compteur de joueurs
        compteurJoueursLobby.text = "";
        compteurJoueursLobby.gameObject.SetActive(false);
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
        erreur.gameObject.GetComponentInParent(typeof(Canvas)).gameObject.SetActive(false);
    }

    //afficher l'erreur
    public void afficherErreur(string err)
    {
        erreur.text = err;
        erreur.gameObject.SetActive(true);
    }

}
