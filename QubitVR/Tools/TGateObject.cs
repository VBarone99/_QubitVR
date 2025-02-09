using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGateObject : SingleGateObject
{
    /// <summary>
    /// TGate has entered a qubit.
    /// </summary>
    /// <param name="other">TGate collider.</param>
    public void OnTriggerEnter(Collider other)
    {
        if (!gateIsBeingHeld)
            return;

        m_targetQubit = other.transform.parent.GetComponent<Qubit>();
    }

    public void Start()
    {
        m_type = "TGate";
    }
}
