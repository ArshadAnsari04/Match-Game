using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private int spriteID;
    private int id;
    private bool flipped;
    private bool turning;
    [SerializeField] private Image imgRef;

    // flip card animation
    // if changeSprite specified, will 90 degree, change to back/front sprite before flipping another 90 degree
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
        //change sprite and flip another 90degree
        if (changeSprite)
        {
            flipped = !flipped;
            ChangeSprite();
            StartCoroutine(FlipTheCard(transform, time, false));
        }
        else
            turning = false;
    }
    // perform a 180 degree flip
    public void Flip()
    {
        turning = true;
       
        StartCoroutine(FlipTheCard(transform, 0.25f, true));
    }
    // toggle front/back sprite
    private void ChangeSprite()
    {
        if (spriteID == -1 || imgRef == null) return;
        if (flipped)
            imgRef.sprite = CardGameManager.Instance.GetSprite(spriteID);
        else
            imgRef.sprite = CardGameManager.Instance.CardBack();
    }
 
    // set card to be active color
    public void Active()
    {
        if (imgRef)
            imgRef.color = Color.white;
    }
    // spriteID getter and setter
    public int SpriteID
    {
        set
        {
            spriteID = value;
            flipped = true;
            ChangeSprite();
        }
        get { return spriteID; }
    }
    // card ID getter and setter
    public int ID
    {
        set { id = value; }
        get { return id; }
    }
    // reset card default rotation
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        flipped = true;
    }
  
   
}