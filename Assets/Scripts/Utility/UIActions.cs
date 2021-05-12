using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIActions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [HideInInspector] public bool gotParent = false;

    public enum Methods { Empty, Block, Socket, HoverMenu, HoverPopup };
    public Methods methods;
    public float hoveringTime = 0;
    public bool beingHovered = false;
    public bool interactiveObject;

    [SerializeField] [TextArea] private string objectDescription;
    [SerializeField] LoopReader loopReader;
    [SerializeField] private GameObject areaPanel;
    [SerializeField] private AudioClip setBlockInSocket;
    [SerializeField] private AudioClip pickBlockFromSocket;

    private UIInformationHolder infoHolder;
    private ConsoleScriptHandler consoleHandler;
    private bool hovering = false;
    private float hoveringCounter = 0;

    //Get reference to the UIInformationHolder class. It's on the Canvas called "UI"
    void Start() {
        infoHolder = FindObjectOfType<UIInformationHolder>();
        consoleHandler = FindObjectOfType<ConsoleScriptHandler>();
    }

    void Update() {
        try {
            if (methods.ToString() == "HoverMenu" || methods.ToString() == "HoverPopup") {
                if (hovering) {
                    hoveringCounter += Time.deltaTime;
                    if (hoveringCounter >= hoveringTime && methods.ToString() == "HoverMenu") {
                        areaPanel.SetActive(true);
                    }
                } else {
                    hoveringCounter -= Time.deltaTime;
                    if (hoveringCounter <= 0 && !areaPanel.GetComponent<UIActions>().beingHovered && methods.ToString() == "HoverMenu") {
                        areaPanel.SetActive(false);
                    }
                }
            }
        } catch { }
    }

    //Interface implemented to raycast UI elements. This one is called, when the cursor enters
    public void OnPointerEnter(PointerEventData eventData) { //Interface method
        if (interactiveObject) {
            SetDescriptionAreaText(false);
        }
        switch (methods.ToString()) {
            case "Socket":
                SwitchBorderSprite(1, true);
                break;
            case "HoverMenu":
                hovering = true;
                hoveringCounter = 0;
                break;
            case "HoverPopup":
                beingHovered = true;
                break;
        } 
    }

    //Interface implemented to raycast UI elements. This one is called, when the cursor exits
    public void OnPointerExit(PointerEventData eventData) { //Interface method
        if (interactiveObject) {
            SetDescriptionAreaText(true);
        }
        switch (methods.ToString()) {
            case "Socket":
                SwitchBorderSprite(0, false);
                break;
            case "HoverMenu":
                hovering = false;
                hoveringCounter = 0.25f;
                break;
            case "HoverPopup":
                beingHovered = false;
                break;
        }
    }

    //Interface implemented to raycast UI elements. This one is called, when the cursor clicks
    public void OnPointerClick(PointerEventData eventData) { //Interface method
        switch (methods.ToString()) {
            case "Block":
                PickupBlock(false);
                break;
            case "Socket":
                if (transform.parent.tag == "Block" && gotParent) {
                    PickupBlock(true);
                    infoHolder.PlaySound(pickBlockFromSocket);
                } else {
                    infoHolder.PlaySound(setBlockInSocket);
                }
                break;
        }
    }

    void SetDescriptionAreaText(bool setDefault) {
        if (setDefault) {
            infoHolder.descriptionArea.text = infoHolder.descriptionAreaDefault;
        } else {
            infoHolder.descriptionArea.text = objectDescription;
        }
    }

    //This method changes the sprite of the sockets, whenever the cursor moves over them
    void SwitchBorderSprite(int spriteIndex, bool hovering) {
        GetComponent<RawImage>().texture = infoHolder.borders[spriteIndex];
        infoHolder.blockMover.hoveringUIElement = hovering;
        if (hovering) {
            infoHolder.blockMover.currentUIElement = gameObject;
        } else {
            infoHolder.blockMover.currentUIElement = null;
        }
    }

    //This method is used to pickup blocks that are already placed in sockets, or if a block is lying around in the scene (shouldn't happen at all)
    //It changes around the parent of the socket, so the elements are properly stacked for the visuals (render order)
    public void PickupBlock(bool switchPickup) { //switchPickup is to check, if the border and block has to switch place, and then pickup the block
        if (infoHolder.blockMover.holdingBlock) { return; }
        infoHolder.blockMover.holdingBlock = true;
        if (switchPickup) {
            infoHolder.blockMover.currentBlock = transform.parent.gameObject;
            transform.SetParent(infoHolder.socketParent, true);
            gotParent = false;
        } else {
            infoHolder.blockMover.currentBlock = gameObject;
        }
    }

    //This places blocks in sockets and set the sockets parent as the block, so the elements are properly stacked for the visuals (rendering order)
    public void SetBlock(Transform socket, GameObject block) { //Used for a button
        if (socket.parent.name.Contains("Block")) { return; }
        infoHolder.blockMover.holdingBlock = false;
        infoHolder.blockMover.currentBlock = null;
        block.transform.position = socket.position;
        socket.SetParent(block.transform, true);
        StartCoroutine(SetSocketParentBool(socket));
    }

    //This is called from the SetBlock method, to ensure there is delay between placing a block in a socket and picking it up, else it will do both instantly
    IEnumerator SetSocketParentBool(Transform socket) {
        yield return new WaitForSeconds(0.2f);
        socket.GetComponent<UIActions>().gotParent = true;
    }

    //Destroys the current block being hold
    public void DeleteCurrentBlock() { //Used for a button
        Destroy(infoHolder.blockMover.currentBlock);
        infoHolder.blockMover.holdingBlock = false;
    }

    //This will instantiate a block of the type, which is clicked
    public void CreateBlock(GameObject blockType) { //Used for a button
        if (infoHolder.blockMover.holdingBlock || blockType == null) { return; }//If the player already has a block in hand, just return, so they don't get multiple
        GameObject go = Instantiate(blockType);
        infoHolder.blockMover.holdingBlock = true;
        infoHolder.blockMover.currentBlock = go;
        go.transform.SetParent(infoHolder.socketParent, false);
    }

    //This will first find the cogwheel, which was clicked, then find the socket associated to the cogwheel (arrays are set up, so they match indexes)
    //it then sets the options of the dropdown menu based on the sockets parent, which is block with a specific type, and open up the editor panel, so
    //the player can choose a new value for the block
    public void OpenBlockEditor() { //Used for a button
        if (infoHolder.blockMover.holdingBlock) { return; }
        for (int i = 0; i < infoHolder.editCogs.Length; i++) {
            if (infoHolder.editCogs[i].GetComponent<UIActions>() == this) {
                infoHolder.lastCogwheelUsed = i;
                break;
            }
        }
        if (infoHolder.sockets[infoHolder.lastCogwheelUsed].transform.parent.name.Contains("Block")) {
            infoHolder.editBlockPanel.SetActive(true);
            infoHolder.editBlockDropdown.ClearOptions(); //Clear dropdown options
            SetDropdownOptions(infoHolder.sockets[infoHolder.lastCogwheelUsed].transform.parent.GetComponent<BlockConstructor>().type); //Call a method, which will 
                                                                                                                                        //set the options based on the given string parameter "type"
        }
    }

    public void CopyBlock() {
        for (int i = 0; i < infoHolder.copyButtons.Length; i++) {
            if (infoHolder.copyButtons[i].GetComponent<UIActions>() == this) {
                infoHolder.lastCopyButtonUsed = i;
                break;
            }
        }
        if (infoHolder.sockets[infoHolder.lastCopyButtonUsed].transform.parent.name.Contains("Block") && infoHolder.blockMover.holdingBlock == false) {
            GameObject go = Instantiate(infoHolder.sockets[infoHolder.lastCopyButtonUsed].transform.parent.gameObject);
            foreach (Transform child in go.transform) {
                if (child.name.Contains("Socket")) {
                    Destroy(child.gameObject);
                }
            }
            infoHolder.blockMover.holdingBlock = true;
            infoHolder.blockMover.currentBlock = go;
            go.transform.SetParent(infoHolder.socketParent, false);
        }
    }

    //This method uses the string parameter "type" to choose the list that should be added to the dropdown, so the player gets the right options for the block
    void SetDropdownOptions(string type) {
        switch (type) {
            case "forward":
            case "back":
            case "addition":
            case "subtraction":
            case "multiplication":
            case "division":
                infoHolder.editBlockDropdown.AddOptions(infoHolder.movementValues);
                break;
            case "rotate-left":
            case "rotate-right":
                infoHolder.editBlockDropdown.AddOptions(infoHolder.rotateValues);
                break;
            case "color":
                infoHolder.editBlockDropdown.AddOptions(infoHolder.colorNames);
                break;
        }
    }

    //Used to confirm the choosen dropdown value in the editor panel. It will then update the necessary variables and the on-block text, and then closes the editor again
    public void ConfirmBlockEdit() { //Used for a button
        if (infoHolder.sockets[infoHolder.lastCogwheelUsed].transform.parent.GetComponent<BlockConstructor>().type == "color") {
            infoHolder.sockets[infoHolder.lastCogwheelUsed].transform.parent.GetComponentInChildren<Text>().text = infoHolder.editBlockDropdown.transform.GetChild(0).GetComponent<Text>().text;
        } else {
            infoHolder.editBlockValue = int.Parse(infoHolder.editBlockDropdown.transform.GetChild(0).GetComponent<Text>().text);
            infoHolder.sockets[infoHolder.lastCogwheelUsed].transform.parent.GetComponentInChildren<Text>().text = infoHolder.editBlockValue.ToString();
        }
        infoHolder.editBlockPanel.SetActive(false);
        //if (infoHolder.sockets[infoHolder.lastCogwheelUsed].transform.parent.GetComponent<BlockConstructor>().type == "color") {
        //    infoHolder.sockets[infoHolder.lastCogwheelUsed].transform.parent.GetComponent<Image>().color = 
        //}
    }

    //This method closes the editor panel without saving anything from the dropdown
    public void CancelBlockEdit() { //Used for a button
        infoHolder.editBlockPanel.SetActive(false);
    }

    //This will start the LoopReader
    public void StartProgram() {
        loopReader.PrepareLoopForReading();
    }

    //This will stop the LoopReader
    public void StopProgramFromRunning() {
        infoHolder.activeAvatar.GetComponent<CharacterHandler>().StopAllActions(false);
    }

    //This will close the play area
    public void ClosePlayArea() {
        loopReader.readLoop = false;
        loopReader.ClosePlayArea();
    }
}