using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightObject : ToolObject
{
    protected Qubit m_targetQubit;

    public Qubit getTargetQubit() { return m_targetQubit; }

    private bool flashlightBeamOverride = false;

    public void Start()
    {
        m_type = "Flashlight";
    }

    // Update is called once per frame
    public void Update()
    {
        // If the gate is inside a qubit
        if (m_targetQubit != null && (!gateIsBeingHeld || flashlightBeamOverride))
        {
            m_qubitManager.ApplyFlashlight(this);
            m_targetQubit = null;
            flashlightBeamOverride = false;
        }
    }

    /// <summary>
    /// FlashlightObject has entered qubit.
    /// </summary>
    /// <param name="other">FlashlightObject collider.</param>
    public void OnTriggerEnter(Collider other)
    {
        m_targetQubit = other.transform.parent.GetComponent<Qubit>();
    }

    public void ConeOnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Qubit>() != null)
            m_targetQubit = other.transform.parent.GetComponent<Qubit>();
   
        flashlightBeamOverride = true;
    }

    /// <summary>
    /// FlashlightObject has exited qubit.
    /// </summary>
    /// <param name="other">FlashlightObject collider.</param>
    public void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.GetComponent<Qubit>() == m_targetQubit)
            m_targetQubit = null;
    }
}