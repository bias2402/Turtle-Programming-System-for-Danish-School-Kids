using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCycler : MonoBehaviour {
    public Sprite[] spritesForAnimation;
    [Tooltip("This sets the type of animation: use 'duo' for two images only; 'series' for multiple images that should run through and restart; " +
        "'cycle' if the images should run through and reverse back through")]
    public string animationType;

    [SerializeField] private Image targetImage;
    [SerializeField] private float animationSpeed = 0.1f;
    [SerializeField] private bool startSelf = false;

    void OnEnable() {
        if (startSelf) {
            StartAnimationSelf();
        }
    }

    void StartAnimationSelf() {
        switch (animationType) {
            case "duo":
                StartCoroutine(PlayAnimationDuo());
                break;
            case "series":
                StartCoroutine(PlayAnimationSeries());
                break;
            case "cycle":
                StartCoroutine(PlayAnimationCycle());
                break;
        }
    }

    public void StartAnimation() {
        switch (GetComponent<AvatarConstructor>().animationType.ToLower()) {
            case "duo":
                StartCoroutine(PlayAnimationDuo());
                break;
            case "series":
                StartCoroutine(PlayAnimationSeries());
                break;
            case "cycle":
                StartCoroutine(PlayAnimationCycle());
                break;
        }
    }

    IEnumerator PlayAnimationDuo() {
        Debug.Log("Running by " + gameObject.name);
        int currentDuoSprite = 0;
        targetImage.sprite = spritesForAnimation[currentDuoSprite];
        currentDuoSprite++;
        yield return new WaitForSeconds(animationSpeed);
        Debug.Log("Running by 2 " + gameObject.name);
        targetImage.sprite = spritesForAnimation[currentDuoSprite];
        yield return new WaitForSeconds(animationSpeed);
        Debug.Log("Running by 3 " + gameObject.name);
        StartCoroutine(PlayAnimationDuo());
    }

    IEnumerator PlayAnimationSeries() {
        for (int i = 0; i < spritesForAnimation.Length; i++) {
            targetImage.sprite = spritesForAnimation[i];
            yield return new WaitForSeconds(animationSpeed);
        }
        targetImage.sprite = spritesForAnimation[0]; //Reset the sprite to the first in the array
        StartCoroutine(PlayAnimationSeries()); //Restart animation
    }

    //Iterate through the array of sprites, first incrementing through them, then decrementing through them, so it looks like the sprite moves forward and back
    IEnumerator PlayAnimationCycle() {
        for (int i = 0; i < spritesForAnimation.Length; i++) {
            targetImage.sprite = spritesForAnimation[i];
            yield return new WaitForSeconds(animationSpeed);
        }
        for (int i = spritesForAnimation.Length - 1; i > 0; i--) { //Lenght of the array minus one, since the lenght isn't equal the highest index
            if (i < 0) {
                targetImage.sprite = spritesForAnimation[0];
                break;
            }
            targetImage.sprite = spritesForAnimation[i];
            yield return new WaitForSeconds(animationSpeed);
        }
        targetImage.sprite = spritesForAnimation[0]; //Reset the sprite to the first in the array
        StartCoroutine(PlayAnimationCycle()); //Restart animation
    }
}
