using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void Start()
    {
        if (BGMManager.instance != null && BGMManager.instance.menuBGM.volume < 1)
        {
            BGMManager.instance.FadeToMenuBGM();
        }
    }

    public void GoToLevelSelect()
    {
        SceneLoader.instance.LoadScene("LevelSelect");
    }

    public void ExitGame()
    { 
        Application.Quit();
    }
}
