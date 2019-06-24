using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorExposerScript : MonoBehaviour
{
    [SerializeField] Renderer renderer;

    public void setRenderer(Material m)
    {
        /*for(int i = 0; i < renderer.materials.Length; i++)
        {
            renderer.materials[i] = m;
        }*/

        renderer.material = m;
    }
}
