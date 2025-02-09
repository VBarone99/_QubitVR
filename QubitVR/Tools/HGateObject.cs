using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HGateObject : SingleGateObject
{
    /// <summary>
    /// HGate has entered a qubit.
    /// </summary>
    /// <param name="other">HGate collider.</param>
    public void OnTriggerEnter(Collider other)
    {
        if (!gateIsBeingHeld)
            return;

        m_targetQubit = other.transform.parent.GetComponent<Qubit>();
    }

    public void Start()
    {
        m_type = "HGate";
    }
}
