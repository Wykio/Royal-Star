using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerScript : MonoBehaviour
{
    [SerializeField] private bool automatic;
    [SerializeField] private float popInterval;
    [SerializeField] private BulletPoolManagerScript bulletPoolManager;
    [SerializeField] private Transform bulletPopPosition;
    [SerializeField] private float speed;
    [SerializeField] private float weight;
    [SerializeField] private bool raycast;
    [SerializeField] private float raycastRange;
    [SerializeField] private int raycastDamage;
    [SerializeField] private MeshRenderer raycastMesh;
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
        raycastMesh.enabled = isFiring;
    }

    public void Shoot(int tireurID)
    {
        if (Time.time > nextPopTime)
        {
            if (raycast) ShootWithRaycast(tireurID);

            else bulletPoolManager.Shoot(bulletPopPosition, speed);

            SetNextPopTime();
        }
    }

    public void ShootWithRaycast(int tireurID)
    {
        RaycastHit hit;

        if (Physics.Raycast(bulletPopPosition.position, bulletPopPosition.forward, out hit, raycastRange) && hit.transform.gameObject.CompareTag("Player"))
        {
            ShipExposer target = hit.transform.GetComponent<ShipExposer>();

            if (target != null && target.playerID != tireurID) target.TakeDamage(raycastDamage);
        }
    }

    public float GetNextPopTime()
    {
        return nextPopTime;
    }
}
