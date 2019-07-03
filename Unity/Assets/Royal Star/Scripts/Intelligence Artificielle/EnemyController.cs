using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject target;

    [SerializeField] private float lookRange = 400.0f;
    [SerializeField] private float targetDistance;
    [SerializeField] private float followSpeed = 0.02f;
    private RaycastHit shot;
    
    private void Start()
    {
        target = EnemyManager.instance.AiTargets;
    }

    private void Update()
    {
        transform.LookAt(target.transform);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out shot))
        {
            targetDistance = shot.distance;
            if (targetDistance <= lookRange)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, followSpeed);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRange);
    }
}
