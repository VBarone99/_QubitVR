using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisOfRotation : MonoBehaviour
{
    /// <summary>
    /// Set the rotation of the cylindrical axis for the display of each gate rotation
    /// </summary>
    /// <param name="gateType"></param>
    public void SetAxisOfRotation(string gateType)
    {
        Quaternion quat = new Quaternion(0, 0, 0, 1);

        switch(gateType)
        {
            case "HGate":
                quat.eulerAngles = new Vector3(0f, 0f, 135f);
                break;
            case "XGate":
                quat.eulerAngles = new Vector3(0f, 0f, 90f);
                break;
            case "SGate":
                quat.eulerAngles = new Vector3(0f, 0f, 0f);
                break;
            case "TGate":
                quat.eulerAngles = new Vector3(0f, 0f, 0f);
                break;
            case "CNOTGate":
                quat.eulerAngles = new Vector3(0f, 0f, 90f);
                break;
            default:
                break;
        }

        // Set the rotation of the AxisOfRotation
        transform.rotation = quat;
    }

    /// <summary>
    ///  Enable the AxisOfRotation game object.
    /// </summary>
    /// <param name="val">Boolean value to set the AxisOfRotation active or not.</param>
    public void EnableAxisOfRotation(bool val)
    {
        transform.gameObject.SetActive(val);
    }
}