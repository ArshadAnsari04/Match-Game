using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : Singleton<CardGameManager>
{
    // UI and prefab references for card game
    [SerializeField] private GameObject prefab; // Card prefab
    [SerializeField] RectTransform cardParent; // Parent container for cards
    [SerializeField] private CardTextureData cardTextureData; // Card textures (front and back)
    [SerializeField] RectTransform panel; // Panel for card grid

   
    private Card[] cards; // Array of card objects
  
    private void Start()
    {
        // Hide the panel at the start of the game
        panel.gameObject.SetActive(false);
    }

    // Initializes the card game
    public void StartCardGame()
    {
        GameStateManager.Instance.SetState(new PlayingState()); // Set game state to playing
      

        panel.gameObject.SetActive(true); // Display the card panel

        // Using the CardFactory to create and set up cards
        var cardFactory = new CardFactory(prefab, cardParent, panel);
        cards = cardFactory.CreateCards(5, 5); // Create a 5x5 grid of cards
      
        AllocateSpritesToCards(); // Assign sprites to cards
        StartCoroutine(HideFace()); // Hide card faces after showing them briefly
    }

   
    // Allocate random sprites to cards ensuring each sprite appears twice
    private void AllocateSpritesToCards()
    {
        int[] selectedIDs = new int[cards.Length / 2]; // Array to store IDs of selected sprites
        for (int i = 0; i < cards.Length / 2; i++)
        {
            // Select a random sprite ID and ensure no duplicates
            int value = Random.Range(0, cardTextureData.frontTextures.Count - 1);
            for (int j = i; j > 0; j--)
            {
                if (selectedIDs[j - 1] == value)
                    value = (value + 1) % cardTextureData.frontTextures.Count;
            }
            selectedIDs[i] = value; // Store selected sprite ID
        }

        // Reset all cards
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Active(); // Activate the card
            cards[i].SpriteID = -1; // Reset sprite ID
            cards[i].ResetRotation(); // Reset the rotation
        }

        // Assign each sprite to two cards
        for (int i = 0; i < cards.Length / 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                // Randomly select an available card and assign the sprite ID
                int value = Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedIDs[i]; // Assign sprite ID
            }
        }
    }

    // Return sprite based on ID
    public Sprite GetSprite(int spriteId)
    {
        return cardTextureData.frontTextures[spriteId];
    }

    // Return the back sprite of the card
    public Sprite CardBack()
    {
        return cardTextureData.backTexture;
    }
    // Coroutine to briefly show card faces, then hide them
    private IEnumerator HideFace()
    {
        yield return new WaitForSeconds(0.3f); // Wait for 0.3 seconds
        foreach (var card in cards)
        {
            card.Flip(); // Flip all cards to hide their faces
        }
        yield return new WaitForSeconds(0.5f); // Wait before resuming
    }
}
