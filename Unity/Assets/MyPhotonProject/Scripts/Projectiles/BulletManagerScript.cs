using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManagerScript : MonoBehaviour
{
    [SerializeField]
    private Transform bulletPopPositionTransform;

    [SerializeField]
    private BulletPoolManagerScript bulletPoolManager;

    [SerializeField]
    private float popInterval;

    private readonly List<BulletExposerScript> poppedBullets = new List<BulletExposerScript>(100);

    private float nextPopTime = float.MinValue;

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextPopTime)
        {
            BulletExposerScript bullet = bulletPoolManager.GetBullet();
            bullet.SetParentReference(
                bulletPopPositionTransform.position,
                bulletPopPositionTransform.forward * 58,
                bulletPopPositionTransform.rotation
            );
            poppedBullets.Add(bullet);
            nextPopTime = Time.time + popInterval;
        }

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
