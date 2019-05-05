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

    private float nextPopTime = float.MinValue;

    private void setNextPopTime()
    {
        nextPopTime = Time.time + popInterval;
    }

    public bool GetAutomatic()
    {
        return automatic;
    }

    public void Shoot()
    {
        Debug.Log($"{nextPopTime} ${Time.time}");
        if (Time.time > nextPopTime)
        {
            Debug.Log("SHOOOOOOOOT");
            bulletPoolManager.Shoot(bulletPopPosition);
            setNextPopTime();
        }
    }
}
