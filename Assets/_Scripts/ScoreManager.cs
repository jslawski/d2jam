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

    public int GetCalculatedScore()
    {
        int timeDeduction = Mathf.RoundToInt(this.timeToComplete * this._timeMultiplier);
        int timeScore = this._timeMaxScore - timeDeduction;

        int distanceDeduction = Mathf.RoundToInt(this.distanceTravelled * this._distanceMultiplier);
        int distanceScore = this._distanceMaxScore - distanceDeduction;

        int collectibleScore = this._scorePerCollectible * this.collectiblesGrabbed;

        return (timeScore + distanceScore + collectibleScore + this._scoreForFinishingLevel);
    }

    public void ResetCurrentScore()
    {
        this.timeToComplete = 0.0f;
        this.distanceTravelled = 0.0f;
        this.collectiblesGrabbed = 0;
    }
}
