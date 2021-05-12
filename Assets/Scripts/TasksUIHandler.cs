using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TasksUIHandler : MonoBehaviour {
    public Text description;
    public Image taskObjective;
    public Text inTaskDescription;
    public Image inTaskObjective;
    public string programmingMode;

    [SerializeField] private GameObject[] categories;
    [SerializeField] private GameObject objectDescriptionArea;
    [SerializeField] private GameObject taskSelectionArea;
    [SerializeField] private GameObject taskDescriptionArea;
    [SerializeField] private GameObject inTaskDescriptionArea;
    [SerializeField] private GameObject blockUI;
    [SerializeField] private GameObject consoleUI;
    [SerializeField] private GameObject categoryMenu;
    [SerializeField] private GameObject inTaskDescriptionAreaButton;
    [SerializeField] private Image blockButtonImage;
    [SerializeField] private Image consoleButtonImage;

    private UIInformationHolder infoHolder;

    //Runs at start before update, and make sure the task select area is shown only
    void Start() {
        infoHolder = GetComponent<UIInformationHolder>();
        ToggleSelectionArea();
        programmingMode = "block";
        blockButtonImage.color = Color.gray;
        consoleButtonImage.color = Color.white;
    }

    //Reset the block area, so it's again.
    public void ResetTheBlockArea() {
        foreach(GameObject socket in infoHolder.sockets) {
            if (socket.transform.parent.name.Contains("Block")) {
                socket.GetComponent<UIActions>().PickupBlock(true);
                infoHolder.blockMover.holdingBlock = false;
                infoHolder.blockMover.currentBlock.GetComponent<UIActions>().DeleteCurrentBlock();
            }
        }
    }

    //Toggle the chosen category and disabling the category menu
    public void ToggleCategory(int index) {
        categoryMenu.SetActive(false);
        categories[index].SetActive(true);
    }

    //Show the task selection area
    public void ToggleSelectionArea() {
        ResetTheBlockArea();
        taskDescriptionArea.SetActive(false);
        infoHolder.playArea.gameObject.SetActive(false);
        blockUI.SetActive(false);
        consoleUI.SetActive(false);
        taskSelectionArea.SetActive(true);
        categoryMenu.SetActive(true);
        objectDescriptionArea.SetActive(false);
        for (int i = 0; i < categories.Length; i++) {
            categories[i].SetActive(false);
        }
    }

    //Show the task description area
    public void ToggleDescriptionArea() {
        taskSelectionArea.SetActive(false);
        infoHolder.playArea.gameObject.SetActive(false);
        blockUI.SetActive(false);
        consoleUI.SetActive(false);
        taskDescriptionArea.SetActive(true);
        objectDescriptionArea.SetActive(false);
    }

    //Show the coding area
    public void ToggleGameUI(bool showIngameDescription) {
        taskSelectionArea.SetActive(false);
        taskDescriptionArea.SetActive(false);
        objectDescriptionArea.SetActive(true);
        if (showIngameDescription) {
            inTaskDescriptionAreaButton.SetActive(true);
        } else {
            inTaskDescriptionAreaButton.SetActive(false);
        }

        if (programmingMode == "block") {
            blockUI.SetActive(true);
            infoHolder.playArea.gameObject.SetActive(false);
        }

        if (programmingMode == "console") {
            consoleUI.SetActive(true);
            infoHolder.playArea.gameObject.SetActive(true);
        }
    }

    //Show the Console coding area
    public void SetProgrammingMode(string mode) {
        if (mode == "block") {
            blockButtonImage.color = Color.gray;
            consoleButtonImage.color = Color.white;
            programmingMode = mode;
        }

        if (mode == "console") {
            blockButtonImage.color = Color.white;
            consoleButtonImage.color = Color.gray;
            programmingMode = mode;
        }
    }

    //Show the description while in a task
    public void ToggleInTaskDescription(bool toggleOn) {
        if (toggleOn) {
            infoHolder.menuBackButton.SetActive(false);
            inTaskDescriptionArea.SetActive(true);
        } else {
            infoHolder.menuBackButton.SetActive(true);
            inTaskDescriptionArea.SetActive(false);
        }
    }

    public void ToggleConsoleDictionary(bool toggleOn) {
        if (toggleOn) {
            infoHolder.consoleDictionary.SetActive(true);
        } else {
            infoHolder.consoleDictionary.SetActive(false);
        }
    }

    public void ToggleConsoleDictionaryExamlpes(bool toggleOn) {
        if (toggleOn) {
            infoHolder.consoleDictionaryExamples.SetActive(true);
            infoHolder.consoleDictionary.SetActive(false);
        } else {
            infoHolder.consoleDictionaryExamples.SetActive(false);
            infoHolder.consoleDictionary.SetActive(true);
        }
    }

    //Go back to the menu
    public void GoToMenu() {
        SceneManager.LoadScene("main");
    }
}