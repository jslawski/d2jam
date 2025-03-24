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

        //Try to grab player's leaderboard stats here.  If nothing exists, then mark the level as "unbeaten"
        //If it does exist, then update the player's leaderboard position and score
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
        SceneLoader.instance.LoadScene("JaredScene");
    }
}
