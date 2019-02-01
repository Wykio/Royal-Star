using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipController : MonoBehaviour
{
    [SerializeField] private Rigidbody shipRigidbody;
    [SerializeField] private Transform shipTransform;
    [SerializeField] private float levitation;
    [SerializeField] private float glide;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(shipTransform.position, shipTransform.TransformDirection(Vector3.down), 1f))
        {
            shipRigidbody.velocity = Vector3.up * levitation;
        }
        else
        {
            shipRigidbody.velocity = Vector3.up * glide;
        }
    }
}
