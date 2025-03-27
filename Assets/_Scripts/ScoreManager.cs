using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int _timeMaxScore = 6666;
    private int _distanceMaxScore = 4444;
    private int _scorePerCollectible = 1000;
    private int _scoreForFinishingLevel = 1000;
    private int _timeMultiplier = 222;
    private int _distanceMultiplier = 60;

    public float timeToComplete = 0.0f;
    public float distanceTravelled = 0.0f;
    public int collectiblesGrabbed = 0;
    public int completionScore = 1000;    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void IncrementTimer()
    {
        this.timeToComplete += Time.fixedDeltaTime;
    }

    public void IncrementDistanceTravelled(float incrementDistance)
    {
        this.distanceTravelled += incrementDistance;
    }

    public void IncrementCollectiblesGrabbed()
    {
        this.collectiblesGrabbed++;
    }

    public int GetTimeScore()
    {
        int timeDeduction = Mathf.RoundToInt(this.timeToComplete * this._timeMultiplier);
        return (this._timeMaxScore - timeDeduction);
    }

    public int GetDistanceScore()
    {
        int distanceDeduction = Mathf.RoundToInt(this.distanceTravelled * this._distanceMultiplier);
        return (this._distanceMaxScore - distanceDeduction);
    }

    public int GetCollectibleScore()
    { 
        return (this._scorePerCollectible * this.collectiblesGrabbed);
    }

    public int GetTotalCalculatedScore()
    {
        return this.GetTimeScore() + this.GetDistanceScore() + this.GetCollectibleScore();        
    }

    public void ResetCurrentScore()
    {
        this.timeToComplete = 0.0f;
        this.distanceTravelled = 0.0f;
        this.collectiblesGrabbed = 0;
    }

    public void UpdateLatestHighScore()
    {        
        string playerName = PlayerPrefs.GetString("username", "");
        UpdateCabbageLeaderboardAsyncRequest request = new UpdateCabbageLeaderboardAsyncRequest(playerName, this.GetTotalCalculatedScore().ToString(), 
                                                                                           LevelList.GetCurrentLevel().sceneName, this.UpdateSuccess, this.UpdateFailure);
                                                                                              
        #if !UNITY_EDITOR
        request.Send();        
        #endif
    }

    private void UpdateSuccess(string data)
    {
        LeaderboardManager.instance.RefreshLeaderboard(LevelList.GetCurrentLevel().sceneName);
        UIManager.instance.RefreshLatestHighScoreValues();        
    }

    private void UpdateFailure()
    {
        Debug.LogError("Failed to update leaderboard high score");
    }
}
