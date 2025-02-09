using System.Collections.Generic;
using UnityEngine;

public class QubitManager : MonoBehaviour
{
    [SerializeField] private List<Qubit> qubits;
    [SerializeField] private Toolbelt toolbelt;
    [SerializeField] private GlobalSettings settings;
    [SerializeField] private bool QubitsStartActive;

    private int NthRoot;
    private bool previewEnabled;
    private int systemSize;
    private QuantumState state;
    private QuantumState previewState;
    private Gate currentGate;
    private Gate pendingGate;
    private List<int> measurementIndices;
    private string GeneralFlag;

    public bool previewIsEnabled() { return previewEnabled; }
    public QuantumState getQuantumState() { return state; }
    public Gate getCurrentGate() { return currentGate; }
    public List<int> getMeasurementIndices() { return measurementIndices; }

    public Toolbelt getToolbelt() { return toolbelt; }

    public void setGeneralFlag(string flag) { GeneralFlag = flag; }
    public string getGeneralFlag() { return GeneralFlag; }

    // Start is called before the first frame update
    void Start()
    {
        toolbelt.setQubitManagerReference(this);
        CreateSystem();

        NthRoot = settings.NthRoot;
        previewEnabled = settings.PreviewEnabled;
        for (int n = 0; n < systemSize; n++)
        {
            setQubitActive(n, QubitsStartActive);
            qubits[n].populateSettings(settings);
        }
    }

    private void OnDisable()
    {
        for (int n = 0; n < systemSize; n++)
            if (qubits[n] != null)
                qubits[n].gameObject.SetActive(false);
    }

    /// <summary>
    /// Create system of qubits based on list of qubits set in inspector.
    /// </summary>
    void CreateSystem()
    {
        if (qubits.Count > 0)
        {
            systemSize = qubits.Count;
            state = new QuantumState(systemSize);
        }
        else
            Debug.LogError("System size must be greater than 0.");
    }

    /// <summary>
    /// Reset the qubit system to all default states and positions.
    /// </summary>
    public void ResetSystem()
    {
        Vector3 defaultBlochVectorPosition = new Vector3(0, 0, 1);
        state = new QuantumState(systemSize);
        for (int n = 0; n < systemSize; n++)
            qubits[n].PointVector(defaultBlochVectorPosition);  
    }

    /// <summary>
    /// Lock the qubits corresponding to the list of boolean values.
    /// </summary>
    /// <param name="boolList"></param>
    public void lockQubits(List<bool> boolList)
    {
        if (boolList.Count > qubits.Count)
        {
            Debug.LogError("Too many lock flags in lockQubits");
            return;
        }

        if (boolList.Count == qubits.Count)
            for (int n = 0; n < qubits.Count; n++)
                qubits[n].setQubitIsLocked(boolList[n]);
    }

    /// <summary>
    /// Set the qubit specified by index to Enabled/Disabled depending on 'val'.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="val"></param>
    public void setQubitActive(int index, bool val)
    {
        qubits[index].gameObject.SetActive(val);
    }

    /// <summary>
    /// Apply a gate manually to the system without the need for any previews or VR interactions. This does NOT update the visualization.
    /// </summary>
    /// <param name="gate"></param>
    public void ApplyGateManually(Gate gate)
    {
        state.ApplyGate(gate);
    }

    /// <summary>
    /// Point all vectors to their expectation values. This usually follows ApplyGateManually() to update the visualization.
    /// </summary>
    public void PointAllVectors()
    {
        for (int n = 0; n < systemSize; n++)
            qubits[n].PointVector(state.getExpectationValues(n));
    }

    /// <summary>
    /// Reset the parameters after a progression condition has been met.
    /// </summary>
    public void clearProgressionRelatedParameters()
    {
        currentGate = null;
        measurementIndices = null;
        GeneralFlag = "";
    }

    /// <summary>
    /// Update the state of the system to be that of the preview state. Used to finalize the preview state if the user applies the gate.
    /// </summary>
    public void ApplyPreviewStateToSystem()
    {
        state = previewState;
        Debug.Log(state.GetMatrix().ToString());
        for (int n = 0; n < systemSize; n++)
            qubits[n].AttemptToDespawnParticles("GateReleased");

        // If preview is enabled, all the vectors will already be pointed in the
        // preview function. If preview is not enabled, we need to manually do it here.
        if (!previewEnabled)
            PointAllVectors();

        currentGate = pendingGate;
    }

    /// <summary>
    /// Apply a flashlight object to the system. Parses the FlashlightObject and measures the qubit.
    /// </summary>
    /// <param name="flashlightObject">FlashlightObject</param>
    public void ApplyFlashlight(FlashlightObject flashlightObject)
    {
        Qubit targetQubit = flashlightObject.getTargetQubit();
        int targetIndex = -1;

        for (int n = 0; n < qubits.Count; n++)
            if (targetQubit == qubits[n])
                targetIndex = n;

        if (targetIndex == -1)
        {
            Debug.LogError("ApplyFlashlight could not find an index for the targetQubit reference");
            return;
        }

        measurementIndices = new List<int> { targetIndex };
        state.Measure(measurementIndices);
        PointAllVectors();
    }

