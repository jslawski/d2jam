using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseRotationToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Awake()
    {
        this._toggle = GetComponent<Toggle>();
        this._toggle.isOn = GlobalVariables.MOUSE_ROTATION;
    }

    public void ToggleMouseRotation()
    {
        GlobalVariables.MOUSE_ROTATION = this._toggle.isOn;
    }
}
