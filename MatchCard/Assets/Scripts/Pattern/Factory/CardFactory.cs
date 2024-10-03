using System.Collections.Generic;
using UnityEngine;

public class CardFactory
{
    private GameObject prefab;
    private Transform cardListParent;
    private RectTransform panel;
    private CardTextureData cardTextureData;
    private float padding;

    public CardFactory(GameObject prefab, Transform cardListParent, RectTransform panel, CardTextureData cardTextureData, float padding)
    {
        this.prefab = prefab;
        this.cardListParent = cardListParent;
        this.panel = panel;
        this.cardTextureData = cardTextureData;
        this.padding = padding;
    }

    public Card[] CreateCards(int rows, int cols, HashSet<int> removedCardIDs)
    {
        int totalCards = rows * cols;

        // Calculate if the grid is odd (for center skip logic)
        bool isOddGrid = (rows % 2 != 0) && (cols % 2 != 0);

        // Adjust array size only if the grid is odd
        Card[] cards = new Card[totalCards];

        // Remove all previous card game objects from parent
        foreach (Transform child in cardListParent)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Get panel dimensions
        float panelWidth = panel.rect.width;
        float panelHeight = panel.rect.height;

        // Calculate card size and spacing based on panel dimensions
        float cardWidth = (panelWidth - (padding * (cols + 1))) / cols;
        float cardHeight = (panelHeight - (padding * (rows + 1))) / rows;

        // Starting positions for the grid
        float startX = -((panelWidth - cardWidth) / 2) + padding;
        float startY = ((panelHeight - cardHeight) / 2) - padding;

        float curX = startX;
        float curY = startY;

        // Calculate the center card index only for odd grids
        int centerIndex = -1;
        if (isOddGrid)
        {
            centerIndex = (rows / 2) * cols + (cols / 2); // Middle row's middle column
        }

        int cardIndex = 0;

        // Create the card objects and position them
        for (int i = 0; i < rows; i++)
        {
            curX = startX;  // Reset X position for each row

            for (int j = 0; j < cols; j++)
            {
                int currentPosIndex = i * cols + j;

                // Skip the center card position for odd grids
                if (currentPosIndex == centerIndex)
                {
                    curX += cardWidth + padding;
                    continue;
                }

                // Skip already removed cards
                if (removedCardIDs.Contains(cardIndex))
                {
                    curX += cardWidth + padding;
                    cardIndex++;
                    continue; // Skip creating this card
                }

                // Instantiate the card prefab
                GameObject cardObject = GameObject.Instantiate(prefab, cardListParent);
                cardObject.transform.localScale = Vector3.one; // Ensure the scale is correct

                // Get Card component
                Card card = cardObject.GetComponent<Card>();

                // Check if card is successfully created
                if (card == null)
                {
                    Debug.LogError("Card component is null after instantiation!");
                    continue;
                }

                // Set card size based on calculated dimensions
                RectTransform cardRect = cardObject.GetComponent<RectTransform>();
                cardRect.sizeDelta = new Vector2(cardWidth, cardHeight);

                // Set card position
                cardObject.transform.localPosition = new Vector3(curX, curY, 0);

                // Set card ID
                cards[cardIndex] = card;
                cards[cardIndex].ID = cardIndex;

                Debug.Log($"Card created at index {cardIndex} with ID {cards[cardIndex].ID}");

                // Move X to the right for the next card
                curX += cardWidth + padding;
                cardIndex++;
            }

            // Move Y down for the next row
            curY -= (cardHeight + padding);
        }

        return cards;
    }






}
