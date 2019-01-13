using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipMotor : MonoBehaviour
{
    [SerializeField] private float speed = 90f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float hoverForce = 65f;
    [SerializeField] private float hoverHeight = 3.5f;
    [SerializeField] private Transform shipTransform;

    [SerializeField] private float frontZ;
    [SerializeField] private float frontY;
    [SerializeField] private float frontHoverForce;

    private float powerInput;
    private float turnInput;
    private Rigidbody shipRigidbody;


    void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody>();

    }

    void Update()
    {
        powerInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Ray rayCentre = new Ray(transform.position, -transform.up);
        Ray rayFront = new Ray(transform.position, new Vector3(0f, frontY, frontZ));

        RaycastHit hit;

        //detection vers le bas
        if (Physics.Raycast(rayCentre, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            shipRigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        //detection à l'avant
        if(Physics.Raycast(rayFront, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = new Vector3(0f, -frontY, -frontZ) * proportionalHeight * frontHoverForce;
            shipRigidbody.AddRelativeTorque(shipTransform.right, ForceMode.VelocityChange);
        }

        shipRigidbody.AddRelativeForce(0f, 0f, powerInput * speed);
        shipRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);

    }
}
