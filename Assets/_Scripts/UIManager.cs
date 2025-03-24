using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using CabbageNetwork;
using DG.Tweening.Core.Easing;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private TextMeshProUGUI _currentCompletionScore;
    [SerializeField]
    private TextMeshProUGUI _currentTime;
    [SerializeField]
    private TextMeshProUGUI _currentTimeScore;
    [SerializeField]
    private TextMeshProUGUI _currentDistanceTraveled;
    [SerializeField]
    private TextMeshProUGUI _currentDistanceScore;
    [SerializeField]
    private TextMeshProUGUI _currentCollectiblesCount;
    [SerializeField]
    private TextMeshProUGUI _currentCollectiblesScore;
    [SerializeField]
    private TextMeshProUGUI _currentTotalScore;

    [SerializeField]
    private TextMeshProUGUI _pbPlacement;
    [SerializeField]
    private TextMeshProUGUI _pbUsername;
    [SerializeField]
    private TextMeshProUGUI _pbScore;

    private bool _displayCurrentPointValues = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        this.RefreshLatestHighScoreValues();

        if (BGMManager.instance != null && BGMManager.instance.gameplayBGM.volume < 1)
        {
            BGMManager.instance.FadeToGameplayBGM();
        }

        this._pbUsername.text = PlayerPrefs.GetString("username", "--");
    }

    private List<int> GetTimerValues()
    {
        List<int> timerValues = new List<int>();

        int secondsValue = Mathf.FloorToInt(ScoreManager.instance.timeToComplete);
        timerValues.Add(secondsValue);

        int millisecondsValue = (int)((ScoreManager.instance.timeToComplete - secondsValue) * 100);
        timerValues.Add(millisecondsValue);

        return timerValues;
    }

    public string GetFormattedTime(float timeToFormat)
    {
        List<int> timerValues = this.GetTimerValues();

        string secondsValue = (timerValues[0] > 9) ? timerValues[0].ToString() : "0" + timerValues[0].ToString();
        string millisecondsValue = (timerValues[1] > 9) ? timerValues[1].ToString() : "0" + timerValues[1].ToString();

        return (secondsValue + "." + millisecondsValue);
    }

    public string GetFormattedDistance(float distanceToFormat)
    {
        return Math.Round((double)distanceToFormat, 2).ToString();
    }

    private void Update()
    {
        this._currentTime.text = this.GetFormattedTime(ScoreManager.instance.timeToComplete);
        this._currentDistanceTraveled.text = this.GetFormattedDistance(ScoreManager.instance.distanceTravelled);
        this._currentCollectiblesCount.text = ScoreManager.instance.collectiblesGrabbed.ToString();

        this._currentCompletionScore.text = ScoreManager.instance.completionScore.ToString();
        this._currentTimeScore.text = ScoreManager.instance.GetTimeScore().ToString();
        this._currentDistanceScore.text = ScoreManager.instance.GetDistanceScore().ToString();
        this._currentCollectiblesScore.text = ScoreManager.instance.GetCollectibleScore().ToString();
        this._currentTotalScore.text = ScoreManager.instance.GetTotalCalculatedScore().ToString();

        if (this._displayCurrentPointValues == true)
        {
            this._currentCompletionScore.gameObject.SetActive(true);
            this._currentTimeScore.gameObject.SetActive(true);
            this._currentDistanceScore.gameObject.SetActive(true);
            this._currentCollectiblesScore.gameObject.SetActive(true);
            this._currentTotalScore.gameObject.SetActive(true);
        }
        else
        {
            this._currentCompletionScore.gameObject.SetActive(false);
            this._currentTimeScore.gameObject.SetActive(false);
            this._currentDistanceScore.gameObject.SetActive(false);
            this._currentCollectiblesScore.gameObject.SetActive(false);
            this._currentTotalScore.gameObject.SetActive(false);
        }
    }

    public void DisplayCurrentPointValues()
    {
        this._displayCurrentPointValues = true;
    }

    public void HideCurrentPointValues()
    {
        this._displayCurrentPointValues = false;
    }

    public void RefreshLatestHighScoreValues()
    {
        string playerName = PlayerPrefs.GetString("username", "");
        GetCabbageLeaderboardEntryAsyncRequest request = new GetCabbageLeaderboardEntryAsyncRequest(playerName, LevelList.GetCurrentLevel().sceneName, this.GetDataSuccess, this.GetDataFailure);
        request.Send();
    }

    private void GetDataSuccess(string data)
    {
        //Empty leaderboard, return
        if (data == "[]")
        {
            return;
        }

        LeaderboardEntryData leaderboardEntry = JsonUtility.FromJson<LeaderboardEntryData>(data);

        this._pbPlacement.text = leaderboardEntry.placement.ToString();
        this._pbScore.text = leaderboardEntry.value.ToString();
        this._pbUsername.text = leaderboardEntry.username;
    }

    private void GetDataFailure()
    {
        this._pbPlacement.text = "--";
        this._pbScore.text = "--";

        this._pbUsername.text = PlayerPrefs.GetString("username", "--");
    }

    public void ReturnToLevelSelect()
    {
        SceneLoader.instance.LoadScene("LevelSelect");
    }
}
