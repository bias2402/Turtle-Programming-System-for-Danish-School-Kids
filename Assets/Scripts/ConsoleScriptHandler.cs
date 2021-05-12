using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System;
using System.Linq;

public class ConsoleScriptHandler : MonoBehaviour {
    public InputField consoleInput;
    public CharacterHandler avatar;

    [SerializeField] private GameObject sendButton;
    [SerializeField] private Image autoSendImage;
    [SerializeField] private GameObject console;
    [SerializeField] private GameObject previousCommandPanel;
    [SerializeField] private Text previousCommandText;
    [SerializeField] private GameObject showPreviousCommandButton;
    [SerializeField] private GameObject showConsoleButton;
    [SerializeField] private GameObject optionsUI;

    private List<string> playerInputArray = new List<string>();
    private List<string> commandHistory = new List<string>();
    private string loopList = "";
    private List<string> loopListArray = new List<string>();
    private bool readAuto = false;
    private bool loopOn = false;
    private float loopCount = 0.0f;

    private bool optionsUIBool = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (!optionsUIBool)
            {
                optionsUI.SetActive(true);
                optionsUIBool = true;
            } else {
                optionsUI.SetActive(false);
                optionsUIBool = false;
            }
        }

        if (readAuto) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                ReadConsoleInput();
            }
        }
        consoleInput.ActivateInputField(); //Sets focus on the input field
    }


    public void SetReadingOnAuto() {
        ReadConsoleInput();
        if (readAuto) { //If it is active, set it to inactive
            readAuto = false;
            autoSendImage.color = Color.white;
            sendButton.SetActive(true);
        } else { //If it is inactive, set it to active
            readAuto = true;
            autoSendImage.color = Color.gray;
            sendButton.SetActive(false);
        }
        //Debug.Log("Change");
    }

    public void ReadConsoleInput() {
        if (avatar.moveCharacter || avatar.turnCharacter) {
            //Debug.Log("Return");
            return;
        }
        string input = consoleInput.text;
        consoleInput.text = "";
        CommandHistory(input);

        playerInputArray = input.Split('=', '\n').ToList();
        /*
        //This is used to keep the history of commands, but also make sure it doesn't run the previous lines again.
        if (readAuto) {
            consoleInput.text = input + "-----" + "\n";
        } else {
            consoleInput.text = input + "\n" + "-----" + "\n";
        }
        consoleInput.caretPosition = consoleInput.text.Length + 1;

        
        int rangeTo = 0;
        for (int i = 0; i < playerInputArray.Count; i++) {
            if (playerInputArray[i] == "-----") {
                rangeTo = i;
            }
        }
        playerInputArray.RemoveRange(0, rangeTo);
        */

        SendNextLine();
    }

    //This will make a history of the used commands, but it will only keep 30 entries (as of now), which will be printed to the previous command text panel
    void CommandHistory(string input) {
        List<string> temp = input.Split('\n').ToList();
        if (temp.Count > 30) {
            temp.RemoveRange(0, temp.Count - 30);
        }
        if (temp.Count == 30) {
            commandHistory.Clear();
            commandHistory = temp;
            commandHistory.Add("-------------------");
        } else {
            int entries = commandHistory.Count + temp.Count;
            if (entries > 30) {
                commandHistory.RemoveRange(0, entries - 30);
                for(int i = 0; i < temp.Count; i++) {
                    commandHistory.Add(temp[i]);
                }
                commandHistory.Add("-------------------");
            } else if (entries <= 30) {
                for (int i = 0; i < temp.Count; i++) {
                    commandHistory.Add(temp[i]);
                }
                commandHistory.Add("-------------------");
            }
        }
        string s = "";
        for (int i = 0; i < commandHistory.Count; i++) {
            s += commandHistory[i];
            s += "\n";
        }
        previousCommandText.text = s;
    }

    public void SendNextLine() {
        string s = "";
        for (int i = 0; i < playerInputArray.Count; i++) {
            s += playerInputArray[i] + "   ";
        }
        Debug.Log(s);

        if (playerInputArray.Count == 0) {
            Debug.Log("Done");
        }
        //Debug.Log("sendNextLine");
        bool sending = false;
        for (int i = 0; i < playerInputArray.Count; i++) {
            Debug.Log(i);
            switch (playerInputArray[i]) {
                case "f":
                case "frem":
                    if (!loopOn) {
                        //loopList = loopList + playerInputArray[i].ToString() + ",";
                        //loopList = loopList + playerInputArray[i + 1].ToString() + ",";
                        avatar.SetMove(float.Parse(playerInputArray[i + 1]), 1, null, this);
                        sending = true;
                        Debug.Log("Removing: " + playerInputArray[i] + " " + playerInputArray[i + 1] + " at position " + i + " and position " + (i + 1));
                        playerInputArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                    }                    
                    break;
                case "drejhøjre":
                case "drejh":
                case "højre":
                case "dh":                    
                    if (!loopOn) {
                        //loopList = loopList + playerInputArray[i].ToString() + ",";
                        //loopList = loopList + playerInputArray[i + 1].ToString() + ",";
                        avatar.SetTurn(float.Parse(playerInputArray[i + 1]) * -1, null, this);
                        sending = true;
                        Debug.Log("Removing: " + playerInputArray[i] + " " + playerInputArray[i + 1] + " at position " + i +  " and position " + (i+1));
                        playerInputArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                    }                
                    break;
                case "drejvenstre":
                case "drejv":
                case "venstre":
                case "dv":                    
                    if (!loopOn) {
                        //loopList = loopList + playerInputArray[i].ToString() + ",";
                        //loopList = loopList + playerInputArray[i + 1].ToString() + ",";
                        avatar.SetTurn(float.Parse(playerInputArray[i + 1]), null, this);
                        sending = true;
                        Debug.Log("Removing: " + playerInputArray[i] + " " + playerInputArray[i + 1] + " at position " + i + " and position " + (i + 1));
                        playerInputArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                    }
                    break;
                case "skiftfarve":
                case "skiftf":
                case "sfarve":
                case "sf":
                case "farve":
                    if (!loopOn)
                    {
                        //loopList = loopList + playerInputArray[i].ToString() + ",";
                        //loopList = loopList + playerInputArray[i + 1].ToString() + ",";
                        sending = true;
                        avatar.SetLineColor(playerInputArray[i + 1], null, this);
                        Debug.Log("Removing: " + playerInputArray[i] + " " + playerInputArray[i + 1] + " at position " + i + " and position " + (i + 1));
                        playerInputArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                    }
                    break;
                case "loopstart":
                case "lstart":
                    //Debug.Log("loopstart");
                    loopCount = float.Parse(playerInputArray[i + 1]);
                    Debug.Log("Removing: " + playerInputArray[i] + " " + playerInputArray[i + 1] + " at position " + i + " and position " + (i + 1));
                    playerInputArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                    loopOn = true;
                    break;
                case "loopstop":
                case "lstop":
                    //Debug.Log("loopstop");
                    //SendLoop();
                    playerInputArray.RemoveAt(i);
                    PrepareCommandLoop();
                    sending = true;
                    break;
            }

            if (sending) {
                break;
            }
        }
    }

    //New code to run loops. This copy the inputs created using SendNextLine, override the command list and send them to be executed.
    void PrepareCommandLoop() {
        List<string> temp = new List<string>();
        string s = "";
        for (int i = 0; i < loopCount; i++) { //Build a temporary list of the strings
            for (int j = 0; j < playerInputArray.Count; j++) {
                s += playerInputArray[j] + "   ";
                temp.Add(playerInputArray[j]);
            }
        }
        Debug.Log(s);
        playerInputArray.Clear(); //Clear the inputarray (probably not necessary, just a pre-caution)
        playerInputArray = temp;  //Give the inputarray the values of the temporary list
        loopOn = false;
        SendNextLine();
    }

    //Previous code for running loops. This is a copy of SendNextLine, which is made for loops specifically.
    public void SendLoop()
    {
        Debug.Log("Starting loop");

        Debug.Log(loopList + " <- looplist string");

        for (int j = 0; j < loopCount; j++)
        {
            loopListArray.Clear();
            loopListArray = loopList.Split(',').ToList();

            for (int i = 0; i < loopListArray.Count; i++)
            {
                Debug.Log("sendNextLoop");
                Debug.Log(loopListArray.Count);
                Debug.Log(i);
                Debug.Log(loopCount);
                bool sending = false;
                switch (loopListArray[i])
                {

                    case "f":
                    case "frem":
                        Debug.Log("frem in loop");
                        avatar.SetMove(float.Parse(loopListArray[i + 1]), 1, null, this);
                        sending = true;
                        loopListArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                        break;
                    case "drejhøjre":
                    case "drejh":
                    case "højre":
                    case "dh":
                        Debug.Log("dh in loop");
                        avatar.SetTurn(float.Parse(loopListArray[i + 1]) * -1, null, this);
                        sending = true;
                        loopListArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                        break;
                    case "drejvenstre":
                    case "drejv":
                    case "venstre":
                    case "dv":
                        Debug.Log("dv in loop");
                        avatar.SetTurn(float.Parse(loopListArray[i + 1]), null, this);
                        sending = true;
                        loopListArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                        break;
                    case "skiftfarve":
                    case "skiftf":
                    case "sfarve":
                    case "sf":
                    case "farve":
                        Debug.Log("color in loop");
                        loopCount--;
                        sending = true;
                        avatar.SetLineColor(loopListArray[i + 1], null, this);
                        loopListArray.RemoveRange(i, 2); //Remove items at index 'i' and the next (from 'i' (included) and two forward, which gives 'i' and 'i+1')
                        break;
                }

                if (sending)
                {
                    break;
                }
            }
            if (loopCount == j)
            {
                Debug.Log("Loop Done");
            }
        }
    }

    public void ClearDrawArea() {
        avatar.StopAllActions(true);
    }

    public void ClearCommandList() {
        loopListArray.Clear();
    }

    public void ShowPreviousCommand(bool toggleOn) {
        if (toggleOn) {
            console.SetActive(false);
            showPreviousCommandButton.SetActive(false);
            previousCommandPanel.SetActive(true);
            showConsoleButton.SetActive(true);
        } else {
            console.SetActive(true);
            showPreviousCommandButton.SetActive(true);
            previousCommandPanel.SetActive(false);
            showConsoleButton.SetActive(false);
        }
    }
}