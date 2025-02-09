using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedGate : MonoBehaviour
{
    [SerializeField] private QubitManager m_qubitManager;
    [SerializeField] private ToolObject m_animatedGate;
    // Start is called before the first frame update
    void Start()
    {
        if (m_animatedGate.GetComponent<SGateObject>() != null || m_animatedGate.GetComponent<TGateObject>() != null)
        {
            // ERROR IN SECTION 2 TUTORIAL: for some reason, the m_qubitMangager reference just drops off the face of the
            // earth and i have NO idea why. maybe consider using a better game engine
            m_qubitManager.ApplyGateManually(new HGate(1, 0));
            m_qubitManager.PointAllVectors();
        }

        m_animatedGate.setQubitManager(m_qubitManager);
        m_animatedGate.setGateIsBeingHeld(true);
        m_animatedGate.transform.parent.gameObject.SetActive(true);
    }
}