using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Qubit : MonoBehaviour
{
    [SerializeField] private GameObject shell;
    [SerializeField] private BlochVector m_blochVector;
    [SerializeField] private AxisOfRotation m_axisOfRotation;
    [SerializeField] private TrajectoryVisualization m_trajectoryVisualization;
    [SerializeField] private Material qubit_light_green;
    [SerializeField] private Material qubit_yellow;
    [SerializeField] private Material qubit_light_gray;

    private float totalTimeForPreview;
    private bool enableAxisOfRotation;
    private bool enableTrajectoryVisualization;
    private float timer;
    private int nthRootIndex;
    private List<Vector3> m_locations;
    private bool m_previewIsReady = false;

    // Start is called before the first frame update
    void Start()
    {
        m_axisOfRotation.EnableAxisOfRotation(enableAxisOfRotation);
    }

    /// <summary>
    /// Populate the settings from the GlobalSettings scriptable object instance.
    /// </summary>
    /// <param name="settings"></param>
    public void populateSettings(GlobalSettings settings)
    {
        enableAxisOfRotation = settings.EnableAxisOfRotation;
        enableTrajectoryVisualization = settings.EnableTrajectoryVisualization;
        totalTimeForPreview = settings.PreviewTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_previewIsReady && timer < 0)
        {
            PointVector(m_locations[nthRootIndex]);
            nthRootIndex++;
            timer = totalTimeForPreview / m_locations.Count;
            if (nthRootIndex >= m_locations.Count)
            {
                nthRootIndex = 0;
                m_previewIsReady = false;

                if (enableTrajectoryVisualization)
                    m_trajectoryVisualization.DisableEmissions();
            }
        }
        timer -= Time.deltaTime;
    }
    
    /// <summary>
    /// Points and scales the bloch-vector based on 'location'.
    /// </summary>
    /// <param name="location">Location representing the (x, y, z) expectation values on the bloch-sphere.</param>
    public void PointVector(Vector3 location)
    {
        m_blochVector.PointBlochVector(location);
        m_blochVector.ScaleBlochVector(location.magnitude);
    }

    /// <summary>
    /// Start animating the qubit by pointing the vector to each location in the list.
    /// </summary>
    /// <param name="locations">List of expectation values, one for each NthRootState.</param>
    public void StartPreviewAnimation(List<Vector3> locations)
    {
        m_locations = locations;
        nthRootIndex = 0;
        timer = totalTimeForPreview / m_locations.Count;
        m_previewIsReady = true;

        if (enableTrajectoryVisualization)
            m_trajectoryVisualization.EnableEmissions();
    }

    /// <summary>
    /// Stop the preview animation for the qubit. Reset back to original position.
    /// </summary>
    public void StopPreviewAnimation()
    {
        m_previewIsReady = false;
        PointVector(m_locations[0]);

        if (enableTrajectoryVisualization)
            m_trajectoryVisualization.DisableAndClearEmissions();
    }

    /// <summary>
    /// Set axis of rotation position based on type of gate.
    /// </summary>
    /// <param name="gateType">String representing type of gate.</param>
    public void SetAxisOfRotation(string gateType)
    {
        m_axisOfRotation.SetAxisOfRotation(gateType);
    }

    /// <summary>
    /// Sets the color of the qubit shell.
    /// </summary>
    /// <param name="color"></param>
    private void setQubitColor(Material color)
    {
        shell.GetComponent<MeshRenderer>().material = color;
    }

    /// <summary>
    /// Set the color of the qubit shell based on a colorString.
    /// </summary>
    /// <param name="colorString"></param>
    public void setQubitColor(string colorString)
    {
        if (colorString == "Default")
            setQubitColor(qubit_light_green);
        else if (colorString == "Control")
            setQubitColor(qubit_yellow);
        else if (colorString == "Locked")
            setQubitColor(qubit_light_gray);
        else
            setQubitColor(qubit_light_green);
    }

    /// <summary>
    /// Prevent the qubit from taking in Gates from the user by disabling the collider.
    /// </summary>
    /// <param name="val"></param>
    public void setQubitIsLocked(bool val)
    {
        shell.GetComponent<Collider>().enabled = !val;
        setQubitColor(val ? qubit_light_gray : qubit_light_green);
    }

    /// <summary>
    /// Attempt to despawn particles based on incoming flag.
    /// </summary>
    /// <param name="flagName">Condition that has been met to attempt to despawn particles.</param>
    public void AttemptToDespawnParticles(string flagName)
    {
        m_trajectoryVisualization.AttemptToDespawnParticles(flagName);
    }
}