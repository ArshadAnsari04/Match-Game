using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTextureData", menuName = "CardGame/Card Texture Data")]
public class CardTextureData : ScriptableObject
{
    public Sprite backTexture;        // Shared texture for the back of all cards
    public List<Sprite> frontTextures; // List of front textures for the cards
}
