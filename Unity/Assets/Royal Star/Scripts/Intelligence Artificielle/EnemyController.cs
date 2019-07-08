using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject target;

    [SerializeField] private float lookRange = 1000.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float targetDistance;
    [SerializeField] private float followSpeed;
    private RaycastHit shot;
    
    private void Start()
    {
        target = EnemyManager.instance.AiTargets;
    }

    private void Update()
    {
        targetDistance = getDistanceBetween(target.transform.position, transform.position);
        if (targetDistance <= lookRange)
        {
            transform.LookAt(target.transform);
            if (targetDistance >= 20)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    target.transform.position, maxSpeed * (targetDistance/lookRange));
            }
        }
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
