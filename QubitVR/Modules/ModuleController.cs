using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleController : MonoBehaviour
{
    private ModuleManager m_moduleManager;
    [SerializeField] private GameObject m_nextButton;
    [SerializeField] private GameObject m_backButton;

    public void setModuleManager(ModuleManager moduleManager) { m_moduleManager = moduleManager; }

    /// <summary>
    /// Called when the 'Next' button is pressed on a menu.
    /// </summary>
    public void nextButton()
    {
        if (m_moduleManager != null)
            m_moduleManager.nextModule();
        else
            Debug.LogError("Null ModuleManager reference in ModuleController");
    }

    /// <summary>
    /// Called when the 'Back' button is pressed on a menu.
    /// </summary>
    public void backButton()
    {
        if (m_moduleManager != null)
            m_moduleManager.previousModule();
        else
            Debug.LogError("Null ModuleManager reference in ModuleController");
    }

    /// <summary>
    /// Enable/Disable the next button based on 'val'.
    /// </summary>
    /// <param name="val"></param>
    public void nextButtonIsActive(bool val)
    {
		// The button is highlighted red instead of hiding it so that the modules can be tested.
        //m_nextButton.SetActive(val);
        m_nextButton.GetComponent<Image>().color = Color.red;
    }

    /// <summary>
    /// Enable/Disable the back button based on 'val'.
    /// </summary>
    /// <param name="val"></param>
    public void backButtonIsActive(bool val)
    {
		// The button is highlighted red instead of hiding it so that the modules can be tested.
        //m_backButton.SetActive(val);
        m_backButton.GetComponent<Image>().color = Color.red;
    }
}