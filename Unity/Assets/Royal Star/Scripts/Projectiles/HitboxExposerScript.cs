using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxExposerScript : MonoBehaviour
{
    public delegate void TriggerDelegateType(Collider other);

    private TriggerDelegateType triggerEnterCallback = null;

    public void Subscribe(TriggerDelegateType enterCallback)
    {
        triggerEnterCallback = enterCallback;
    }

    public void UnSubscribe()
    {
        triggerEnterCallback = null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        triggerEnterCallback?.Invoke(other);
    }
}
