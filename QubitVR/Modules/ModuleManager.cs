using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    [SerializeField] private ProgressionConditionLibrary progressionConditionsLibrary;
    [SerializeField] private ModuleConditionsLibrary moduleConditionsLibrary;
    [SerializeField] private QubitManager qubitManager;
    [SerializeField] private List<GameObject> m_modules;
    [SerializeField] private int parentStartingIndex;
    [SerializeField] private int childStartingIndex;

    private int m_currentIndex;
    private ModuleManager parentModuleManager;
    private List<ProgressionCondition> progressionConditions = null;
    private ModuleConditions moduleConditions;
    private Toolbelt toolbelt;

    public void setParentModuleManager(ModuleManager moduleManager) { parentModuleManager = moduleManager; }
    public int getCurrentIndex() { return m_currentIndex; }

    // Start is called before the first frame update
    void Start()
    {
        // Give children modules a reference to current instance of module manager
        ModuleController moduleController;
        ModuleManager moduleManager;
        for (int n = 0; n < m_modules.Count; n++)
        {
            moduleController = m_modules[n].GetComponent<ModuleController>();
            if (moduleController != null)
                moduleController.setModuleManager(this);

            moduleManager = m_modules[n].GetComponent<ModuleManager>();
            if (moduleManager != null)
                moduleManager.setParentModuleManager(this);
        }

        // Set the module slides active
        for (int n = 0; n < m_modules.Count; n++)
            m_modules[n].SetActive(false);

        if (parentModuleManager == null)
            m_currentIndex = parentStartingIndex;
        else
        {
            // Only perform these actions if we are in the child module manager
            m_currentIndex = childStartingIndex;
            toolbelt = qubitManager.getToolbelt();
            getProgressionCondition();
            setupModuleConditions();
        }

        m_modules[m_currentIndex].SetActive(true);
    }

    /// <summary>
    /// Moves to the next module in the module list.
    /// </summary>
    public void nextModule()
    {
        if (m_currentIndex + 1 == m_modules.Count)
        {
            CloseEntireModuleAndOpenNext();
            return;
        }
        else if (m_currentIndex < 0 || m_currentIndex + 1 >= m_modules.Count)
        {
            Debug.LogError("Current index out of range in ModuleManager");
            return;
        }

        m_modules[m_currentIndex++].SetActive(false);
        m_modules[m_currentIndex].SetActive(true);
        setupModuleConditions();
        getProgressionCondition();
    }

    /// <summary>
    /// Moves to the previous module in the module list.
    /// </summary>
    public void previousModule()
    {
        if (m_currentIndex < 1 || m_currentIndex >= m_modules.Count)
        {
            Debug.LogError("Current index out of range in ModuleManager");
            return;
        }

        m_modules[m_currentIndex--].SetActive(false);
        m_modules[m_currentIndex].SetActive(true);
        setupModuleConditions();
        getProgressionCondition();
    }

    /// <summary>
    /// Moves to the module index specified by index.
    /// </summary>
    /// <param name="index">Index of module in module list</param>
    public void openModule(int index)
    {
        //Debug.Log("open module");
        if (m_currentIndex < 0 || m_currentIndex >= m_modules.Count)
        {
            Debug.LogError("Current index out of range in ModuleManager");
            return;
        }

        if (index < 0 || index >= m_modules.Count)
        {
            Debug.LogError("Provided index out of range in ModuleManager");
            return;
        }

        m_modules[m_currentIndex].SetActive(false);
        m_currentIndex = index;
        m_modules[m_currentIndex].SetActive(true);
        setupModuleConditions();
        getProgressionCondition();
    }

    /// <summary>
    /// Closes all modules in the current list of modules and opens the next module in the parent module manager.
    /// </summary>
    public void CloseEntireModuleAndOpenNext()
    {
        for (int n = 0; n < m_modules.Count; n++)
            m_modules[n].SetActive(false);
        if (parentModuleManager != null)
            parentModuleManager.nextModule();
    }

    /// <summary>
    /// Fetch the progression condition corresponding to the current index and the parentModuleManager's index.
    /// </summary>
    public void getProgressionCondition()
    {
        progressionConditions = null;

        if (parentModuleManager != null)
        {
            // Get the progression condition for the current module
            progressionConditions = progressionConditionsLibrary.getConditions(parentModuleManager.getCurrentIndex(), m_currentIndex);

            // If the previous scene has a progression condition, remove the previous button of this scene
            if (m_currentIndex == 0 || progressionConditionsLibrary.getConditions(parentModuleManager.getCurrentIndex(), m_currentIndex - 1) != null)
                m_modules[m_currentIndex].GetComponent<ModuleController>().backButtonIsActive(false);
        }

        // Set the next button inactive if we find a progression condition
        if (progressionConditions != null)
            m_modules[m_currentIndex].GetComponent<ModuleController>().nextButtonIsActive(false);

        qubitManager.clearProgressionRelatedParameters();
    }

    /// <summary>
    /// Fetch the module condition corresponding to the current index and the parentModuleManager's index.
    /// </summary>
    public void setupModuleConditions()
    {
        if (parentModuleManager != null)
            moduleConditions = moduleConditionsLibrary.getModuleConditions(parentModuleManager.getCurrentIndex(), m_currentIndex);
        if (moduleConditions == null)
            return;

        // Set active the proper tool objects for the scene
        toolbelt.setToolObjectsActive(moduleConditions.getListOfGateEnabledFlags());

        // Set the proper qubits active
        List<bool> activeQubits = moduleConditions.getEnabledQubits();
        for (int n = 0; n < activeQubits.Count; n++)
            qubitManager.setQubitActive(n, activeQubits[n]);

        // Lock the proper qubits
        qubitManager.lockQubits(moduleConditions.getLockedQubits());

        // Set the state of the qubits
        List<Gate> gateSequence = moduleConditions.getGateSequence();
        if (gateSequence.Count != 0)
        {
            if (qubitManager == null)
                Debug.Log("1");
            qubitManager.ResetSystem();
            for (int n = 0; n < gateSequence.Count; n++)
                qubitManager.ApplyGateManually(gateSequence[n]);
            qubitManager.PointAllVectors();
        }
    }

    /// <summary>
    /// Constantly check if the fetched conditions match that of the current state of QubitVR.
    /// </summary>
    private void Update()
    {
        // Cannot test for conditions if the progression condition is null
        if (progressionConditions == null)
            return;

        int count = progressionConditions.Count;
        for (int n = 0; n < count; n++)
        {
            if (progressionConditions == null || n >= progressionConditions.Count)
                break;

            string conditionType = progressionConditions[n].getType();
            switch (conditionType)
            {
                case "QuantumStateAsArray":
                    if (progressionConditions[n].isEqual(qubitManager.getQuantumState()))
                        openModule(progressionConditions[n].getDestinationIndex());
                    break;
                case "GateAsArray":
                    if (progressionConditions[n].isEqual(qubitManager.getCurrentGate()))
                        openModule(progressionConditions[n].getDestinationIndex());
                    break;
                case "MeasurementIndex":
                    if (progressionConditions[n].isEqual(qubitManager.getMeasurementIndices()))
                        openModule(progressionConditions[n].getDestinationIndex());
                    break;
                case "GeneralFlag":
                    if (progressionConditions[n].isEqual(qubitManager.getGeneralFlag()))
                        openModule(progressionConditions[n].getDestinationIndex());
                    break;
            }
        }
    }
}