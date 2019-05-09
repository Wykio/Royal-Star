using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManagerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;

    private BulletExposerScript[] alreadyInstanciatedBullets;

    private readonly Queue<BulletExposerScript> availableBullets = new Queue<BulletExposerScript>(100);

    private readonly List<BulletExposerScript> poppedBullets = new List<BulletExposerScript>(100);

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

    public void Shoot(Transform popPosition)
    {
        BulletExposerScript bullet = GetBullet();

        bullet.SetParentReference(
            popPosition.position,
            popPosition.forward,
            popPosition.rotation
        );
        poppedBullets.Add(bullet);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < poppedBullets.Count; i++)
        {
            BulletExposerScript bullet = poppedBullets[i];

			if (bullet.GetDestroy())
            {
                poppedBullets.RemoveAt(i--);
                ReleaseBullet(bullet);
            }
        }
    }
}
