﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

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

    //composants Photon pour mise en réseau
    [Header("Composants Photon")]
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Transform[] positionsDepart;
    [SerializeField] private AIntentReceiver[] onlineIntentReceivers;
    [SerializeField] private ShipExposer[] vaisseaux;
    [SerializeField] private MenuPrincipalScript gameController;
    private AIntentReceiver[] activatedIntentReceivers;
    private bool gameStarted { get; set; }

    //paramètres avancés
    [SerializeField] private float compensation = 1.5f;


    private float propulsionAvantAppliquee;
    private float forceLevitationAppliquee;

    void Awake()
    {
        gameController.OnlinePret += ChooseAndSubscribeToOnlineIntentReceivers;
        gameController.JoueurARejoint += ActivationVaisseau;
        gameController.JoueurAQuitte += DesactivationVaisseau;
        gameController.Deconnecte += FinPartie;
        gameController.MasterclientSwitch += FinPartie;
    }

    void UpdateGameState()
    {
        //touche ECHAP pour quitter le jeu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinPartie();

            //on remet le curseur visible
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            return;
        }

        var activatedAvatarsCount = 0;

        for (var i = 0; i < activatedIntentReceivers.Length; i++)
        {   
            var intentReceiver = activatedIntentReceivers[i];
            var vaisseau = vaisseaux[i];

            //Debug.Log(intentReceiver.BoostPitch.ToString());

            if (!vaisseau.alive)
            {
                break;
            }

            //Nombre de joueurs encore en vie
            activatedAvatarsCount += vaisseau.ShipRootGameObject.activeSelf ? 1 : 0;

            //pour chaque vaisseau connecté, on détecte s'il est au niveau du sol
            DetectionDuSolOnLine(vaisseau);
            
            //S'il veut tirer
            if(intentReceiver.WantToShootFirst)
            {
                vaisseau.ShipWeapons[vaisseau.currentWeaponIndex]?.Shoot();
                if (!vaisseau.ShipWeapons[vaisseau.currentWeaponIndex].GetAutomatic())
                {
                    intentReceiver.WantToShootFirst = false;
                }
            }

            //s'il est en contact avec le sol, on applique la fonction de lévitation
            if(!vaisseau.Aerien) Hover(vaisseau);

            //si le vaisseau est en l'air on gère les intents suivants 
            if(vaisseau.Aerien)
            {
                //si le vaisseau active le boost on gère ces intents
                if(intentReceiver.AirBoostActivate)
                {
                    if(intentReceiver.BoostForward)
                    {
                        vaisseau.ShipRigidBody.AddForce(vaisseau.ShipTransform.forward * (speed * 1.5f), ForceMode.Force);
                    }
                    if(intentReceiver.BoostBackward)
                    {
                        vaisseau.ShipRigidBody.AddForce(-vaisseau.ShipTransform.forward * (speed * 1.5f), ForceMode.Force);
                    }
                    if(intentReceiver.AirRollLeft)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, speedRotate * Time.deltaTime);
                    }
                    if(intentReceiver.AirRollRight)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, -speedRotate * Time.deltaTime);
                    }
                    if(intentReceiver.BoostPitch != 0f)
                    {
                        vaisseau.ShipRigidBody.AddRelativeTorque(intentReceiver.BoostPitch * speedRotate, 0, 0);
                        intentReceiver.BoostPitch = 0f;
                    }
                    if(intentReceiver.BoostTurn != 0f)
                    {
                        vaisseau.ShipRigidBody.AddRelativeTorque(0, intentReceiver.BoostTurn * speedRotate, 0);
                        intentReceiver.BoostTurn = 0f;
                    }
                }
                else
                {
                    //sinon on gère ces intents
                    if(intentReceiver.AirPitchUp)
                    {
                        vaisseau.ShipTransform.Rotate(-speedRotate * Time.deltaTime, 0, 0);
                    }
                    if(intentReceiver.AirPitchDown)
                    {
                        vaisseau.ShipTransform.Rotate(speedRotate * Time.deltaTime, 0, 0);
                    }
                    if(intentReceiver.AirRollLeft)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, speedRotate * Time.deltaTime);
                    }
                    if(intentReceiver.AirRollRight)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, -speedRotate * Time.deltaTime);
                    }
                }
            }
            else
            {
                //sinon on gère ces intents 
                var moveIntent = Vector3.zero;

                if (intentReceiver.WantToGoForward)
                {
                    moveIntent += vaisseau.ShipTransform.forward;
                }
                if (intentReceiver.WantToGoBackward)
                {
                    moveIntent += -vaisseau.ShipTransform.forward;
                }
                if (intentReceiver.WantToStrafeRight)
                {
                    moveIntent += vaisseau.ShipTransform.right;
                }
                if (intentReceiver.WantToStrafeLeft)
                {
                    moveIntent += -vaisseau.ShipTransform.right;
                }
                if(intentReceiver.WantToTurn != 0f)
                {
                    vaisseau.ShipRigidBody.AddRelativeTorque(0, intentReceiver.WantToTurn * speedRotate, 0);
                    intentReceiver.WantToTurn = 0f;
                }

                moveIntent = moveIntent.normalized;

                vaisseau.ShipRigidBody.AddForce(moveIntent * propulsionAvantAppliquee, ForceMode.Force);
            }

            //application de l'effet de damping sur le vaisseau
            Damping(vaisseau);
        }

        if (activatedAvatarsCount <= 1) {
            gameStarted = false;
            FinPartie();
        }
    }

    void FixedUpdate()
    {
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
        
        Ray scan = new Ray(vaisseau.ShipCentreGravite.position, -vaisseau.ShipCentreGravite.up);
        RaycastHit hit;
 
        if (Physics.Raycast(scan, out hit, dampingHeight))
        {
            vaisseau.Aerien = false;
        } 
    }

    //fonction de lévitation
    void Hover(ShipExposer vaisseau)
    {
        Ray scan = new Ray(vaisseau.ShipCentreGravite.position, -vaisseau.ShipCentreGravite.up);
        RaycastHit hit;

        if(Physics.Raycast(scan, out hit, hoverHeight))
        {
            float distance = Vector3.Distance(vaisseau.ShipCentreGravite.position, hit.point);
            
            //vérification si la surface détectée est horizontale ou non, si c'est le cas, on ajoute une compensation au mouvement du vaisseau
            if (Vector3.Magnitude(Quaternion.FromToRotation(vaisseau.ShipTransform.up, hit.normal).eulerAngles) > 0)
            {
                propulsionAvantAppliquee = speed;
                forceLevitationAppliquee = hoverForce;
            }
            else
            {
                //sinon on applique la règle simple
                propulsionAvantAppliquee = speed;
                forceLevitationAppliquee = hoverForce;
            }

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
                vaisseau.ShipRigidBody.velocity.z * dampingSpeed
            );
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

    void HitboxTriggerEnter(Collider other, int id)
    {
        if (Equals(other.gameObject.tag, "Bullet"))
            {
                int damage = other.gameObject.GetComponent<BulletExposerScript>().GetDamage();

                vaisseaux[id].TakeDamage(damage);
                Debug.Log($"{vaisseaux[id].playerName} has lost {damage}hp");
            }
    }

    #region fonctions Photon

    private void ResetGame()
    {
        for (var i = 0; i < vaisseaux.Length; i++)
        {
            var vaisseau = vaisseaux[i];
            vaisseau.ShipRigidBody.velocity = Vector3.zero;
            vaisseau.ShipRigidBody.angularVelocity = Vector3.zero;
            vaisseau.ShipTransform.position = positionsDepart[i].position;
            vaisseau.ShipTransform.rotation = positionsDepart[i].rotation;
            vaisseau.ShipRigidbodyView.enabled = activatedIntentReceivers == onlineIntentReceivers;
        }

        ActiverIntentReceivers();
        gameStarted = true;
    }

    private void ActivationVaisseau(int id)
    {
        //on bloque et cache le curseur de la souris
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("ActivationVaisseauRPC", RpcTarget.AllBuffered, id);
        }
        else
        {
            ActivationVaisseauRPC(id);
        }
    }

    [PunRPC]
    private void ActivationVaisseauRPC(int idVaisseau)
    {
        vaisseaux[idVaisseau].ShipRootGameObject.SetActive(true);
        vaisseaux[idVaisseau].ShipCamera.enabled = PhotonNetwork.LocalPlayer.ActorNumber == PlayerNumbering.SortedPlayers[idVaisseau].ActorNumber;
        vaisseaux[idVaisseau].ShipHitbox.Subscribe((Collider other) => HitboxTriggerEnter(other, idVaisseau));
    }

    private void DesactivationVaisseau(int id)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("DeactivativationVaisseauRPC", RpcTarget.AllBuffered, id);
        }
        else
        {
            DesactivationVaisseauRPC(id);
        }
    }

    [PunRPC]
    private void DesactivationVaisseauRPC(int idVaisseau)
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

    //désactiver les vaisseaux et les intents
    private void FinPartie()
    {
        gameStarted = false;
        activatedIntentReceivers = null;

        for (var i = 0; i < vaisseaux.Length; i++)
        {
            vaisseaux[i].ShipRootGameObject.SetActive(false);
        }
        
        DesactiverIntentReceivers();

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        
        gameController.AfficherMenu();
    }

    #endregion
}
