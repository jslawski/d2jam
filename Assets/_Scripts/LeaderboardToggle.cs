using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeaderboardToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject _leaderboardObject;
    [SerializeField]
    private GameObject _closeButtonObject;
    [SerializeField]
    private GameObject _openButtonObject;
    [SerializeField]
    private VerticalLayoutGroup _verticalLayoutGroup;

    public void OpenLeaderboard()
    {
        this._leaderboardObject.SetActive(true);
        this._closeButtonObject.SetActive(true);
        this._openButtonObject.SetActive(false);

        this._verticalLayoutGroup.padding.bottom = -40;
    }

    public void CloseLeaderboard()
    {
        this._leaderboardObject.SetActive(false);
        this._closeButtonObject.SetActive(false);
        this._openButtonObject.SetActive(true);

        this._verticalLayoutGroup.padding.bottom = 360;
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
