using UnityEngine;

public static class LevelList
{
    public static int currentLevelIndex = 0;
    public static Level[] allLevels;

    public static void SetupList(Level[] levelResources)
    {
        LevelList.allLevels = levelResources;
    }

    public static Level GetLevel(int index)
    {
        if (LevelList.allLevels == null)
        {
            LevelList.SetupList(Resources.LoadAll<Level>("Levels"));
        }

        if (index >= LevelList.allLevels.Length)
        {
            return null;
        }

        return LevelList.allLevels[index];
    }

    public static Level GetCurrentLevel()
    {
        if (LevelList.allLevels == null)
        {
            LevelList.SetupList(Resources.LoadAll<Level>("Levels"));
        }
    
        return LevelList.allLevels[LevelList.currentLevelIndex];
    }

    public static void SetLevelIndex(int index)
    {
        if (LevelList.allLevels == null)
        {
            LevelList.SetupList(Resources.LoadAll<Level>("Levels"));
        }

        LevelList.currentLevelIndex = index;
    }
}
