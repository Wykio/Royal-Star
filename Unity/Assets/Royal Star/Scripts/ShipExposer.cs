using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShipExposer : MonoBehaviour
{
    public Rigidbody ShipRigidBody;

    public PhotonRigidbodyView ShipRigidbodyView;

    public Transform ShipTransform;

    public GameObject ShipRootGameObject;

    public Transform[] ShipHoverPoints;

    public Transform ShipCentreGravite;

    public Camera ShipCamera;

    public int healthPoints = 150;

    public WeaponManagerScript[] ShipWeapons = new WeaponManagerScript[3];

    public int currentWeaponIndex = 0;

}
