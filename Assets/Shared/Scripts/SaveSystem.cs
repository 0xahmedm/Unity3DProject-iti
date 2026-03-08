using UnityEngine;

public static class SaveSystem
{
    public static void MarkLevelComplete(string levelName)
    {
        PlayerPrefs.SetInt(levelName,1);
        PlayerPrefs.Save();
    }

    public static bool IsLevelComplete(string levelName)
    {
        return PlayerPrefs.GetInt(levelName,0)==1;
    }

    public static void SetCheckpoint(string sceneName)
    {
        PlayerPrefs.SetString("LastScene",sceneName);
    }

    public static string GetCheckpoint()
    {
        return PlayerPrefs.GetString("LastScene","Level1");
    }
}