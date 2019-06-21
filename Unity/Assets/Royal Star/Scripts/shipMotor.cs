 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;

public class shipMotor : MonoBehaviour
{
    //potentiomètres pour bidouiller les stats du vaisseau
    [Header ("Potentiomètres du vaisseau")]
    [SerializeField] private float speed;
    [SerializeField] private float hoverForce;
    [SerializeField] private float hoverHeight;
    [SerializeField] private float speedRotate;
    [SerializeField] private float dampingHover;
    [SerializeField] private float dampingSpeed;
    [SerializeField] private float dampingHeight;
    [SerializeField] private float utilisationBoost;
    [SerializeField] private float rechargeBoost;

    //composants Photon pour mise en réseau
    [Header("Composants Photon")]
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Transform[] positionsDepart;
    [SerializeField] private AIntentReceiver[] onlineIntentReceivers;
    [SerializeField] private ShipExposer[] vaisseaux;
    [SerializeField] private MenuPrincipalScript gameController;
    [SerializeField] private IngameInterfaceManagerScript ingameInterfaceManager;
    private AIntentReceiver[] activatedIntentReceivers;
    private bool gameStarted { get; set; }

    [Header("Attribut pour le client")]
    //booléen pour savoir si le menu est affiché ou non
    private bool menuPauseAffiche = false;

    [Header("Gestion de la Map")]
    [SerializeField] private GestionMapScript gestionnaireMap;
    [SerializeField] private float hauteurMort;

    #region Events
    public event Action AfficherMenuPause;
    public event Action MasquerMenuPause;
    public event Action<int, bool> FinDePartiePourUnJoueur;
    #endregion

    private float propulsionAvantAppliquee;
    private float forceLevitationAppliquee;

    void Awake()
    {
        gameController.OnlinePret += ChooseAndSubscribeToOnlineIntentReceivers;
        gameController.JoueurARejoint += ActivationVaisseau;
        gameController.JoueurAQuitte += DesactivationVaisseau;
        gameController.MasterclientSwitch += FinJeu;
        gameController.FinDePartie += FinPartieRetourMenu;
    }

