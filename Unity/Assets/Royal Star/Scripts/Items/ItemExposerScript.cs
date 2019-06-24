﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExposerScript : MonoBehaviour
{
    [SerializeField] protected Transform itemTransform;
    [SerializeField] protected bool pose = true;
    [SerializeField] protected bool ramasse = false;

    public void SetPose(bool b)
    {
        pose = b;
    }

    public Transform GetItemTransform()
    {
        return itemTransform;
    }

    public void SetRamasse(bool b)
    {
        ramasse = b;
    }

    //activer le gameobject de l'item
    public void ActivationItem()
    {
        itemTransform.gameObject.SetActive(true);
    }

    //désactiver le gameobject de l'item
    public void DesactivationItem()
    {
        itemTransform.gameObject.SetActive(false);
    }

    //définir la position de l'item
    public void SetPosition(Vector3 position)
    {
        itemTransform.position = position;
    }

    //fonction de collision entre un objet et l'item
    private void OnTriggerEnter(Collider other)
    {
        if (ramasse) return;

        //si le collider est un joueur
        if(other.attachedRigidbody.gameObject.tag == "Player")
        {
            var vaisseau = other.attachedRigidbody.gameObject.GetComponent<ShipExposer>();
            
            if(this.gameObject.tag == "Arme Bleue")
            {
                vaisseau.ActiverArmeBleue();
            }
            else
            {
                if(this.gameObject.tag == "Arme Verte")
                {
                    vaisseau.ActiverArmeVerte();
                }
                else
                {
                    if(this.gameObject.tag == "Arme Rouge")
                    {
                        vaisseau.ActiverArmeRouge();
                    }
                }
            }
        }

        ramasse = true;
        SetPose(true);
        DesactivationItem();
    }

    //getteur pour avoir l'état de l'item, en place ou ramassé
    public bool getRamasse()
    {
        return ramasse;
    }

    public void Update()
    {
        if (pose) return;

        transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        Ray chute = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if(Physics.Raycast(chute, out hit, 3.5f))
        {
            SetPose(true);
        }
    }
}
