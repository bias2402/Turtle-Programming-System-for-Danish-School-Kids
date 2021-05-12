using UnityEngine;

public class GameStartUp : MonoBehaviour {
    [SerializeField] private Sprite defaultAvatar;
    [SerializeField] private Sprite defaultBackground;

    void Start() {
        SaveAndLoad.LoadOptions();
        FindObjectOfType<MenuMethods>().SetBackground(defaultBackground);
        FindObjectOfType<MenuMethods>().SetAvatar(0);
        FindObjectOfType<MenuMethods>().SetAvatarPreview(defaultAvatar);
    }
}
