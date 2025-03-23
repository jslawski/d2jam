using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsSliderValue : MonoBehaviour
{
    [SerializeField]
    private Slider _settingSlider;

    private TextMeshProUGUI _valueText;

    private void Awake()
    {
        this._valueText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        this._valueText.text = Math.Round((double)this._settingSlider.value, 2).ToString();
    }
}
