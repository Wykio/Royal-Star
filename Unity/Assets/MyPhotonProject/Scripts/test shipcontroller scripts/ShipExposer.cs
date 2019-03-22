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
    public Camera shipCamera;
}
