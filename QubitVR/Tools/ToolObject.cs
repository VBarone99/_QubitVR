using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolObject : MonoBehaviour
{
    protected QubitManager m_qubitManager;
    protected bool gateIsBeingHeld = false;
    protected bool previewHasBeenTriggered = false;
    protected string m_type;
    protected string m_doubleGateNumber = "";

    public void setQubitManager(QubitManager qubitManager) { m_qubitManager = qubitManager; }
    public void setGateIsBeingHeld(bool val) { gateIsBeingHeld = val; }

    public string getType() { return m_type; }

    /// <summary>
    /// Set the gate Active/Inactive depending on 'val'. Only the renderer and the collider are disabled to prevent other scripts from breaking.
    /// </summary>
    /// <param name="val"></param>
    public void setGateActive(bool val)
    {
        GetComponent<Renderer>().enabled = val;
        GetComponent<Collider>().enabled = val;
        for (int n = 0; n < transform.childCount; n++)
            transform.GetChild(n).gameObject.SetActive(val);
    }

    /// <summary>
    /// Sets a GeneralFlag in qubit manager based on what toolobject is currently in use.
    /// </summary>
    /// <param name="action"></param>
    public void setGeneralFlagInQubitManager(string action)
    {
        m_qubitManager.setGeneralFlag(m_type + m_doubleGateNumber + "_" + action);
    }
}