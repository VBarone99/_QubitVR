using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGateObject : SingleGateObject
{
    /// <summary>
    /// SGate has entered a qubit.
    /// </summary>
    /// <param name="other">SGate collider.</param>
    public void OnTriggerEnter(Collider other)
    {
        if (!gateIsBeingHeld)
            return;

        m_targetQubit = other.transform.parent.GetComponent<Qubit>();
    }

    public void Start()
    {
        m_type = "SGate";
    }
}
