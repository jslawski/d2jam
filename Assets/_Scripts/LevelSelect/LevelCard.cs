using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCard : MonoBehaviour
{
    private Level _associatedLevel;

    [SerializeField]
    private TextMeshProUGUI _leaderboardPositionText;
    [SerializeField]
    private TextMeshProUGUI _levelNameText;
    [SerializeField]
    private Image _levelImage;
    [SerializeField]
    private TextMeshProUGUI _playerScoreText;
    [SerializeField]
    private AudioClip _selectLevelSound;

    public void SetupLevelCard(Level setupLevel)
    {
        this._associatedLevel = setupLevel;
        this._levelNameText.text = setupLevel.levelName;

        this._levelImage.sprite = Resources.Load<Sprite>("LevelImages/" + setupLevel.imageFileName);

        this.GetLatestHighscoreValues();
    }

    private void GetLatestHighscoreValues()
    {
        string playerName = PlayerPrefs.GetString("username", "");
        GetCabbageLeaderboardEntryAsyncRequest request = new GetCabbageLeaderboardEntryAsyncRequest(playerName, this._associatedLevel.sceneName, this.GetDataSuccess, this.GetDataFailure);
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

        if (leaderboardEntry.value == 0)
        {
            this._leaderboardPositionText.text = "--";
            this._playerScoreText.text = "Unbeaten";
        }
        else
        {
            this._leaderboardPositionText.text = leaderboardEntry.placement.ToString();
            this._playerScoreText.text = leaderboardEntry.value.ToString();
        }
    }

    private void GetDataFailure()
    {
        this._leaderboardPositionText.text = "--";
        this._leaderboardPositionText.text = "Unbeaten";
    }

    public void SelectLevel()
    {
        AudioChannelSettings channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");

        if (this._selectLevelSound != null)
        {
            AudioManager.instance.Play(this._selectLevelSound, channelSettings);
        }

        LevelList.SetLevelIndex(this._associatedLevel.levelIndex);

        //Load into the next scene here
        SceneLoader.instance.LoadScene(this._associatedLevel.sceneName);
    }
}
