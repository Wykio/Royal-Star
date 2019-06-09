using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExposerScript : MonoBehaviour
{
    [SerializeField] private Transform itemTransform;

    private bool ramasse = false;

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
        //si le collider est un joueur
        if(other.gameObject.tag == "Player")
        {

        }
        ramasse = true;
        DesactivationItem();
    }

    //getteur pour avoir l'état de l'item, en place ou ramassé
    public bool getRamasse()
    {
        return ramasse;
    }
}
