using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGateObject : ToolObject
{
    [SerializeField] private Material first_gate_color;
    [SerializeField] private Material second_gate_color;
    [SerializeField] private GameObject hand;
    [SerializeField] private LineRenderer line;
    [SerializeField] private bool enableLineRenderer;

    protected Qubit m_targetQubit, m_controlQubit;
    private int gateNum = 1;
    private bool secondGateHasBeenGrabbed = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public Qubit getControlQubit() { return m_controlQubit; }
    public Qubit getTargetQubit() { return m_targetQubit; }

    protected void _Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        m_doubleGateNumber = "1";
    }

    /// <summary>
    /// Listens for actions on a DoubleGateObject.
    /// </summary>
    void Update()
    {
        // If the control qubit is selected by dropping the first part of the double gate
        if (gateNum == 1 && m_controlQubit != null && !gateIsBeingHeld)
            SwitchToSecondGate();  
        else if (gateNum == 2)
        {
            if (!secondGateHasBeenGrabbed)
            {
                // Gate 2 is grabbed from in front of the user
                if (gateIsBeingHeld)
                    secondGateHasBeenGrabbed = true;
                // Gate 2 has not yet been grabbed, keep updating position to hand
                else
                {
                    // Anchor line renderer and second gate to hand
                    line.SetPosition(0, hand.transform.position);
                    transform.position = hand.transform.position;
                }
            } 
            // Gate 2 is released
            else if (secondGateHasBeenGrabbed) 
            {
                // Gate 2 is released inside a 'targetQubit'
                if (m_targetQubit != null)
                {
                    if (gateIsBeingHeld) // Gate is being held inside a qubit
                    {
                        if (!previewHasBeenTriggered)
                        {
                            m_qubitManager.PreviewDoubleGate(this);
                            previewHasBeenTriggered = true;
                        } 
                    }
                    else
                    {
                        m_qubitManager.ApplyPreviewStateToSystem();
                        cancelGate();
                        previewHasBeenTriggered = false;
                    }
                }
                // Gate 2 is released outside a 'targetQubit', implying the gate should be cancelled
                else if (!gateIsBeingHeld)
                    cancelGate();
            }
        }   
    }

    /// <summary>
    /// General OnTriggerEnter for all DoubleGateObjects.
    /// </summary>
    /// <param name="other">DoubleGateObject collider.</param>
    protected void _OnTriggerEnter(Collider other)
    {
        if (m_controlQubit == null)
        {
            m_controlQubit = other.transform.parent.GetComponent<Qubit>();

            if (m_controlQubit != null)
                m_controlQubit.setQubitColor("Control");
        }
        else if (m_targetQubit == null)
        {
            m_targetQubit = other.transform.parent.GetComponent<Qubit>();

            // Ensure that the target qubit cannot be the same as the control qubit
            if (m_targetQubit == m_controlQubit)
                m_targetQubit = null;
        }
    }

    /// <summary>
    /// DoubleGateObject has exited a qubit.
    /// </summary>
    /// <param name="other">SingleGateObject collider.</param>
    public void OnTriggerExit(Collider other)
    {
        if (!gateIsBeingHeld)
            return;

        // Clear the control qubit if the trigger leaves the qubit via OnTriggerEnter (as opposed to being dropped)
        if (gateNum == 1 && m_controlQubit != null && m_controlQubit == other.transform.parent.GetComponent<Qubit>())
        {
            m_controlQubit.setQubitColor("Default");
            m_controlQubit = null;
        }
            
        // If gate 2 is pulled out
        if (gateNum == 2 && m_targetQubit != null && m_targetQubit == other.transform.parent.GetComponent<Qubit>())
        {
            m_targetQubit = null;
            if (m_qubitManager.previewIsEnabled())
            {
                m_qubitManager.StopAllPreviews();
                previewHasBeenTriggered = false;
            }
        }
    }

    /// <summary>
    /// Cancel the application of the double gate.
    /// </summary>
    private void cancelGate()
    {
        m_qubitManager.setGeneralFlag("DoubleGateCancelled");
        m_targetQubit = null;
        m_controlQubit.setQubitColor("Default");
        m_controlQubit = null;
        secondGateHasBeenGrabbed = false;
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
        SwitchToFirstGate();
    }

    /// <summary>
    /// Switch to the first gate in the double gate.
    /// </summary>
    public void SwitchToFirstGate()
    {
        gateNum = 1;
        m_doubleGateNumber = "1";
        this.GetComponent<MeshRenderer>().material = first_gate_color;
        line.enabled = false;
    }

    /// <summary>
    /// Switch to the second gate in the double gate.
    /// </summary>
    public void SwitchToSecondGate()
    {
        m_qubitManager.setGeneralFlag("FirstLayerApplied");
        gateNum = 2;
        m_doubleGateNumber = "2";
        this.GetComponent<MeshRenderer>().material = second_gate_color;
        line.SetPosition(1, m_controlQubit.transform.position);
        line.enabled = enableLineRenderer;
    }
}