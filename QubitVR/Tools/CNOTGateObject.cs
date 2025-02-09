using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNOTGateObject : DoubleGateObject
{
    /// <summary>
    /// CNOTGate has entered a qubit.
    /// </summary>
    /// <param name="other">CNOTGate collider.</param>
    public void OnTriggerEnter(Collider other)
    {
        if (!gateIsBeingHeld)
            return;

        _OnTriggerEnter(other);
    }

    public void Start()
    {
        m_type = "CNOTGate";
        _Start();
    }
}
