using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbelt : MonoBehaviour
{
    protected QubitManager m_qubitManager;
    public List<ToolObject> m_toolObjects;

    private Vector3 LeftEndpoint;
    private Vector3 RightEndpoint;
    public float offsetLeft;
    public float offsetRight;
    public float heightLeft;
    public float heightRight;

    public Vector3 getLeftEndpoint() { return LeftEndpoint; }
    public Vector3 getRightEndpoint() { return RightEndpoint; }

    /// <summary>
    /// Sets certain tool objects active/inactive based on a list of boolean values.
    /// </summary>
    /// <param name="boolList"></param>
    public void setToolObjectsActive(List<bool> boolList)
    {
        if (boolList.Count != m_toolObjects.Count)
        {
            Debug.LogError("Size of ToolObject list does not match size of passed boolList; in setToolObjectsActive");
            return;
        }

        for (int n = 0; n < m_toolObjects.Count; n++)
            m_toolObjects[n].setGateActive(boolList[n]);
    }

    /// <summary>
    /// Sets the QubitManager reference of the Toolbelt and all the ToolObjects.
    /// </summary>
    /// <param name="qubitManager"></param>
    public void setQubitManagerReference(QubitManager qubitManager)
    {
        m_qubitManager = qubitManager;

        for (int n = 0; n < m_toolObjects.Count; n++)
            m_toolObjects[n].setQubitManager(m_qubitManager);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int n = 0; n < m_toolObjects.Count; n++) 
            m_toolObjects[n].setGateActive(true);
    }

    /// <summary>
    /// Continuously positions the tools in front of the user.
    /// </summary>
    void Update()
    {
        Vector3 tempLeft = Camera.main.transform.position + Camera.main.transform.right * offsetLeft;
        Vector3 tempRight = Camera.main.transform.position + Camera.main.transform.right * (-offsetRight);
        LeftEndpoint = new Vector3(tempLeft.x, Camera.main.transform.position.y * heightLeft, tempLeft.z);
        RightEndpoint = new Vector3(tempRight.x, Camera.main.transform.position.y * heightRight, tempRight.z);
    }
}