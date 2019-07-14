using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject[] targets;

    [SerializeField] private float lookRange = 1000.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float targetDistance;
    private RaycastHit shot;
    
    private void Start()
    {
        targets = EnemyManager.instance.AiTargets;
        //Debug.Log("la taille du tableau vaut" + targets.Length);
    }

    private void Update()
    {
        int idTargetLocked = getClosestTargetId(targets);
        // Debug.Log("Id target :" + idTargetLocked);
        targetDistance = getDistanceBetween(targets[idTargetLocked].transform.position, transform.position);
        if (targetDistance <= lookRange)
        {
            transform.LookAt(targets[idTargetLocked].transform);
            if (targetDistance >= 30)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    targets[idTargetLocked].transform.position, maxSpeed * (targetDistance/lookRange));
            }
        }
    }

    private int getClosestTargetId(GameObject[] myTargets)
    {
        int i = 0;
        float targetDistance = 0.0f;
        float targetDistanceMin = 10000.0f;//besoin d'un nombre tres grand
        int idTargetDistanceMin = 0;


        for (i = 0; i < 20; i++) //20 c'est le nombre de joueur
        {
            if (targets[i].activeSelf == true)
            {
                targetDistance = getDistanceBetween(targets[i].transform.position, transform.position);
                if (targetDistance < targetDistanceMin)
                {
                    targetDistanceMin = targetDistance;
                    idTargetDistanceMin = i;
                }
            }
        }
        return idTargetDistanceMin;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRange);
    }

    private float getDistanceBetween(Vector3 a, Vector3 b)
    {
        float res = (float)Math.Sqrt(Math.Pow(b.x - a.x,2)+Math.Pow(b.y - a.y,2)+Math.Pow(b.z - a.z,2));
        return res;
    }
}
