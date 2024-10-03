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
        Debug.Log("Save file path: " + filePath);

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
            Debug.Log("Created save directory: " + directory);
        }
    }
   

    // Save the game data
    //public void SaveGame(int score, int matchCount, Card[] cards, int cardLeft, HashSet<int> removedCardIDs, int difficultyLevel)
    //{
    //    GameData data = new GameData
    //    {
    //        score = score,
    //        matchCount = matchCount,
    //        cardLeft = cardLeft,
    //        cardData = new List<CardData>(),
    //        removedCardIDs =new List<int>(removedCardIDs),
    //        difficultyLevel = difficultyLevel // Save the difficulty level
    //    };

    //    foreach (var card in cards)
    //    {
    //        data.cardData.Add(new CardData
    //        {
    //            id = card.ID,
    //            spriteID = card.SpriteID,
    //            flipped = card.Flipped
    //        });
    //    }
    //    //GameData gameData = new GameData();
    //    //gameData.score = score;
    //    //gameData.matchCount = matchCount;
    //    //gameData.cardLeft = cardLeft;
    //    //gameData.removedCardIDs = new HashSet<int>(removedCardIDs);
    //    //gameData.difficultyLevel = difficultyLevel; // Save difficulty level

    //    //List<CardData> cardDataList = new List<CardData>();
    //    //foreach (var card in cards)
    //    //{
    //    //    cardDataList.Add(new CardData { id = card.ID, flipped = card.Flipped, spriteID = card.SpriteID });
    //    //}
    //    //gameData.cardData = cardDataList;
    //    string json = JsonUtility.ToJson(data);

    //    try
    //    {
    //        File.WriteAllText(filePath, json);
    //        Debug.Log("Game saved successfully to: " + filePath);
    //    }
    //    catch (IOException e)
    //    {
    //        Debug.LogError("Failed to save game: " + e.Message);
    //    }
    //}
    //public void SaveGame(int score, int matchCount, Card[] cards, int cardLeft, HashSet<int> removedCardIDs, int difficultyLevel)
    //{
    //    GameData gameData = new GameData();
    //    gameData.score = score;
    //    gameData.matchCount = matchCount;
    //    gameData.cardLeft = cardLeft;
    //    gameData.removedCardIDs = new HashSet<int>(removedCardIDs);
    //    gameData.difficultyLevel = difficultyLevel; // Ensure the difficulty is saved

    //    List<CardData> cardDataList = new List<CardData>();
    //    foreach (var card in cards)
    //    {
    //        cardDataList.Add(new CardData { id = card.ID, flipped = card.Flipped, spriteID = card.SpriteID });
    //    }
    //    gameData.cardData = cardDataList;
    //    string json = JsonUtility.ToJson(gameData);

    //    try
    //    {
    //        File.WriteAllText(filePath, json);
    //        Debug.Log("Game saved successfully to: " + filePath);
    //    }
    //    catch (IOException e)
    //    {
    //        Debug.LogError("Failed to save game: " + e.Message);
    //    }
    //   // SaveLoadSystem.Instance.Save(gameData);
    //}
    public void SaveGame(int score, int matchCount, Card[] cards, int cardLeft, HashSet<int> removedCardIDs, int difficultyLevel)
    {
        // Debugging logs to track parameter states
        //Debug.Log($"Saving Game: Score: {score}, MatchCount: {matchCount}, CardLeft: {cardLeft}, RemovedCardIDs Count: {removedCardIDs?.Count ?? 0}, Difficulty: {difficultyLevel}");

        // Check for null parameters
        if (cards == null)
        {
            Debug.LogError("Cards array is null. Cannot save game.");
            return; // Exit the method to prevent further errors
        }

        if (removedCardIDs == null)
        {
            Debug.LogError("RemovedCardIDs HashSet is null. Cannot save game.");
            return; // Exit the method to prevent further errors
        }

        //GameData gameData = new GameData
        //{
        //    score = score,
        //    matchCount = matchCount,
        //    cardLeft = cardLeft,
        //    removedCardIDs = new HashSet<int>(removedCardIDs), // Safe to use now
        //    difficultyLevel = difficultyLevel
        //};

        //List<CardData> cardDataList = new List<CardData>();

        //// Loop through the cards and collect their data
        //foreach (var card in cards)
        //{
        //    // Check if the card is not null
        //    if (card != null)
        //    {
        //        cardDataList.Add(new CardData { id = card.ID, flipped = card.Flipped, spriteID = card.SpriteID });
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Encountered a null card in the cards array.");
        //    }
        //}

        //gameData.cardData = cardDataList;



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
            else
            {
                Debug.LogWarning("Card is null in SaveGame!");
            }
        }



        //GameData gameData = new GameData
        //{
        //    score = score,
        //    matchCount = matchCount,
        //    cardLeft = cardLeft,
        //    removedCardIDs = new HashSet<int>(removedCardIDs),
        //    difficultyLevel = difficultyLevel,
        //    cardData = new List<CardData>()
        //};

        //foreach (var card in cards)
        //{
        //    CardData cardData = new CardData
        //    {
        //        id = card.ID,
        //        spriteID = card.SpriteID,
        //        flipped = card.Flipped // Save whether the card is flipped
        //    };
        //    gameData.cardData.Add(cardData);
        //}

        string json = JsonUtility.ToJson(gameData);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Game saved successfully to: " + filePath);
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
                Debug.Log("Game loaded successfully from: " + filePath);
                return JsonUtility.FromJson<GameData>(json);
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("No save file found at: " + filePath);
        }

        return null;
    }



    // Reset the save data by deleting the save file
    public void ResetSaveData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Save data reset successfully.");
        }
        else
        {
            Debug.Log("No save data to reset.");
        }
    }
}
