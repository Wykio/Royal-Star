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
            Debug.Log($"{player.GetName()} has lost 30hp");
            player.TakeDamage(other.gameObject.GetComponent<BulletExposerScript>().GetDamage());
        }
    }

    void OnDestroy()
    {
        triggerExposer.UnSubscribe();
    }
}
