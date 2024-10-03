using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private int spriteID;
    private int id;
    private bool flipped;
    private bool turning;
    [SerializeField]
    private Image img;

    // Flip card animation coroutine
    private IEnumerator FlipTheCard(Transform thisTransform, float time, bool changeSprite)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
        float rate = 1.0f / time;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        if (changeSprite)
        {
            flipped = !flipped;
            UpdateCardSprite(); // Update sprite to show front or back
            StartCoroutine(FlipTheCard(thisTransform, time, false)); // Rotate back
        }
        else
        {
            turning = false;
        }
    }

    // Public method to flip the card
    public void Flip()
    {
        if (turning || !CanFlip()) return;
        turning = true;
        SoundManager.Instance.PlayFlipSound();
        StartCoroutine(FlipTheCard(transform, 0.25f, true));
    }

    // Check if the card can be flipped (not matched)
    private bool CanFlip()
    {
        return !CardGameManager.Instance.IsCardMatched(id);
    }

    // Update the sprite based on the flipped state
    private void UpdateCardSprite()
    {
        if (spriteID == -1 || img == null) return;
        img.sprite = flipped ? CardGameManager.Instance.GetSprite(spriteID) : CardGameManager.Instance.CardBack();
    }

    // Fade out the card and deactivate it
    public void Inactive()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float rate = 1.0f / 2.5f;
        float t = 0.0f;
        Color originalColor = img.color;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            img.color = Color.Lerp(originalColor, Color.clear, t);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // Reset the card to be visible
    public void Active()
    {
        if (img)
        {
            img.color = Color.white;
            gameObject.SetActive(true);
        }
    }

    // Show the back of the card
    public void ShowBack()
    {
        flipped = false;
        UpdateCardSprite();
    }

    // Property for SpriteID
    public int SpriteID
    {
        set
        {
            spriteID = value;
            flipped = true;
            UpdateCardSprite();
        }
        get { return spriteID; }
    }

    // Property for Card ID
    public int ID
    {
        set { id = value; }
        get { return id; }
    }

    // Reset the card's rotation
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        flipped = false;
        UpdateCardSprite();
    }

    // Property to check if the card is flipped
    public bool Flipped
    {
        get { return flipped; }
    }

    // Card on-click event
    public void CardBtn()
    {
        if (flipped || turning) return;
        Flip();
        StartCoroutine(SelectionEvent());
    }

    // Inform the manager that the card was selected
    private IEnumerator SelectionEvent()
    {
        yield return new WaitForSeconds(0.5f);
        CardGameManager.Instance.CardClicked(spriteID, id);
    }

    // Restore the flipped state of the card after loading the game
    public void RestoreState(bool isFlipped, int spriteId)
    {
        SpriteID = spriteId;

        if (CardGameManager.Instance.IsCardMatched(id))
        {
            Inactive(); // Fade out and deactivate if matched
        }
        else
        {
            flipped = isFlipped; // Restore flipped state
            UpdateCardSprite(); // Update the sprite without animation
           // CardGameManager.Instance.CardClicked(spriteID, id);
            //StartCoroutine(SelectionEvent());
        }
    }

    
}
