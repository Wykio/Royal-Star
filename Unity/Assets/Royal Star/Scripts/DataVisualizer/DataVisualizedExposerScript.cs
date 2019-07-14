using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataVisualizedExposerScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshCube;
    [SerializeField] private MeshRenderer meshTir;

    public MeshRenderer GetMeshCube()
    {
        return meshCube;
    }

    public MeshRenderer getMeshTir()
    {
        return meshTir;
    }

    public void setMaterial(Material m)
    {
        meshCube.material = m;
        meshTir.material = m;
    }
}
