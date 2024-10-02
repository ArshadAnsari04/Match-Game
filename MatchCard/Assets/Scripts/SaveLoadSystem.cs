using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int score;
    public int matchCount;
}

public class SaveLoadSystem : Singleton<SaveLoadSystem>
{
    private string filePath;

    private void Start()
    {
        filePath = Application.persistentDataPath + "/gameData.json";
    }

    public void SaveGame(int score, int matchCount)
    {
        GameData data = new GameData
        {
            score = score,
            matchCount = matchCount
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    public GameData LoadGame()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }
}
