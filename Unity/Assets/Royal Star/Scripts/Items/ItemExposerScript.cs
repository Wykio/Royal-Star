using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExposerScript : MonoBehaviour
{
    [SerializeField] private Transform itemTransform;

    private bool ramasse = false;

    public void ActivationItem()
    {
        itemTransform.gameObject.SetActive(true);
    }

    public void DesactivationItem()
    {
        itemTransform.gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 position)
    {
        itemTransform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        ramasse = true;
        DesactivationItem();
    }

    public bool getRamasse()
    {
        return ramasse;
    }
}
