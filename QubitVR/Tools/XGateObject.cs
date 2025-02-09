using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XGateObject : SingleGateObject
{
    /// <summary>
    /// XGate has entered a qubit.
    /// </summary>
    /// <param name="other">XGate collider.</param>
    public void OnTriggerEnter(Collider other)
    {
        if (!gateIsBeingHeld)
            return;

        m_targetQubit = other.transform.parent.GetComponent<Qubit>();
    }

    public void Start()
    {
        m_type = "XGate";
    }
}