    //fonction du masterclient pour update l'ensemble des vaisseaux en fonction des inputs envoyés par leurs clients respectifs
    void UpdateGameState()
    {
        var activatedAvatarsCount = 0;

        //boucle pour controller l'ensemble des vaisseaux
        for (var i = 0; i < activatedIntentReceivers.Length; i++)
        {   
            var intentReceiver = activatedIntentReceivers[i];
            var vaisseau = vaisseaux[i];

            //si le vaisseau tombe dans le vide et dépasse la hauteur limite, il meurt
            if(vaisseau.ShipTransform.position.y < hauteurMort)
            {
                vaisseau.TakeDamage(9999);
            }

            //si le vaisseau à 0 PV et encore actif, afficher l'écran de défaite et désactivation du vaisseau
            if (!vaisseau.alive && vaisseau.ShipRootGameObject.activeSelf)
            {
                //affichage de l'écran de défaite par l'interfaceManager via l'event
                FinDePartiePourUnJoueur(vaisseau.playerID, false);

                //désactivation du vaisseau
                DesactivationVaisseau(i, vaisseau.playerID);
            }

            //Nombre de joueurs encore en vie
            activatedAvatarsCount += vaisseau.ShipRootGameObject.activeSelf ? 1 : 0;

            //Calcul du poids des armes actuellement sur le vaisseau
            float weight = 1;

            for (int w = 0; w < vaisseau.ShipWeapons.Length; w++)
            {
                var weaponWeight = vaisseau.ShipWeapons[w]?.GetWeight();
                
                if (weaponWeight != null)
                    weight += (float) weaponWeight;
            }

            float spinWeight = weight / 4;

            //pour chaque vaisseau connecté, on détecte s'il est au niveau du sol
            DetectionDuSolOnLine(vaisseau);

            //s'il veut changer d'arme
            if (intentReceiver.ChangerArme != -1)
            {
                vaisseau.ChangerArme(intentReceiver.ChangerArme);
            }
            Debug.Log($"INDEX ARME: {vaisseau.currentWeaponIndex}");

            if (vaisseau.currentWeaponIndex != intentReceiver.SelectedWeapon) {
                vaisseau.ChangeWeapon(intentReceiver.SelectedWeapon);
            }

            //S'il veut tirer
            if (intentReceiver.WantToShootFirst && vaisseau.ShipWeapons[vaisseau.currentWeaponIndex])
            {
                vaisseau.ShipWeapons[vaisseau.currentWeaponIndex].Shoot();
                vaisseau.ShipWeapons[vaisseau.currentWeaponIndex].SetBulletPoolManagerFiring(true);
                if (!vaisseau.ShipWeapons[vaisseau.currentWeaponIndex].GetAutomatic())
                {
                    intentReceiver.WantToShootFirst = false;
                }
            } else {
                vaisseau.ShipWeapons[vaisseau.currentWeaponIndex].SetBulletPoolManagerFiring(false);
            }

            //si le vaisseau est en l'air on gère les intents suivants 
            if(vaisseau.Aerien)
            {
                if (intentReceiver.BoostPitch != 0f)
                {
                    vaisseau.ShipRigidBody.AddRelativeTorque(intentReceiver.BoostPitch * speedRotate, 0, 0);
                    intentReceiver.BoostPitch = 0f;
                }
                if (intentReceiver.BoostTurn != 0f)
                {
                    vaisseau.ShipRigidBody.AddRelativeTorque(0, intentReceiver.BoostTurn * speedRotate, 0);
                    intentReceiver.BoostTurn = 0f;
                }
                //si le vaisseau active le boost on gère ces intents
                if (intentReceiver.AirBoostActivate && vaisseau.getBoostState())
                {
                    Vector3 applicatedForce = vaisseau.ShipTransform.forward * (speed * 1.5f) / weight;

                    if(intentReceiver.BoostForward || intentReceiver.BoostBackward)
                    {
                        vaisseau.SetNewFieldOfView(90f, vaisseau.playerID);
                        vaisseau.ShipRigidBody.AddForce(
                            intentReceiver.BoostForward ? applicatedForce : -applicatedForce,
                            ForceMode.Force
                        );
                        vaisseau.UtilisationBoost(utilisationBoost);

                        //si la jauge de boost tombe à 0, le boost est désactivé le temps de sa recharge
                        if(vaisseau.getBoost() <= 0.0f)
                        {
                            vaisseau.setBoostState(false);
                        }
                    }
                    if(intentReceiver.AirRollLeft)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, speedRotate * Time.deltaTime / spinWeight);
                    }
                    if(intentReceiver.AirRollRight)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, -speedRotate * Time.deltaTime / spinWeight);
                    }
                }
                else
                {
                    float xAngle = 0;
                    float zAngle = 0;

                    vaisseau.SetNewFieldOfView(64f, vaisseau.playerID);
                    //sinon on gère ces intents
                    if(intentReceiver.AirPitchUp)
                    {
                        xAngle = -speedRotate * Time.deltaTime;
                    }
                    if(intentReceiver.AirPitchDown)
                    {
                        xAngle = speedRotate * Time.deltaTime;
                    }
                    if(intentReceiver.AirRollLeft)
                    {
                        zAngle = speedRotate * Time.deltaTime;
                    }
                    if(intentReceiver.AirRollRight)
                    {
                        zAngle = -speedRotate * Time.deltaTime;
                    }
                    vaisseau.ShipTransform.Rotate(xAngle / spinWeight, 0, zAngle / spinWeight);

                    //recharge du boost
                    vaisseau.RechargeBoost(rechargeBoost);

                    //si le vaisseau a son boost en rechargement et qu'il est au max, il est de nouveau disponible
                    if(!vaisseau.getBoostState() && vaisseau.getBoost()>= 200f)
                    {
                        vaisseau.setBoostState(true);
                    }
                }
            }
            else
            {
                //s'il est en contact avec le sol, on applique la fonction de lévitation
                Hover(vaisseau);

                //et on gère ces intents 
                Vector3 moveIntent = Vector3.zero;
                vaisseau.SetNewFieldOfView(60f, vaisseau.playerID);

                if (intentReceiver.WantToGoForward)
                    moveIntent += vaisseau.ShipTransform.forward;
                if (intentReceiver.WantToGoBackward)
                    moveIntent += -vaisseau.ShipTransform.forward;
                if (intentReceiver.WantToStrafeRight)
                    moveIntent += vaisseau.ShipTransform.right;
                if (intentReceiver.WantToStrafeLeft)
                    moveIntent += -vaisseau.ShipTransform.right;
                if(intentReceiver.WantToTurn != 0f)
                {
                    vaisseau.ShipRigidBody.AddRelativeTorque(0, intentReceiver.WantToTurn * speedRotate / spinWeight, 0);
                    intentReceiver.WantToTurn = 0f;
                }

                vaisseau.ShipRigidBody.AddForce(
                    moveIntent.normalized * propulsionAvantAppliquee / weight,
                    ForceMode.Force
                );

                //recharge du boost
                vaisseau.RechargeBoost(rechargeBoost);

                //si le vaisseau a son boost en rechargement et qu'il est au max, il est de nouveau disponible
                if (!vaisseau.getBoostState() && vaisseau.getBoost() >= 200f)
                {
                    vaisseau.setBoostState(true);
                }
            }

            //application de l'effet de damping sur le vaisseau
            Damping(vaisseau);
        }
        //s'il ne reste qu'un joueur en vie, il gagne la partie
        if (activatedAvatarsCount == 1 && gameController.waitForPlayersToPlay)
        {
            int parcours = 0;

            for(; parcours < vaisseaux.Length; parcours++)
            {
                if(vaisseaux[parcours].ShipRootGameObject.activeSelf)
                {
                    break;
                }
            }

            FinDePartiePourUnJoueur(vaisseaux[parcours].playerID, true);

            DesactivationVaisseau(parcours, vaisseaux[parcours].playerID);
        }
    }

    void FixedUpdate()
    {
        //au niveau du client, si le joueur presse ECHAP, le menu de pause s'active ou se désactive en fonction de son état
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuPauseAffiche)
            {
                AfficherMenuPause.Invoke();
                menuPauseAffiche = true;
            }
            else
            {
                MasquerMenuPause.Invoke();
                menuPauseAffiche = false;
            }
        }

        //si le client n'est pas le masterClient, on ne fait rien
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (!gameStarted)
        {
            return;
        }

        UpdateGameState();
    }

    void DetectionDuSolOnLine(ShipExposer vaisseau)
    {
        vaisseau.Aerien = true;

        foreach (Transform point in vaisseau.ShipHoverPoints)
        {
            Ray scan = new Ray(point.position, -point.up);
            RaycastHit hit;

            if (Physics.Raycast(scan, out hit, dampingHeight))
            {
                vaisseau.Aerien = false;
            }
        }
    }

    //fonction de lévitation
    void Hover(ShipExposer vaisseau)
    {
        //raycast vertical au centre du vaisseau
        Ray scan = new Ray(vaisseau.ShipCentreGravite.position, -vaisseau.ShipCentreGravite.up);
        RaycastHit hit;

        //si le vaisseau est en dessous de la hauteur maximale, on applique une force vers le haut relative à la distance entre le sol et le vaisseau
        if(Physics.Raycast(scan, out hit, hoverHeight))
        {
            float distance = Vector3.Distance(vaisseau.ShipCentreGravite.position, hit.point);
            
            propulsionAvantAppliquee = speed;
            forceLevitationAppliquee = hoverForce;

            //plus on est proche du sol, plus la force de léviation est grande
            vaisseau.ShipRigidBody.AddForce(vaisseau.ShipTransform.up * forceLevitationAppliquee * (1f - distance / hoverHeight), ForceMode.Force);

            //pour que l'inclinaison du vaisseau suive le terrain
            vaisseau.ShipRigidBody.rotation = Quaternion.Slerp(vaisseau.ShipRigidBody.rotation, Quaternion.FromToRotation(vaisseau.ShipTransform.up, RecupNormaleMoyenne(vaisseau.ShipHoverPoints, hoverHeight)) * vaisseau.ShipRigidBody.rotation, Time.fixedDeltaTime * 3.75f);
        }
    }

    //fonction de damping pour limiter les forces sur le vaisseau
    void Damping(ShipExposer vaisseau)
    {
        //si le vaisseau est en l'air, le damping est modéré
        if(vaisseau.Aerien)
        {
            vaisseau.ShipRigidBody.velocity = new Vector3(
                vaisseau.ShipRigidBody.velocity.x * dampingSpeed * 1.04f,
                vaisseau.ShipRigidBody.velocity.y > 0 ? vaisseau.ShipRigidBody.velocity.y * dampingHover : vaisseau.ShipRigidBody.velocity.y,
                vaisseau.ShipRigidBody.velocity.z * dampingSpeed * 1.04f
            );
        }
        else
        {
            //s'il est au sol le damping est plus élevé
            vaisseau.ShipRigidBody.velocity = new Vector3(
                vaisseau.ShipRigidBody.velocity.x * dampingSpeed,
                vaisseau.ShipRigidBody.velocity.y * dampingHover,
                vaisseau.ShipRigidBody.velocity.z * dampingSpeed);
        }
    }

    //methode pour récupérer la moyenne des normales détectées par les capteurs
    Vector3 RecupNormaleMoyenne(Transform[] points, float distance)
    {
        List<Vector3> listeNormales = new List<Vector3>();
        Vector3 normaleMoyenne = Vector3.zero;

        foreach(Transform hoverPoint in points)
        {
            Ray scan = new Ray(hoverPoint.position, -hoverPoint.up);
            RaycastHit hit;

            if(Physics.Raycast(scan, out hit, distance))
            {
                listeNormales.Add(hit.normal);
            }
        }

        foreach(Vector3 normale in listeNormales)
        {
            normaleMoyenne += normale;
        }

        return normaleMoyenne / listeNormales.Count;
    }

    #region fonctions Photon

    private void ResetGame()
    {
        for (var i = 0; i < vaisseaux.Length; i++)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(0, 4000), 200, UnityEngine.Random.Range(0, 4000));
            positionsDepart[i].position = position;

            var vaisseau = vaisseaux[i];
            vaisseau.ShipRigidBody.velocity = Vector3.zero;
            vaisseau.ShipRigidBody.angularVelocity = Vector3.zero;
            vaisseau.ShipTransform.position = positionsDepart[i].position;
            vaisseau.ShipTransform.rotation = positionsDepart[i].rotation;
            vaisseau.ShipRigidbodyView.enabled = activatedIntentReceivers == onlineIntentReceivers;
        }

        ActiverIntentReceivers();
        gameStarted = true;

        gestionnaireMap.SetDebutGame(Time.time);
        StartCoroutine(gestionnaireMap.GestionMap());
    }

    public void UpdateHauteurMort()
    {
        hauteurMort = ((gestionnaireMap.biomeCourant+1) * 5000) -200;
    }

    private void ActivationVaisseau(int id, int playerActorNumber)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("ActivationVaisseauRPC", RpcTarget.AllBuffered, id, playerActorNumber);
        }
        else
        {
            ActivationVaisseauRPC(id, playerActorNumber);
        }
    }
    
    void HitboxTriggerEnter(Collider other, int id)
    {
        if (Equals(other.gameObject.tag, "Bullet"))
        {
            int damage = other.gameObject.GetComponent<BulletExposerScript>().GetDamage();

            vaisseaux[id].TakeDamage(damage);
        }
    }

    public bool getGameStarted()
    {
        return gameStarted;
    }

    [PunRPC]
    private void ActivationVaisseauRPC(int idVaisseau, int playerActorNumber)
    {
        vaisseaux[idVaisseau].ShipRootGameObject.SetActive(true);
        vaisseaux[idVaisseau].ShipCamera.enabled = PhotonNetwork.LocalPlayer.ActorNumber == PlayerNumbering.SortedPlayers[idVaisseau].ActorNumber;
        vaisseaux[idVaisseau].ShipHitbox.Subscribe((Collider other) => HitboxTriggerEnter(other, idVaisseau));
        vaisseaux[idVaisseau].playerID = playerActorNumber;
        ingameInterfaceManager.ActivationUpdateInterfaceToggle(true);
    }

    private void DesactivationVaisseau(int id, int playerActorNumber)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("DesactivationVaisseauRPC", RpcTarget.AllBuffered, id, playerActorNumber);
        }
        else
        {
            DesactivationVaisseauRPC(id, playerActorNumber);
        }
    }

    [PunRPC]
    private void DesactivationVaisseauRPC(int idVaisseau, int playerActorNumber)
    {
        vaisseaux[idVaisseau].ShipRootGameObject.SetActive(false);
    }

    private void ChooseAndSubscribeToOnlineIntentReceivers()
    {
        activatedIntentReceivers = onlineIntentReceivers;
        ResetGame();
    }

    //Desactiver l'ensemble des IntentReceivers de chaque vaisseau de la room
    private void DesactiverIntentReceivers()
    {
        if (activatedIntentReceivers == null)
        {
            return;
        }

        for (var i = 0; i < activatedIntentReceivers.Length; i++)
        {
            activatedIntentReceivers[i].enabled = false;
        }
        for (var i = 0; i < onlineIntentReceivers.Length; i++)
        {
            onlineIntentReceivers[i].enabled = false;
        }
    }
    
    //activer l'ensemble des IntentReceivers de chaque vaisseau de la room
    private void ActiverIntentReceivers()
    {
        if (activatedIntentReceivers == null)
        {
            return;
        }

        for (var i = 0; i < activatedIntentReceivers.Length; i++)
        {
            activatedIntentReceivers[i].enabled = true;
            activatedIntentReceivers[i].AirBoostActivate = false;
            activatedIntentReceivers[i].AirPitchUp = false;
            activatedIntentReceivers[i].AirPitchDown = false;
            activatedIntentReceivers[i].AirRollRight = false;
            activatedIntentReceivers[i].AirRollLeft = false;
            activatedIntentReceivers[i].BoostBackward = false;
            activatedIntentReceivers[i].BoostForward = false;
            activatedIntentReceivers[i].BoostPitch = 0f;
            activatedIntentReceivers[i].BoostTurn = 0f;
            activatedIntentReceivers[i].WantToGoBackward = false;
            activatedIntentReceivers[i].WantToGoForward = false;
            activatedIntentReceivers[i].WantToStrafeLeft = false;
            activatedIntentReceivers[i].WantToStrafeRight = false;
            activatedIntentReceivers[i].WantToTurn = 0f;
            activatedIntentReceivers[i].WantToShootFirst = false;
        }
    }
    
    private void FinPartieRetourMenu()
    {
        gameStarted = false;
        activatedIntentReceivers = null;

        for (var i = 0; i < vaisseaux.Length; i++)
        {
            vaisseaux[i].ShipRootGameObject.SetActive(false);
        }
        DesactiverIntentReceivers();
        PhotonNetwork.LeaveRoom();
    }
    
    //désactiver les vaisseaux et les intents
    //fonction a revoir
    private void FinJeu()
    {
        gameStarted = false;
        activatedIntentReceivers = null;

        for (var i = 0; i < vaisseaux.Length; i++)
        {
            vaisseaux[i].ShipRootGameObject.SetActive(false);
        }

        //gameController.ChargementMenu();
        DesactiverIntentReceivers();

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    #endregion
}
