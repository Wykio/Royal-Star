using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipMotor : MonoBehaviour
{
    //potentiomètres pour bidouiller les stats du vaisseau
    
    [SerializeField] private float speed = 90f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float hoverForce = 65f;
    [SerializeField] private float hoverHeight = 3.5f;
    [SerializeField] private float speedRotate = 100;
    [SerializeField] private float damping = 0.95f;
    [SerializeField] private float dampingHeight = 4f;

    //composants du vaisseau
    [SerializeField] private Transform[] hoverPoints;
    [SerializeField] private Transform centreGravite;
    private Transform shipTransform;
    private Rigidbody shipRigidbody;

    //variables pour les inputs
    private float powerInput;
    private float lateralInput;
    private float turnInput;
    private float sourisHorizontale;
    private float sourisVerticale;
    private bool Aerien = true;

    //paramètres avancés
    [SerializeField] private float compensation = 1.5f;


    private float propulsionAvantAppliquee;
    private float forceLevitationAppliquee;

    void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody>();
        shipTransform = GetComponent<Transform>();
    }

    void Update()
    {
        powerInput = Input.GetAxis("Vertical");
        sourisHorizontale = -Input.GetAxis("Mouse Y");
        sourisVerticale = Input.GetAxis("Mouse X");  
    }

    void FixedUpdate()
    {
        DetectionDuSol();

        Debug.Log("Aerien = " + Aerien);

        if (!Aerien)
        {
            Hover();
            
        }

        //si le vaisseau est en l'air, les touches latérales le font rouler
        if(Aerien)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                shipTransform.Rotate(0, 0, -speedRotate * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                shipTransform.Rotate(0, 0, speedRotate * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Z))
            {
                shipTransform.Rotate(speedRotate * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                shipTransform.Rotate(-speedRotate * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            shipRigidbody.AddForce(shipTransform.forward * powerInput * propulsionAvantAppliquee, ForceMode.Force);

            //sinon elles le font strafer
            if (Input.GetKey(KeyCode.Q))
            {
                shipRigidbody.AddForce(-(speed) * Time.deltaTime, 0, 0, ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.D))
            {
                shipRigidbody.AddForce((speed) * Time.deltaTime, 0, 0, ForceMode.Impulse);
            }
        }
        
    }

    //On vérifie si le vaisseau est en chute libre ou en lévitation
    void DetectionDuSol()
    {
        Aerien = true;

        foreach(Transform point in hoverPoints)
        {
            Ray scan = new Ray(point.position, -point.up);
            
            RaycastHit hit;

            if(Physics.Raycast(scan, out hit, hoverHeight + 0.5f))
            {
                Debug.Log("collision trouvée : " + hit.collider.name);
                Aerien = false;
            }
        }
    }

    //fonction de lévitation
    void Hover()
    {
        Ray scan = new Ray(centreGravite.position, -centreGravite.up);
        Ray scanDamping = new Ray(centreGravite.position, -centreGravite.up);
        RaycastHit hit;
        RaycastHit hitDamping;
        Vector3 upvector;

        if(Physics.Raycast(scan, out hit, hoverHeight))
        {
            float distance = Vector3.Distance(centreGravite.position, hit.point);

            //vérification si la surface détectée est horizontale ou non, si c'est le cas, on ajoute une compensation au mouvement du vaisseau
            if(Vector3.Magnitude(Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles) > 0)
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
                shipRigidbody.AddForce(upvector * forceLevitationAppliquee * (1f - distance / hoverHeight), ForceMode.Force);
                shipRigidbody.rotation = Quaternion.Slerp(shipRigidbody.rotation, Quaternion.FromToRotation(transform.up, recupNormaleMoyenne(hoverPoints, hoverHeight + 1f)) * shipRigidbody.rotation, Time.fixedDeltaTime * 3.75f);
            }
        }

        // effet de damping pour limiter le rebond du vaisseau
        if(Physics.Raycast(scanDamping, out hitDamping, dampingHeight))
        {
            float distance = Vector3.Distance(centreGravite.position, hitDamping.point);

            // si le vaisseau dépasse la hauteur de lévitation, on active le damping
            if(distance > hoverHeight)
            {
                shipRigidbody.velocity = new Vector3(shipRigidbody.velocity.x,
                shipRigidbody.velocity.y * damping,
                shipRigidbody.velocity.z);
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

    
}
