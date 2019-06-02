using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

//Script pour le menu principal : choix entre créer ou rejoindre une partie
public class MenuPrincipalScript : MonoBehaviourPunCallbacks
{
    #region ClassVariables
    [SerializeField] public bool waitForPlayersToPlay = false;
    [SerializeField] public int DureeMatchmaking = 30;
    #endregion

    #region Interface
    [Header("Boutons Menu Principal")]
    [SerializeField] private Button boutonCreerRoom;
    [SerializeField] private Button boutonRejoindre;

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
    public event Action MasterclientSwitch;
    public event Action FinDePartie;
    public event Action decompteMatchmaking;
    public event Action masquerMenuPause;
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
            OnError.Invoke("Erreur à la connexion à Photon : " + e.ToString());
        }

        //ajout des listeners
        boutonCreerRoom.onClick.AddListener(CreerRoom);
        boutonRejoindre.onClick.AddListener(RejoindreRoom);
        boutonQuitterPartie.onClick.AddListener(QuitterPartie);
        boutonReprendrePartie.onClick.AddListener(ReprendrePartie);
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

    //Quand on clique sur Quitter dans le menu pause
    private void QuitterPartie()
    {
        //indiquer au masterclient que le client va quitter la room
        photonView.RPC("DeconnexionViaClientRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

        //quitter la room
        PhotonNetwork.LeaveRoom();

        //rechargement du jeu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //quand on clique sur reprendre, le menu pause est masqué ainsi que la souris
    private void ReprendrePartie()
    {
        masquerMenuPause.Invoke();

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
        Debug.Log("MenuPrincipal OnDisconnected");
        Deconnecte?.Invoke();
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

        JoueurAQuitte?.Invoke(i, otherPlayer.ActorNumber);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("MenuPrincipal OnMasterClientSwitched");
        MasterclientSwitch?.Invoke();

    }
    
    private IEnumerator GestionLobby()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(3f);
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
            }
            else
            {
                //tous les clients connectés lancent SetPlayerReady
                photonView.RPC("SetPlayerReadyRPC", RpcTarget.All);

                //le masterclient s'occupe d'activer les vaisseaux pour tous les joueurs
                for (int i = 0; i < PlayerNumbering.SortedPlayers.Length; i++)
                {
                    JoueurARejoint?.Invoke(i, PlayerNumbering.SortedPlayers[i].ActorNumber);
                }
                photonView.RPC("MasquerSourisRPC", RpcTarget.All);
            }
        } 
    } 
    
    private IEnumerator SetPlayerReady()
    {
        Debug.Log("MenuPrincipal SetPlayerReady");

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
