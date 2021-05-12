using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//This script merely holds all the information we need to access. CAN'T be static, 
//since it'll need references to access, and therefore needs an object to do so with.
//But now it also contains methods... Yeah, didn't hold that very clean in the long run.
public class UIInformationHolder : MonoBehaviour {
    public float finalSolution;
    public string taskType;
    
    public GameObject[] sockets;
    public GameObject[] editCogs;
    public GameObject[] copyButtons;
    public Texture[] borders;
    public Transform socketParent;
    public Text loopInput;
    public BlockMovement blockMover;
    public GameObject blockArea;
    public GameObject socketsArea;
    public GameObject feedbackArea;
    public GameObject editBlockPanel;
    public Dropdown editBlockDropdown;
    public int editBlockValue;
    public int lastCogwheelUsed;
    public int lastCopyButtonUsed;
    public Text descriptionArea;
    public string descriptionAreaDefault;
    public GameObject menuBackButton;
    public GameObject blockAreaBackButton;
    public RectTransform playArea;
    public GameObject startPositionArea;
    public GameObject consoleDictionary;
    public GameObject consoleDictionaryExamples;

    public Image[] backgroundObjects;

    public GameObject[] avatarsPrefabs;
    public GameObject activeAvatar;
    public Vector3 avatarStartPosition;
    public Quaternion avatarStartRotation;
    public int avatarChosen = 0;

    public SoundEffectHandler soundEffectHandler;

    public List<string> movementValues;
    public List<string> rotateValues;
    public List<string> colorNames;

    private bool selectingStartPosition = false;

    void Awake() {
        descriptionAreaDefault = descriptionArea.text;
        foreach (Image img in backgroundObjects) {
            img.sprite = Options.background;
        }
        Debug.Log(Options.avatarIndex);
        GameObject avatar = Instantiate(avatarsPrefabs[Options.avatarIndex], playArea);
        activeAvatar = avatar;
        avatar.GetComponent<CharacterHandler>().infoHolder = this;
        avatarStartPosition = activeAvatar.GetComponent<RectTransform>().localPosition;
        avatarStartRotation = activeAvatar.GetComponent<RectTransform>().localRotation;
    }

    //Play the sound from the parameter
    public void PlaySound(AudioClip soundClip) {
        soundEffectHandler.SetAndPlaySoundClip(soundClip);
    }

    public void ChangeWindow(string windowToShow) {
        if (FindObjectOfType<TasksUIHandler>().programmingMode == "block") {
            switch (windowToShow) {
                case "game":
                    menuBackButton.SetActive(false);
                    blockAreaBackButton.SetActive(true);
                    feedbackArea.SetActive(true);
                    socketsArea.SetActive(false);
                    blockArea.SetActive(false);
                    playArea.gameObject.SetActive(true);
                    break;
                case "edit":
                    menuBackButton.SetActive(true);
                    blockAreaBackButton.SetActive(false);
                    feedbackArea.SetActive(false);
                    socketsArea.SetActive(true);
                    blockArea.SetActive(true);
                    playArea.gameObject.SetActive(false);
                    break;
            }
        }
    }

    //This is used to enter the set-avatar mode
    public void ChooseAvatarStartPosition() {
        ChangeWindow("game");
        activeAvatar.GetComponent<CharacterHandler>().resetCharacterNow = true;
        startPositionArea.SetActive(true);
    }

    public void ChooseThisPositionForAvatar() {
        ChangeWindow("edit");
        startPositionArea.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 5000);
        avatarStartPosition = hit.point;
        activeAvatar.transform.position = avatarStartPosition;
        startPositionArea.SetActive(false);

        avatarStartPosition = activeAvatar.transform.localPosition;
        avatarStartRotation = activeAvatar.transform.localRotation;

        /*foreach (Transform lineChild in activeAvatar.transform.parent) {
            if (lineChild.name.Contains("UI Line")) {
                Destroy(lineChild.gameObject);
            }
        }*/
    }
}