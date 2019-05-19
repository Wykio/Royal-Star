using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class InterfaceManager : MonoBehaviourPunCallbacks
{
    [Header("éléments de l'interface")]
    [SerializeField] private Text pv;
    [SerializeField] private Text bouclier;
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
    private bool ingame = false;

    private void Awake()
    {
        menuController.OnlinePret += setInterfaceJeu;
        menuController.ConnectedToMaster += AfficherMenuPrincipal;
        menuController.JoueurAQuitte += AfficherMenuPrincipal;
        //menuController.JoueurARejoint += masquerMenuPrincipal;
        menuController.OnError += afficherErreur;
        menuController.OnClicCreer += connexionRoomEnCours;
        menuController.OnClicRejoindre += connexionRoomEnCours;

        listeElements = new List<GameObject>();
        listeElements.Add(pv.gameObject);
        listeElements.Add(bouclier.gameObject);
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

    private void connexionRoomEnCours()
    {
        masquerMenuPrincipal();

        erreur.gameObject.SetActive(true);
        erreur.text = "Connexion à la room";
    }

    public override void OnConnectedToMaster()
    {
        AfficherMenuPrincipal(0);
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
        }
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
    public void AfficherMenuPrincipal(int i)
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
    }

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

    //afficher les éléments de l'interface de gameplay
    public void setInterfaceJeu()
    {
        pv.gameObject.SetActive(true);
        bouclier.gameObject.SetActive(true);
    }

    //afficher l'erreur
    public void afficherErreur(string err)
    {
        erreur.text = err;
        erreur.gameObject.SetActive(true);
    }

}
