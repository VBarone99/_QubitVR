using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightConeListener : MonoBehaviour
{
    [SerializeField] private FlashlightObject m_flashlightObject;

    public void OnTriggerEnter(Collider other)
    {
        m_flashlightObject.ConeOnTriggerEnter(other);
    }
}
