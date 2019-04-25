using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    public GameObject connectedTeleport;
    public float distanceAfterTeleport = 1.0f;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("collision");
            Vector3 endPosition = connectedTeleport.transform.position;
            endPosition.z += distanceAfterTeleport;
            col.gameObject.transform.SetPositionAndRotation(endPosition, connectedTeleport.transform.rotation);
        }
    }
    
}
