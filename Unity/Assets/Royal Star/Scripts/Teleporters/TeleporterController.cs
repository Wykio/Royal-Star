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
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
            
        if (other.gameObject.tag == "Player")
        {     
            //Debug.Log("collision");
            Vector3 endPosition = connectedTeleport.transform.position;
            endPosition.z += distanceAfterTeleport;
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
