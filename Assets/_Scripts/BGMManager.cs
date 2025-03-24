using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    public AudioSource menuBGM;    
    public AudioSource gameplayBGM;

    private float _timeToFade = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        this.menuBGM.volume = 0.0f;
        this.gameplayBGM.volume = 0.0f;

        this.menuBGM.Play();
        this.gameplayBGM.Play();
    }

    public void FadeToGameplayBGM()
    {
        this.menuBGM.DOFade(0.0f, this._timeToFade).SetEase(Ease.Linear);
        this.gameplayBGM.DOFade(1.0f, this._timeToFade).SetEase(Ease.Linear);
    }

    public void FadeToMenuBGM()
    {
        this.menuBGM.DOFade(1.0f, this._timeToFade).SetEase(Ease.Linear);
        this.gameplayBGM.DOFade(0.0f, this._timeToFade).SetEase(Ease.Linear);
    }
}
