using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerScript : MonoBehaviour
{
    [SerializeField]
    private bool automatic;
    
    [SerializeField]
    private float popInterval;

    [SerializeField]
    private BulletPoolManagerScript bulletPoolManager;

    [SerializeField]
    private Transform bulletPopPosition;

    [SerializeField]
    private float weaponWeight;

    private float nextPopTime = float.MinValue;

    private void setNextPopTime()
    {
        nextPopTime = Time.time + popInterval;
    }

    public bool GetAutomatic()
    {
        return automatic;
    }

    public float GetWeight()
    {
        return weaponWeight;
    }

    public void Shoot()
    {
        if (Time.time > nextPopTime)
        {
            bulletPoolManager.Shoot(bulletPopPosition);
            setNextPopTime();
        }
    }
}
