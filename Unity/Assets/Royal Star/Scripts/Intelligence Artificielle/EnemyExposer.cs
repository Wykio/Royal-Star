using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExposer : MonoBehaviour
{
    [SerializeField] protected Transform botTransform;
    
    [Header("Stats")]
    [SerializeField] private int healthPoints = 20;
    public bool alive = true;

    [Header("VFX")] 
    [SerializeField] private ParticleSystem hitVFX;
    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        hitVFX.Play();
        //Debug.Log("bot life = " + healthPoints);
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
}
