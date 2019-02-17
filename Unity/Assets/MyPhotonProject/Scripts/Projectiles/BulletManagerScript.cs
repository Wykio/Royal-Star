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
    private float popInterval = 1f;

    private readonly List<BulletExposerScript> poppedBullets = new List<BulletExposerScript>(100);

    private float nextPopTime = float.MinValue;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextPopTime)
        {
            BulletExposerScript bullet = bulletPoolManager.GetBall();
            bullet.targetTransform.position = bulletPopPositionTransform.position;
            poppedBullets.Add(bullet);
            nextPopTime = Time.time + popInterval;
        }

        for (int i = 0; i < poppedBullets.Count; i++)
        {
            BulletExposerScript bullet = poppedBullets[i];
            if (bullet.targetTransform.position.x > 50)
            {
                poppedBullets.RemoveAt(i);
                i--;
                bulletPoolManager.ReleaseBall(bullet);
            }
        }
    }
}
