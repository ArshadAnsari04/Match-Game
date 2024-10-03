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

            SetDifficulty(gameData.difficultyLevel);
            
            var cardFactory = new CardFactory(prefab, cardParent, panel, cardTextureData, 10);
            var removedCardIDsHashSet = new HashSet<int>(gameData.removedCardIDs);
            cards = cardFactory.CreateCards(rows, columns, removedCardIDsHashSet);
            spriteSelected = -1;

            foreach (var cardData in gameData.cardData)
            {
                if (cardData.id < 0 || cardData.id >= cards.Length) continue;

                Card card = cards[cardData.id];
                if (card == null) continue;

                if (removedCardIDsHashSet.Contains(card.ID))
                {
                    Destroy(card.gameObject);
                    continue;
                }

                card.RestoreState(cardData.flipped, cardData.spriteID);
                if (!cardData.flipped) card.Active();
                if (cardData.flipped) CardClicked(cardData.spriteID, cardData.id);

                Image cardImage = card.GetComponent<Image>();
                cardImage.sprite = cardData.flipped ? GetSprite(cardData.spriteID) : CardBack();
            }

            UIManager.Instance.UpdateScore(score);
            UIManager.Instance.UpdateMatchCount(matchCount);
        }
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

    

    #endregion

    #region Card Management

    public void CardClicked(int spriteId, int cardId)
    {
        if (cardId < 0 || cardId >= cards.Length || cards[cardId] == null) return;

        if (removedCardIDs.Contains(cards[cardId].ID)) return;

        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
            cards[cardId].Active();
        }
        else
        {
            if (spriteSelected == spriteId)
            {
                removedCardIDs.Add(cards[cardSelected].ID);
                removedCardIDs.Add(cards[cardId].ID);
                matchCount++;
                score += 1;
                UpdateScoreAndMatchCount();
                Destroy(cards[cardSelected].gameObject);
                Destroy(cards[cardId].gameObject);
                cardLeft -= 2;
                CheckGameWin();
            }
            else
            {
                StartCoroutine(FlipBack(cards[cardSelected], cards[cardId]));
            }

            spriteSelected = -1;
            cardSelected = -1;
        }
    }

    private IEnumerator FlipBack(Card firstCard, Card secondCard)
    {
        yield return new WaitForSeconds(0.5f);
        firstCard.Flip();
        secondCard.Flip();
    }

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
