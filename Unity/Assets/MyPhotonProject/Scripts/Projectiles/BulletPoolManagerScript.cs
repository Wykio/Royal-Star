using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManagerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;

    private BulletExposerScript[] alreadyInstanciatedBullets;
    private readonly Queue<BulletExposerScript> availableBullets = new Queue<BulletExposerScript>(100);

    public void Awake()
    {
        GameObject instanciatedBullet;

        alreadyInstanciatedBullets = new BulletExposerScript[100];
        for (int i = 0; i < 100; i++)
        {
            instanciatedBullet = (GameObject)Instantiate(bulletPrefab);
            alreadyInstanciatedBullets[i] = instanciatedBullet.GetComponent<BulletExposerScript>();
        }
        foreach (BulletExposerScript bullet in alreadyInstanciatedBullets)
        {
            availableBullets.Enqueue(bullet);
        }
    }

    public BulletExposerScript GetBullet()
    {
        BulletExposerScript bullet = availableBullets.Dequeue();
        bullet.Enable();
        return bullet;
    }

    public void ReleaseBullet(BulletExposerScript bullet)
    {
        availableBullets.Enqueue(bullet);
        bullet.Disable();
    }
}
