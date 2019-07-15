using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header ("Référence")]
    [SerializeField] private WeaponManagerScript weaponManagerScript;
    
    [Header ("Déplacement")]
    private GameObject[] targets;
    [SerializeField] private float lookRange = 1000.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float targetDistance;
    
    private void Start()
    {
        targets = EnemyManager.instance.AiTargets;
    }

    private void Update()
    {
        int idTargetLocked = getClosestTargetId(targets);

        targetDistance = getDistanceBetween(targets[idTargetLocked].transform.position, transform.position);

        if (targetDistance <= lookRange)
        {
            transform.LookAt(targets[idTargetLocked].transform);

            if (targetDistance >= 70)
            {
                Vector3 cible = targets[idTargetLocked].transform.position;

                //imprecision du déplacement pour éviter la visée parfaite
                switch(UnityEngine.Random.Range(0,2))
                {
                    case 0:
                        cible.x += UnityEngine.Random.Range(-100, 100);
                        break;
                    case 1:
                        cible.y += UnityEngine.Random.Range(-100, 100);
                        break;
                    case 2:
                        cible.z += UnityEngine.Random.Range(-100, 100);
                        break;
                }

                transform.position = Vector3.MoveTowards(transform.position, cible, maxSpeed * (targetDistance/lookRange));

                if (targetDistance <= 200)
                {
                    int probaTir = UnityEngine.Random.Range(0, 3);

                    //if(probaTir >= 1) weaponManagerScript.Shoot(0);
                }
            }
        }
    }

    private int getClosestTargetId(GameObject[] myTargets)
    {
        int i = 0;
        float targetDistance = 0.0f;
        float targetDistanceMin = 10000.0f;     //besoin d'un nombre tres grand
        int idTargetDistanceMin = 0;


        for (i = 0; i < 20; i++)    //20 c'est le nombre de joueur
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

    private float getDistanceBetween(Vector3 a, Vector3 b)
    {
        float res = (float)Math.Sqrt(Math.Pow(b.x - a.x,2)+Math.Pow(b.y - a.y,2)+Math.Pow(b.z - a.z,2));
        return res;
    }
}
