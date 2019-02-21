using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class shipMotor : MonoBehaviour
{
    //potentiomètres pour bidouiller les stats du vaisseau
    [Header ("Potentiomètres du vaisseau")]
    [SerializeField] private float speed = 90f;
    [SerializeField] private float hoverForce = 65f;
    [SerializeField] private float hoverHeight = 3.5f;
    [SerializeField] private float speedRotate = 100;
    [SerializeField] private float dampingHover = 0.85f;
    [SerializeField] private float dampingSpeed = 0.95f;
    [SerializeField] private float dampingHeight = 4f;

    //composants Photon pour mise en réseau
    [Header("Composant Photon")]
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Transform[] positionsDepart;
    [SerializeField] private AIntentReceiver[] onlineIntentReceivers;
    [SerializeField] private ShipExposer[] vaisseaux;
    [SerializeField] private MenuPrincipalScript gameController;
    private AIntentReceiver[] activatedIntentReceivers;
    private bool GameStarted { get; set; }
    private bool Aerien;

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

    void FixedUpdate()
    {
        //touche ECHAP pour quitter le jeu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinPartie();
            return;
        }

        //si le client n'est pas le masterClient on ne fait rien
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (!GameStarted)
        {
            return;
        }

        var activatedAvatarsCount = 0;

        for (var i = 0; i < activatedIntentReceivers.Length; i++)
        {
            Debug.Log($"{i} indice de joueur");
            
            var intentReceiver = activatedIntentReceivers[i];
            var vaisseau = vaisseaux[i];

            //Nombre de joueurs encore en vie
            activatedAvatarsCount += vaisseau.ShipRootGameObject.activeSelf ? 1 : 0;

            //pour chaque vaisseau connecté, on détecte s'il est au niveau du sol
            DetectionDuSolOnLine(vaisseau);

            //s'il est en contact avec le sol, on applique la fonction de lévitation
            if(!Aerien) Hover(vaisseau);

            activatedAvatarsCount += vaisseau.ShipRootGameObject.activeSelf ? 1 : 0;

            //si le vaisseau est en l'air on gère les intents suivants 
            if(Aerien)
            {
                //si le vaisseau active le boost on gère ces intents
                if(intentReceiver.AirBoostActivate)
                {
                    if(intentReceiver.BoostForward)
                    {
                        vaisseau.ShipRigidBody.AddForce(vaisseau.ShipTransform.forward * (speed * 2f), ForceMode.Force);
                    }
                    if(intentReceiver.BoostBackward)
                    {
                        vaisseau.ShipRigidBody.AddForce(-vaisseau.ShipTransform.forward * (speed * 2f), ForceMode.Force);
                    }
                    if(intentReceiver.AirRollLeft)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, speedRotate * Time.deltaTime);
                    }
                    if(intentReceiver.AirRollRight)
                    {
                        vaisseau.ShipTransform.Rotate(0, 0, -speedRotate * Time.deltaTime);
                    }
                    if(intentReceiver.BoostPicht != 0f)
                    {
                        vaisseau.ShipRigidBody.AddRelativeTorque(0, 0, intentReceiver.BoostTurn * (speed / 2f));
                    }
                    if(intentReceiver.BoostTurn != 0f)
                    {
                        vaisseau.ShipRigidBody.AddRelativeTorque(0, intentReceiver.BoostTurn * (speed / 2f), 0);
                    }
                }
                else
                {
                    //sinon on gère ces intents
                    if(intentReceiver.AirPicthUp)
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
                    vaisseau.ShipRigidBody.AddRelativeTorque(0, intentReceiver.WantToTurn * (speed / 2f), 0);
                }

                moveIntent = moveIntent.normalized;

                vaisseau.ShipRigidBody.AddForce(moveIntent * speed * propulsionAvantAppliquee, ForceMode.Force);
            }
   
        }

    }

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
        GameStarted = true;
    }

    void DetectionDuSolOnLine(ShipExposer vaisseau)
    {
        Aerien = true;

        foreach(Transform point in vaisseau.ShipHoverPoints)
        {
            Ray scan = new Ray(point.position, -point.up);
            RaycastHit hit;

            if(Physics.Raycast(scan, out hit, hoverHeight + 1f))
            {
                Aerien = false;
            }
        }
    }

    //fonction de lévitation
    void Hover(ShipExposer vaisseau)
    {
        Ray scan = new Ray(vaisseau.ShipCentreGravite.position, -vaisseau.ShipCentreGravite.up);
        Ray scanDamping = new Ray(vaisseau.ShipCentreGravite.position, -vaisseau.ShipCentreGravite.up);
        RaycastHit hit;
        RaycastHit hitDamping;
        Vector3 upvector;

        if(Physics.Raycast(scan, out hit, hoverHeight))
        {
            float distance = Vector3.Distance(vaisseau.ShipCentreGravite.position, hit.point);
            
            //vérification si la surface détectée est horizontale ou non, si c'est le cas, on ajoute une compensation au mouvement du vaisseau
            if (Vector3.Magnitude(Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles) > 0)
            {
                propulsionAvantAppliquee = speed * compensation;
                forceLevitationAppliquee = hoverForce * (compensation / 2);
                upvector = Vector3.up;
            }
            else
            {
                //sinon on applique la règle simple
                propulsionAvantAppliquee = speed;
                forceLevitationAppliquee = hoverForce;
                upvector = Vector3.up;
            }

            //plus on est proche du sol, plus la force de léviation est grande
            if (distance < hoverHeight)
            {
                vaisseau.ShipRigidBody.AddForce(vaisseau.ShipTransform.up * forceLevitationAppliquee * (1f - distance / hoverHeight), ForceMode.Force);
                vaisseau.ShipRigidBody.rotation = Quaternion.Slerp(vaisseau.ShipRigidBody.rotation, Quaternion.FromToRotation(transform.up, recupNormaleMoyenne(vaisseau.ShipHoverPoints, hoverHeight + 1f)) * vaisseau.ShipRigidBody.rotation, Time.fixedDeltaTime * 3.75f);
            }
        }

        // effet de damping pour limiter le rebond du vaisseau
        if(Physics.Raycast(scanDamping, out hitDamping, dampingHeight))
        {
            float distance = Vector3.Distance(vaisseau.ShipCentreGravite.position, hitDamping.point);

            // si le vaisseau dépasse la hauteur de lévitation, on active le damping
            if(distance > hoverHeight)
            {
                vaisseau.ShipRigidBody.velocity = new Vector3(vaisseau.ShipRigidBody.velocity.x,
                vaisseau.ShipRigidBody.velocity.y * dampingHover,
                vaisseau.ShipRigidBody.velocity.z);
            }
        }
    }

    //methode pour récupérer la moyenne des normales détectées par les capteurs
    Vector3 recupNormaleMoyenne(Transform[] points, float distance)
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

    private void ActivationVaisseau(int id)
    {
        if(PhotonNetwork.IsConnected)
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
        GameStarted = true;
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
            activatedIntentReceivers[i].AirPicthUp = false;
            activatedIntentReceivers[i].AirPitchDown = false;
            activatedIntentReceivers[i].AirRollRight = false;
            activatedIntentReceivers[i].AirRollLeft = false;
            activatedIntentReceivers[i].BoostBackward = false;
            activatedIntentReceivers[i].BoostForward = false;
            activatedIntentReceivers[i].BoostPicht = 0f;
            activatedIntentReceivers[i].BoostTurn = 0f;
            activatedIntentReceivers[i].WantToGoBackward = false;
            activatedIntentReceivers[i].WantToGoForward = false;
            activatedIntentReceivers[i].WantToStrafeLeft = false;
            activatedIntentReceivers[i].WantToStrafeRight = false;
            activatedIntentReceivers[i].WantToTurn = 0f;
        }
    }

    //désactiver les vaisseaux et les intents
    private void FinPartie()
    {
        GameStarted = false;
        activatedIntentReceivers = null;

        for (var i = 0; i < vaisseaux.Length; i++)
        {
            vaisseaux[i].ShipRootGameObject.SetActive(false);
        }

        DesactiverIntentReceivers();
        ResetGame();
    }
}
