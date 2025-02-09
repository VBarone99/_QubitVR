using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlochVector : MonoBehaviour
{
    public GameObject m_line;
    public GameObject m_terminus;

    private GameObject m_blochVector;
    private Vector3 m_qubitPosition;
    private Vector3 m_initialLineScale;
    private Vector3 m_initialTerminusScale;

    void OnEnable()
    {
        m_blochVector = transform.gameObject;
        m_qubitPosition = transform.parent.parent.position;
        m_initialLineScale = m_line.transform.localScale;
        m_initialTerminusScale = m_terminus.transform.localScale;
    }

    /// <summary>
    /// Point the bloch-vector object towards location specified by 'location'.
    /// </summary>
    /// <param name="location"></param>
    public void PointBlochVector(Vector3 location)
    {
        Vector3 target = new Vector3(location.x, location.z, location.y);
        m_blochVector.transform.LookAt(m_qubitPosition + target);
    }

    /// <summary>
    /// Scale bloch-vector based on the magnitude of the location vector.
    /// </summary>
    /// <param name="size">Magnitude of bloch-vector.</param>
    public void ScaleBlochVector(float size)
    {
        m_line.transform.localScale = size * m_initialLineScale;
        m_terminus.transform.localScale = size * m_initialTerminusScale;
    }
}