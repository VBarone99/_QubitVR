using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGateObject : ToolObject
{
    protected Qubit m_targetQubit;

    public Qubit getTargetQubit() { return m_targetQubit; }

    /// <summary>
    /// Listens for actions on a SingleGateObject.
    /// </summary>
    public void Update()
    {
        if (m_targetQubit != null) // If the gate is inside a qubit
        {
            if (gateIsBeingHeld) // Gate is being held inside a qubit
            {
                if (!previewHasBeenTriggered)
                {
                    m_qubitManager.PreviewSingleGate(this);
                    previewHasBeenTriggered = true;
                }
            }
            else // Gate is released inside a qubit
            {
                m_qubitManager.ApplyPreviewStateToSystem();
                m_targetQubit = null;
                previewHasBeenTriggered = false;
            }
        }
    }

    /// <summary>
    /// SingleGateObject has exited a qubit.
    /// </summary>
    /// <param name="other">SingleGateObject collider.</param>
    public void OnTriggerExit(Collider other)
    {
        if (!gateIsBeingHeld)
            return;

        if (m_targetQubit != null && other.transform.parent.GetComponent<Qubit>() == m_targetQubit)
        {
            m_targetQubit = null;

            // Only execute code to handle previews when previewIsEnabled
            if (m_qubitManager.previewIsEnabled())
            {
                m_qubitManager.StopAllPreviews();
                previewHasBeenTriggered = false;
            }
        }
    }
}