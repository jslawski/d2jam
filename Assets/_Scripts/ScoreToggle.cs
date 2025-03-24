using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject _pointsPanel;
    [SerializeField]
    private GameObject _moreButton;
    [SerializeField]
    private GameObject _closeButton;
    [SerializeField]
    private GameObject _nextLevelButton;
    [SerializeField]
    private VerticalLayoutGroup _verticalLayoutGroup;

    public void Start()
    {
        if (LevelList.currentLevelIndex < LevelList.allLevels.Length - 1)
        {
            this._nextLevelButton.SetActive(true);
        }
        else
        {
            this._nextLevelButton.SetActive(false);
        }
    }

    public void OpenPointsPanel()
    {
        this._pointsPanel.SetActive(true);
        this._moreButton.SetActive(false);
        this._closeButton.SetActive(true);

        this._verticalLayoutGroup.padding.bottom = -250;
    }

    public void ClosePointsPanel()
    {
        this._pointsPanel.SetActive(false);
        this._moreButton.SetActive(true);
        this._closeButton.SetActive(false);

        this._verticalLayoutGroup.padding.bottom = 0;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        GlobalVariables.PREVENT_SHOOTING = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        GlobalVariables.PREVENT_SHOOTING = false;
    }
}
