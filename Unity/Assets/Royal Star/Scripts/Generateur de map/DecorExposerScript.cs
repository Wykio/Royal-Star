using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorExposerScript : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    public void setMeshRenderer(Material m)
    {
        for(int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
        {
            meshRenderer.sharedMaterials[i] = m;
        }
    }
}