    /// <summary>
    /// Convert a SingleGateObject into a Gate object.
    /// </summary>
    /// <param name="gateObject">SingleGateObject</param>
    /// <returns>Gate object</returns>
    private Gate ParseSingleGate(SingleGateObject gateObject)
    {
        //Debug.Log("Parse single gate");
        Qubit targetQubit = gateObject.getTargetQubit();
        int targetIndex = -1;

        for (int n = 0; n < qubits.Count; n++)
            if (targetQubit == qubits[n])
                targetIndex = n;

        if (targetIndex == -1)
        {
            Debug.LogError("ParseSingleGate could not find an index for the targetQubit reference");
            return new Gate();
        }

        Gate gate;
        string gateType = gateObject.getType();
        switch (gateType)
        {
            case "XGate":
                gate = new XGate(qubits.Count, targetIndex);
                break;
            case "HGate":
                gate = new HGate(qubits.Count, targetIndex);
                break;
            case "SGate":
                gate = new SGate(qubits.Count, targetIndex);
                break;
            case "TGate":
                gate = new TGate(qubits.Count, targetIndex);
                break;
            default:
                Debug.LogError("ApplySingleGate could not recognize the gate type");
                return new Gate();
        }

        // Pass the gate type to the AxisOfRotation to properly display on the qubit
        qubits[targetIndex].SetAxisOfRotation(gateType);

        // Set the current gate for the QubitManagerListener to use
        pendingGate = gate;

        return gate;
    }

    /// <summary>
    /// Convert a DoubleGateObject into a Gate object.
    /// </summary>
    /// <param name="gateObject">DoubleGateObject</param>
    /// <returns>Gate object</returns>
    private Gate ParseDoubleGate(DoubleGateObject gateObject)
    {
        Qubit controlQubit = gateObject.getControlQubit();
        Qubit targetQubit = gateObject.getTargetQubit();
        int controlIndex = -1, targetIndex = -1;

        for (int n = 0; n < qubits.Count; n++)
        {
            if (controlQubit == qubits[n])
                controlIndex = n;
            if (targetQubit == qubits[n])
                targetIndex = n;
        }

        if (controlIndex == -1 || targetIndex == -1)
        {
            Debug.LogError("ParseDoubleGate could not find an index for both the control and target qubit references.");
            return new Gate();
        }

        Gate gate;
        string gateType = gateObject.getType();
        switch (gateType)
        {
            case "CNOTGate":
                gate = new CNOTGate(qubits.Count, controlIndex, targetIndex);
                break;
            default:
                Debug.LogError("ApplyDoubleGate could not recognize the gate type");
                return new Gate();
        }

        // Pass the gate type to the AxisOfRotation to properly display on the qubit
        qubits[controlIndex].SetAxisOfRotation(gateType);
        qubits[targetIndex].SetAxisOfRotation(gateType);

        // Set the current gate for the QubitManagerListener to use
        pendingGate = gate;

        return gate;
    }

    /// <summary>
    /// Calls preview gate and passes in the parsed SingleGateObject as an NthRootGate.
    /// </summary>
    /// <param name="gateObject">SingleGateObject</param>
    public void PreviewSingleGate(SingleGateObject gateObject)
    {
        PreviewGate(ParseSingleGate(gateObject).NthRootGate(NthRoot));
    }

    /// <summary>
    /// Calls preview gate and passes in the parsed DoubleGateObject as an NthRootGate.
    /// </summary>
    /// <param name="gateObject">DoubleGateObject</param>
    public void PreviewDoubleGate(DoubleGateObject gateObject) // ApplyDoubleGate(DoubleGate gateObject, int N)
    {
        PreviewGate(ParseDoubleGate(gateObject).NthRootGate(NthRoot));
    }

    /// <summary>
    /// Creates preview animation of quantum state by applying the NthRootGate N times. Passes that list to the qubit to animate.
    /// </summary>
    /// <param name="gate"></param>
    private void PreviewGate(Gate gate)
    {
        List<QuantumState> NthRootStates = new List<QuantumState>();
        QuantumState tempState = new QuantumState(state.GetMatrix());
        NthRootStates.Add(state);

        // Apply NthRootGate to system N times. Store the NthRootState each time.
        for (int NthRootNum = 0; NthRootNum < NthRoot; NthRootNum++)
        {
            tempState.ApplyGate(gate);
            NthRootStates.Add(new QuantumState(tempState.GetMatrix()));
        }

        previewState = NthRootStates[NthRootStates.Count - 1];

        // Only continue in this function if the preview is enabled
        if (!previewEnabled)
            return;

        // For each qubit, animate the vector using the list of NthRootStates
        for (int qubitIndex = 0; qubitIndex < systemSize; qubitIndex++) //int qubitIndex = systemSize - 1; qubitIndex >= 0; qubitIndex--
        {
            List<Vector3> NthRootLocations = new List<Vector3>();

            for (int NthRootNum = 0; NthRootNum <= NthRoot; NthRootNum++)
                NthRootLocations.Add(NthRootStates[NthRootNum].getExpectationValues(qubitIndex));

            qubits[qubitIndex].StartPreviewAnimation(NthRootLocations);
        }
    }

    /// <summary>
    /// Instantly ends the preview for all qubits.
    /// </summary>
    public void StopAllPreviews()
    {
        //Debug.Log("Gate preview stop");
        for (int n = 0; n < qubits.Count; n++)
            qubits[n].StopPreviewAnimation();
    }
}