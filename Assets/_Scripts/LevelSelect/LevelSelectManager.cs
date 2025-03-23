using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager instance;

    [SerializeField]
    private GameObject levelCardPrefab;

    [SerializeField]
    private RectTransform levelParent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.SetupLevelList();

        this.LoadLevelsIntoScene();
    }

    private void SetupLevelList()
    {
        LevelList.SetupList(Resources.LoadAll<Level>("Levels"));
    }

    private void LoadLevelsIntoScene()
    {
        for (int i = 0; i < LevelList.allLevels.Length; i++)
        {
            this.CreateLevelCard(i);
        }
    }

    private void CreateLevelCard(int levelIndex)
    {
        GameObject newLevelCard = GameObject.Instantiate(this.levelCardPrefab, levelParent);
        LevelCard levelCardComponent = newLevelCard.GetComponent<LevelCard>();

        levelCardComponent.SetupLevelCard(LevelList.allLevels[levelIndex]);
    }
}
