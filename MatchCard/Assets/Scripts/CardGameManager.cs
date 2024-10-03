using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : Singleton<CardGameManager>
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private RectTransform cardParent;
    [SerializeField] private CardTextureData cardTextureData;
    [SerializeField] private RectTransform panel;

    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private Card[] cards;
    private int score;
    private int matchCount;
    private int rows = 2;
    private int columns = 2;
    private HashSet<int> removedCardIDs = new HashSet<int>();

    private void Start()
    {
        panel.gameObject.SetActive(false);
    }

    #region Game Initialization

    public void StartCardGame(bool loadPreviousGame = false)
    {
        GameStateManager.Instance.SetState(new PlayingState());
        UIManager.Instance.ResetUI();
        panel.gameObject.SetActive(true);
        var cardFactory = new CardFactory(prefab, cardParent, panel, cardTextureData, 10);
        cards = cardFactory.CreateCards(rows, columns, new HashSet<int>());

        if (loadPreviousGame)
        {
           
            LoadGame();
        }
        else
        {
            InitializeGame();
            AllocateSpritesToCards();
            StartCoroutine(HideFace());
        }
    }

    private void InitializeGame()
    {
        matchCount = 0;
        score = 0;
        spriteSelected = -1;
        cardSelected = -1;
        cardLeft = cards.Length;
        removedCardIDs.Clear();
    }

    #endregion

    #region Game State Management

    private void LoadGame()
    {
        var gameData = SaveLoadSystem.Instance.LoadGame();
        if (gameData != null)
        {
            score = gameData.score;
            matchCount = gameData.matchCount;
            cardLeft = gameData.cardLeft;

            SetDifficulty(gameData.difficultyLevel); // Set the difficulty level

            var cardFactory = new CardFactory(prefab, cardParent, panel, cardTextureData, 10);
            var removedCardIDsHashSet = new HashSet<int>(gameData.removedCardIDs);
            cards = cardFactory.CreateCards(rows, columns, removedCardIDsHashSet);
            spriteSelected = -1;

            // Restore each card's state
            foreach (var cardData in gameData.cardData)
            {
                // Check for valid ID
                if (cardData.id < 0 || cardData.id >= cards.Length)
                {
                    Debug.LogError($"Card at index {cardData.id} is out of bounds!");
                    continue; // Skip invalid cards
                }

                Card card = cards[cardData.id];

                // Check for null card
                if (card == null)
                {
                    Debug.LogError($"Card at index {cardData.id} is null!");
                    continue; // Skip null cards
                }

                // If this card was removed, destroy it
                if (removedCardIDsHashSet.Contains(card.ID))
                {
                    Destroy(card.gameObject); // Remove matched card
                    continue; // Skip further processing for this card
                }

                // Restore the card's sprite and flipped state
                card.RestoreState(cardData.flipped, cardData.spriteID);
                if (!cardData.flipped) // Only activate if the card isn't flipped
                {
                    card.Active(); // Ensure the card is active if not matched
                }

                // Cache the Image component for better performance
                Image cardImage = card.GetComponent<Image>();
                // Set the sprite based on the flipped state
                cardImage.sprite = cardData.flipped ? GetSprite(cardData.spriteID) : CardBack();
            }

            // Update UI elements with the loaded values
            UIManager.Instance.UpdateScore(score);
            UIManager.Instance.UpdateMatchCount(matchCount);
        }
        else
        {
            Debug.Log("Game data is null! Unable to load the game.");
        }
    }




    private IEnumerator FlipRemainingCardsToBack()
    {
        yield return new WaitForSeconds(0.3f); // Add a small delay for smoother animation

        foreach (var card in cards)
        {
            if (card != null && !removedCardIDs.Contains(card.ID) && card.Flipped)
            {
                // Ensure the card flips back to show the back image
                card.Flip();  // This will flip it back to the back image
            }
        }

        yield return new WaitForSeconds(0.5f); // Wait for the animations to complete
    }




    private void CheckGameWin()
    {
        if (cardLeft == 0)
        {
          
            GameStateManager.Instance.SetState(new GameWinState());
            SaveLoadSystem.Instance.ResetSaveData();
        }
    }

    public void GameOver()
    {
        
        SaveLoadSystem.Instance.ResetSaveData();
        GameStateManager.Instance.SetState(new GameOverState());
    }

    public void SaveGame()
    {
        SaveLoadSystem.Instance.SaveGame(score, matchCount, cards, cardLeft, removedCardIDs, GetCurrentDifficulty());
       
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        
    }

    #endregion

    #region Card Management

    public void CardClicked(int spriteId, int cardId)
    {
        // Ensure cardId is within the bounds of the cards array and not null
        if (cardId < 0 || cardId >= cards.Length || cards[cardId] == null)
        {
            Debug.LogError($"Invalid card ID: {cardId}");
            return; // Exit if the card is invalid
        }

        // Ignore clicks on already matched (removed) cards
        if (removedCardIDs.Contains(cards[cardId].ID))
            return;

        // First card selected
        if (spriteSelected == -1)
        {
           
            // Set the first selected card
            spriteSelected = spriteId;
            cardSelected = cardId;
            cards[cardId].Active(); // Activate the clicked card
        }
        else
        {
           

            // Check if the two cards match
            if (spriteSelected == spriteId)
            {
                // Cards match: add them to the removed set and destroy them
                removedCardIDs.Add(cards[cardSelected].ID);
                removedCardIDs.Add(cards[cardId].ID);

                matchCount++;
                score += 1;

                // Update score and match count in the UI
                UpdateScoreAndMatchCount();

                // Destroy the matched cards
                Destroy(cards[cardSelected].gameObject);
                Destroy(cards[cardId].gameObject);

                // Decrease the count of remaining cards
                cardLeft -= 2;

                // Check if all cards have been matched (game win)
                CheckGameWin();
            }
            else
            {
                // Cards don't match: flip them back after a delay
                StartCoroutine(FlipBack(cards[cardSelected], cards[cardId]));
            }

            // Reset for the next card selection
            spriteSelected = -1;
            cardSelected = -1;
        }
    }





    private IEnumerator FlipBack(Card firstCard, Card secondCard)
    {
        // Check if either card is null
        if (firstCard == null)
        {
            Debug.LogError("First card is null!");
            yield break; // Exit the coroutine early
        }

        if (secondCard == null)
        {
            Debug.LogError("Second card is null!");
            yield break; // Exit the coroutine early
        }

        yield return new WaitForSeconds(0.5f);

        // Flip the cards back
        firstCard.Flip();
        secondCard.Flip();
    }

    //private IEnumerator FlipBack(Card firstCard, Card secondCard)
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    firstCard.Flip();
    //    secondCard.Flip();
    //}

    public bool IsCardMatched(int cardId)
    {
        return removedCardIDs.Contains(cardId);
    }

    private void UpdateScoreAndMatchCount()
    {
        SoundManager.Instance.PlayMatchSound();
        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateMatchCount(matchCount);
    }

    private void AllocateSpritesToCards()
    {
        int[] selectedIDs = new int[cards.Length / 2];

        for (int i = 0; i < cards.Length / 2; i++)
        {
            int value = Random.Range(0, cardTextureData.frontTextures.Count);
            for (int j = i; j > 0; j--)
            {
                if (selectedIDs[j - 1] == value)
                    value = (value + 1) % cardTextureData.frontTextures.Count;
            }
            selectedIDs[i] = value;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }

        for (int i = 0; i < cards.Length / 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedIDs[i];
            }
        }
    }

    public Sprite GetSprite(int spriteId)
    {
        return cardTextureData.frontTextures[spriteId];
    }

    public Sprite CardBack()
    {
        return cardTextureData.backTexture;
    }

    private IEnumerator HideFace()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (var card in cards)
        {
            if (!removedCardIDs.Contains(card.ID))
            {
                card.Flip();
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    #region Difficulty Management

    public void OnDifficultyChange(TMP_Dropdown difficultyDropdown)
    {
        int selectedDifficulty = difficultyDropdown.value;
        SetDifficulty(selectedDifficulty);
    }

    private void SetDifficulty(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 0:
                rows = 2;
                columns = 2;
                break;
            case 1:
                rows = 2;
                columns = 3;
                break;
            case 2:
                rows = 5;
                columns = 5;
                break;
        }
        Debug.Log($"Difficulty set to {difficultyLevel} (Rows: {rows}, Columns: {columns})");
    }

    public int GetCurrentDifficulty()
    {
        if (rows == 2 && columns == 2) return 0;
        if (rows == 2 && columns == 3) return 1;
        if (rows == 5 && columns == 5) return 2;
        return -1;
    }

    #endregion
}
