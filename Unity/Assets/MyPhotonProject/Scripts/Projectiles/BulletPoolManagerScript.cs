using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManagerScript : MonoBehaviour
{
    [SerializeField]
    private BulletExposerScript[] alreadyInstanciatedBullets;

    private readonly Queue<BulletExposerScript> availableBullets = new Queue<BulletExposerScript>(100);

    public void Awake()
    {
        foreach (BulletExposerScript bullet in alreadyInstanciatedBullets)
        {
            availableBullets.Enqueue(bullet);
        }
    }

    public BulletExposerScript GetBall()
    {
        BulletExposerScript bullet = availableBullets.Dequeue();
        bullet.targetRigidBody.velocity = new Vector3(10, 0, 0);
        bullet.targetRigidBody.angularVelocity = Vector3.zero;
        bullet.targetCollider.enabled = true;
        bullet.targetRigidBody.isKinematic = false;
        bullet.targetMeshRenderer.enabled = true;
        return bullet;
    }

    public void ReleaseBall(BulletExposerScript bullet)
    {
        availableBullets.Enqueue(bullet);
        bullet.targetCollider.enabled = false;
        bullet.targetRigidBody.isKinematic = true;
        bullet.targetMeshRenderer.enabled = false;
    }
}
