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
    [SerializeField] private float ratioDeStrafe = 1f;
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

        if (!Aerien) Hover();

        shipRigidbody.AddForce(0, 0, powerInput * propulsionAvantAppliquee);

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
        }
        else
        {
            //sinon elles le font strafer
            if (Input.GetKey(KeyCode.Q))
            {
                shipRigidbody.AddForce(-(speed / 1.5f) * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            }
            if (Input.GetKey(KeyCode.D))
            {
                shipRigidbody.AddForce((speed / 1.5f) * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            }
        }
    }

    //On vérifie si le vaisseau est en chute libre ou en lévitation
    void DetectionDuSol()
    {
        Aerien = true;

        foreach(Transform point in hoverPoints)
        {
            Ray scan = new Ray(point.position, -Vector3.up);
            RaycastHit hit;

            if(Physics.Raycast(scan, out hit, hoverHeight))
            {
                Aerien = false;
            }
        }
    }

    void Hover()
    {
        Ray scan = new Ray(centreGravite.position, -centreGravite.up);
        RaycastHit hit;
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

            if(distance < hoverHeight)
            {
                shipRigidbody.AddForce(upvector * forceLevitationAppliquee * (1f - distance / hoverHeight));
                shipRigidbody.rotation = Quaternion.Slerp(shipRigidbody.rotation, Quaternion.FromToRotation(transform.up, recupNormaleMoyenne(hoverPoints,hoverHeight + 1f)) * shipRigidbody.rotation, Time.fixedDeltaTime * 3.75f);
            }
        }
    }

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
