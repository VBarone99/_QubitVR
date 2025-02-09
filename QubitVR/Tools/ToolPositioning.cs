using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPositioning : MonoBehaviour
{
    private Vector3 LeftEndpoint;
    private Vector3 RightEndpoint;
    private Camera m_camera;

    // The bigger the number, the further left the tool will be.
    // But the tools will still be between the endpoints.
    // BTW, when HowFarLeft=0, HowFarForward seems to stay at 0
    public float InterpFromLeft;
    public float offsetForward;

    private void Start()
    {
        m_camera = Camera.main;
    }

    /// <summary>
    /// Continuously positions the tools in front of the user.
    /// </summary>
    void Update()
    {
        LeftEndpoint = transform.parent.GetComponent<Toolbelt>().getLeftEndpoint();
        RightEndpoint = transform.parent.GetComponent<Toolbelt>().getRightEndpoint();

        // Determine position between the endpoints.
        Vector3 resultingPosition = Vector3.Lerp(LeftEndpoint, RightEndpoint, InterpFromLeft);
        // Get vector from that position to an endpoint.
        Vector3 vector1 = resultingPosition - LeftEndpoint;
        // Use that vector with the UP vector to get a perpendicular vector that points straight away from the user.
        Vector3 normVector = Vector3.Cross(vector1, Vector3.down).normalized;
        // Determine the distance from the user.
        transform.position = resultingPosition + (normVector * offsetForward);
        // Have the tool look at the user so that its orientation always looks the same to the user.
        transform.LookAt(transform.position + (m_camera.transform.rotation * Vector3.right), (m_camera.transform.rotation * Vector3.up));
    }
}