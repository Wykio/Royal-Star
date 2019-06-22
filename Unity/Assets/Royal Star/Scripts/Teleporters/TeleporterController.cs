using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    public GameObject connectedTeleport;
    public float distanceAfterTeleport = 1.0f;

    void OnCollisionEnter(Collision other)
    {
        //si le portail n'a pas de destination, comme les portails d'arrivée, on se fait rien à la collision
        if (connectedTeleport == null) return;

        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
            
        if (other.gameObject.tag == "Player")
        {     
            Vector3 endPosition = connectedTeleport.transform.position;
            endPosition.z += distanceAfterTeleport;
            endPosition.y -= 10;
            other.gameObject.transform.SetPositionAndRotation(endPosition, connectedTeleport.transform.rotation);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;
    }
}
