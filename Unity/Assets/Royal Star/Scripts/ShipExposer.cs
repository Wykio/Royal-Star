using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShipExposer : MonoBehaviour
{
    public bool Aerien;

    public Rigidbody ShipRigidBody;

    public PhotonRigidbodyView ShipRigidbodyView;

    public Transform ShipTransform;

    public GameObject ShipRootGameObject;

    public Transform[] ShipHoverPoints;

    public Transform ShipCentreGravite;

    public Camera ShipCamera;

    public HitboxExposerScript ShipHitbox;

    public string playerName = "Sami";

    public bool alive = true;

    public int healthPoints = 150;

    public WeaponManagerScript[] ShipWeapons = new WeaponManagerScript[3];

    public int currentWeaponIndex = 0;

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            healthPoints = 0;
            alive = false;
        }
    }
}
