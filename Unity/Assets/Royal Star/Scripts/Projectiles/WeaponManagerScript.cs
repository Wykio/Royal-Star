using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerScript : MonoBehaviour
{
    [SerializeField] private bool automatic;
    [SerializeField] private float popInterval;
    [SerializeField] private BulletPoolManagerScript bulletPoolManager;
    [SerializeField] private Transform[] bulletPopPositions;
    [SerializeField] private float speed;
    [SerializeField] private float weight;
    [SerializeField] private bool raycast;
    [SerializeField] private bool trueRaycast;
    [SerializeField] private float raycastRange;
    [SerializeField] private int raycastDamage;
    [SerializeField] private int BlueRaycastDamage;
    [SerializeField] private MeshRenderer raycastMesh;

    [Header("Référence")]
    [SerializeField] private ShipExposer exposer;
    [SerializeField] private GameObject[] canons;
    [SerializeField] private LineRenderer[] tirRaycast;

    private float nextPopTime = float.MinValue;
    private bool firing = false;

    public float GetWeight()
    {
        return weight;
    }

    private void SetNextPopTime()
    {
        nextPopTime = Time.time + popInterval;
    }

    public bool GetAutomatic()
    {
        return automatic;
    }

    public bool IsRaycast()
    {
        return raycast;
    }

    public void SetFiring(bool isFiring)
    {
        firing = isFiring;

        if(raycastMesh != null)
        {
            raycastMesh.enabled = isFiring;
        }
    }

    public void Shoot(int tireurID)
    {
        if (Time.time > nextPopTime)
        {
            if (raycast)
            {
                ShootWithRaycast(tireurID);
             
            }
            else
            {
                if(trueRaycast)
                {
                    for (int i = 0; i < canons.Length; i++)
                    {
                        if (canons[i].activeSelf)
                        {
                            ShootWithTrueRaycast(tireurID, bulletPopPositions[i], i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < canons.Length; i++)
                    {
                        if (canons[i].activeSelf)
                        {
                            bulletPoolManager.Shoot(bulletPopPositions[i], speed);
                            
                        }
                    }
                }
            }
            SetNextPopTime();
        }
    }

    public void ShootWithRaycast(int tireurID)
    {
        RaycastHit hit;

        foreach(var popPosition in bulletPopPositions)
        {
            if (Physics.Raycast(popPosition.position, popPosition.forward, out hit, raycastRange) )
            {
                if(hit.transform.gameObject.CompareTag("Player"))
                {
                    ShipExposer target = hit.transform.GetComponent<ShipExposer>();

                    if (target != null && target.playerID != tireurID) target.TakeDamage(raycastDamage);
                }
                else
                {
                    if(hit.transform.gameObject.CompareTag("Bot"))
                    {
                        EnemyExposer target = hit.transform.GetComponent<EnemyExposer>();
                        target.TakeDamage(raycastDamage);
                    }
                }
            }
        }
    }

    public void ShootWithTrueRaycast(int tireurID, Transform popPosition, int indice)
    {
        RaycastHit hit;

        tirRaycast[indice].SetPosition(0, popPosition.position);
        tirRaycast[indice].SetPosition(1, popPosition.position + popPosition.forward * raycastRange);

        StartCoroutine(CleanTirRaycast());

        if (Physics.Raycast(popPosition.position, popPosition.forward, out hit, raycastRange))
        {
            if(hit.transform.gameObject.tag == "Player")
            {
                ShipExposer target = hit.transform.GetComponent<ShipExposer>();

                if (target != null && target.playerID != tireurID) target.TakeDamage(BlueRaycastDamage);
            }
            else
            {
                if (hit.transform.gameObject.tag == "Bot")
                {
                    EnemyExposer target = hit.transform.GetComponent<EnemyExposer>();

                    target.TakeDamage(BlueRaycastDamage);
                }
            }
        }
    }

    private IEnumerator CleanTirRaycast()
    {
        yield return new WaitForSeconds(0.1f);

        tirRaycast[0].SetPosition(1, tirRaycast[0].GetPosition(0));
        tirRaycast[1].SetPosition(1, tirRaycast[1].GetPosition(0));
    }

    public float GetNextPopTime()
    {
        return nextPopTime;
    }
}
