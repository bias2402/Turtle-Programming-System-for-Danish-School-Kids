using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuMethods : MonoBehaviour {
    [SerializeField] private Slider volume = null;
    [SerializeField] private GameObject options = null;
    [SerializeField] private GameObject menu = null;
    [SerializeField] private GameObject[] spriteMenues;
    [SerializeField] private Text spriteMenuButtonText;
    [SerializeField] private Text backgroundMenuButtonText;
    [SerializeField] private Image background;
    [SerializeField] private Image avatarPreview;

    void Start() {
        ViewMenu();
    }

    public void SaveNewSettings() {
        Options.SetOptions((int)volume.value);
        //SaveAndLoad.SaveOptions();
    }

    public void ViewOptions() {
        options.SetActive(true);
        menu.SetActive(false);
        for (int i = 0; i < spriteMenues.Length; i++) {
            spriteMenues[i].SetActive(false);
        }
    }

    public void ViewMenu() {
        options.SetActive(false);
        menu.SetActive(true);
        for (int i = 0; i < spriteMenues.Length; i++) {
            spriteMenues[i].SetActive(false);
        }
    }

    public void ChangeScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public void Exit() {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void ViewSpriteMenu(int index) {
        spriteMenues[index].SetActive(true);
    }

    public void SetBackground(Sprite spr) {
        Options.SetBackground(spr);
        background.sprite = spr;
        backgroundMenuButtonText.text = spr.name;
        ViewMenu();
    }

    public void SetAvatarPreview(string name) {
        spriteMenuButtonText.text = name;
    }

    public void SetAvatarPreview(Sprite sprite) {
        avatarPreview.sprite = sprite;
    }

    public void SetAvatar(int index) {
        Options.SetAvatar(index);
        ViewMenu();
    }

    public void SendMail() {
        MailingSystem.SendEmailToClient();
    }
}