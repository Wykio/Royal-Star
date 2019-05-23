using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManagerScript : MonoBehaviour
{
    [SerializeField]
    private PlayerManagerScript player;

    [SerializeField]
    private HitboxExposerScript triggerExposer;

    void Start()
    {
        triggerExposer.Subscribe(MyOnTriggerEnter);
    }

    void MyOnTriggerEnter(Collider other)
    {
        if (Equals(other.gameObject.tag, "Bullet"))
        {
            int damage = other.gameObject.GetComponent<BulletExposerScript>().GetDamage();

            //Debug.Log($"{player.GetName()} has lost {damage}hp");
            player.TakeDamage(damage);
        }
    }

    void OnDestroy()
    {
        triggerExposer.UnSubscribe();
    }
}
