using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyExposer : MonoBehaviour
{
    [SerializeField] public GameObject rootGameObject; 
    [SerializeField] protected Transform botTransform;
    [SerializeField] HitboxExposerScript hitbox;
    [SerializeField] private PhotonView photonView;
    
    [Header("Stats")]
    [SerializeField] private int healthPoints = 200;
    public bool alive = true;

    [Header("VFX")] 
    [SerializeField] private ParticleSystem hitVFX;

    private void Start()
    {
        hitbox.Subscribe(HitboxTriggerEnter);
    }

    public void TakeDamage(int damage)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            healthPoints -= damage;
            hitVFX.Play();

            Debug.Log("PV : " + healthPoints);

            if (healthPoints <= 0)
            {
                healthPoints = 0;
                alive = false;

                photonView.RPC("DesactivationBotRPC", RpcTarget.All);
            }
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

    [PunRPC]
    private void DesactivationBotRPC()
    {
        DesactivationBot();
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
