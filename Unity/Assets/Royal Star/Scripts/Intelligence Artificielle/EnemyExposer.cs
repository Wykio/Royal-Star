using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExposer : MonoBehaviour
{
    [SerializeField] protected Transform botTransform;
    [SerializeField] HitboxExposerScript hitbox;
    
    [Header("Stats")]
    [SerializeField] private int healthPoints = 20;
    public bool alive = true;

    [Header("VFX")] 
    [SerializeField] private ParticleSystem hitVFX;

    private void Start()
    {
        hitbox.Subscribe(HitboxTriggerEnter);
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        hitVFX.Play();

        if (healthPoints <= 0)
        {
            healthPoints = 0;
            alive = false;
            DesactivationBot();
        }
    }

    public Transform GetBotTransform()
    {
        return botTransform;
    }

    //activer le gameobject de l'item
    public void ActivationBot()
    {
        botTransform.gameObject.SetActive(true);
    }

    //désactiver le gameobject de l'item
    public void DesactivationBot()
    {
        botTransform.gameObject.SetActive(false);
    }

    //définir la position de l'item
    public void SetPosition(Vector3 position)
    {
        botTransform.position = position;
    }

    void HitboxTriggerEnter(Collider other)
    {
        if (Equals(other.gameObject.tag, "Bullet") || Equals(other.gameObject.tag, "Tir"))
        {
            int damage = other.gameObject.GetComponent<BulletExposerScript>().GetDamage();

            TakeDamage(damage);
        }
    }
}
