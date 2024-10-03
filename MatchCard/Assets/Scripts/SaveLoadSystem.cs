using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CardData
{
    public int id;
    public bool flipped;
    public int spriteID;

    
}

[System.Serializable]
public class GameData
{
    public int score;
    public int matchCount;
    public int cardLeft;
    public List<int> removedCardIDs; // Use List<int> for serialization
    public int difficultyLevel; // Assuming you have a difficulty level field
    public List<CardData> cardData; // Assuming you have a CardData class to hold individual card state
}



public class SaveLoadSystem : Singleton<SaveLoadSystem>
{
    private string filePath;

    private void Start()
    {
        // Set the file path for saving the game data
        filePath = Path.Combine(Application.persistentDataPath, "gameData.json");
       
        // Ensure the save directory exists
        EnsureSaveDirectory();
    }

    // Ensure the directory exists for saving the game
    private void EnsureSaveDirectory()
    {
        string directory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
           
        }
    }
   
    public void SaveGame(int score, int matchCount, Card[] cards, int cardLeft, HashSet<int> removedCardIDs, int difficultyLevel)
    {
       
        // Check for null parameters
        if (cards == null|| removedCardIDs == null)
        {
            
            return; // Exit the method to prevent further errors
        }

       

        // Collect only unmatched card data
        GameData gameData = new GameData();

        gameData.score = score;
        gameData.matchCount = matchCount;
        gameData.cardLeft = cardLeft;
        gameData.removedCardIDs = new List<int>(removedCardIDs);
        gameData.difficultyLevel = difficultyLevel;
        gameData.cardData = new List<CardData>();

        foreach (var card in cards)
        {
            if (card != null)
            {
                CardData cardData = new CardData
                {
                    id = card.ID,
                    flipped = card.Flipped,
                    spriteID = card.SpriteID
                };
                gameData.cardData.Add(cardData);
            }
            //else
            //{
            //    Debug.LogWarning("Card is null in SaveGame!");
            //}
        }

        string json = JsonUtility.ToJson(gameData);

        try
        {
            File.WriteAllText(filePath, json);
           
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public GameData LoadGame()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                
                return JsonUtility.FromJson<GameData>(json);
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
            }
        }
        

        return null;
    }



    // Reset the save data by deleting the save file
    public void ResetSaveData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
           
        }
        
    }
}
