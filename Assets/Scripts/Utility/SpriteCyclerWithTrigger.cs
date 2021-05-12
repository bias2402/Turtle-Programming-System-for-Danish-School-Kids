using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpriteCyclerWithTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Sprite[] spritesForAnimation;
    [SerializeField] private Image targetImage;
    [SerializeField] private float animationSpeed = 0.1f;

    public void OnPointerEnter(PointerEventData eventData) {
        StartCoroutine(PlayAnimation());
    }

    public void OnPointerExit(PointerEventData eventData) {
        StartCoroutine(PlayAnimationReverse());
    }

    //Iterate through the array of sprites, first incrementing through them, then decrementing through them, so it looks like the sprite moves forward and back
    IEnumerator PlayAnimation() {
        for (int i = 0; i < spritesForAnimation.Length; i++) {
            targetImage.sprite = spritesForAnimation[i];
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    IEnumerator PlayAnimationReverse() {
        for (int i = spritesForAnimation.Length - 1; i > 0; i--) { //Lenght of the array minus one, since the lenght isn't equal the highest index
            if (i < 0) {
                targetImage.sprite = spritesForAnimation[0];
                break;
            }
            targetImage.sprite = spritesForAnimation[i];
            yield return new WaitForSeconds(animationSpeed);
        }
        targetImage.sprite = spritesForAnimation[0]; //Reset the sprite to the first in the array
    }
}
