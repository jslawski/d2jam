using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject _pointsPanel;
    [SerializeField]
    private GameObject _moreButton;
    [SerializeField]
    private GameObject _closeButton;
    [SerializeField]
    private VerticalLayoutGroup _verticalLayoutGroup;

    public void OpenPointsPanel()
    {
        this._pointsPanel.SetActive(true);
        this._moreButton.SetActive(false);
        this._closeButton.SetActive(true);

        //Change padding
    }

    public void ClosePointsPanel()
    {
        this._pointsPanel.SetActive(false);
        this._moreButton.SetActive(true);
        this._closeButton.SetActive(false);

        //Change padding
    }
}
