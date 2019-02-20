using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManagerScript : MonoBehaviour
{
    [SerializeField]
    private BulletPoolManagerScript bulletPoolManager;

    private readonly List<BulletExposerScript> poppedBullets = new List<BulletExposerScript>(100);

    public void Shoot(Transform popPosition)
    {
        BulletExposerScript bullet = bulletPoolManager.GetBullet();
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
                poppedBullets.RemoveAt(i);
                i--;
                bulletPoolManager.ReleaseBullet(bullet);
            }
        }
    }
}
