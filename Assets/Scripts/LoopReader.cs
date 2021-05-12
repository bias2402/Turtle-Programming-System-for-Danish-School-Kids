using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopReader : MonoBehaviour {
    [HideInInspector] public bool readLoop;
    [HideInInspector] public CharacterHandler avatar;

    [SerializeField] private UIInformationHolder infoHolder;
    [SerializeField] private Transform feedbackBlockPosition;
    [SerializeField] private Text loopInfoText;

    private List<BlockConstructor> blocksInLoop = new List<BlockConstructor>();
    private List<Vector3> blockLoadPositions = new List<Vector3>();
    private GameObject currentProcessingBlock;
    private int loopSize = 0;
    private int blocksRead = 0;

    public void PrepareLoopForReading() {
        if (readLoop) {
            StartCoroutine(DelayStart());
        }
        readLoop = true;
        blocksInLoop.Clear();
        PreparePositionList();
        avatar.ResetCharacter();
        avatar.turnCharacter = false;
        avatar.moveCharacter = false;
        for (int i = 0; i < int.Parse(infoHolder.loopInput.text); i++) {
            foreach (GameObject socket in infoHolder.sockets) {
                if (socket.transform.parent.name.Contains("Block")) {
                    blocksInLoop.Add(socket.transform.parent.GetComponent<BlockConstructor>());
                }
            }
        }
        loopSize = blocksInLoop.Count;
        blocksRead = 0;
        infoHolder.ChangeWindow("game");
        if (currentProcessingBlock != null) {
            Destroy(currentProcessingBlock);
        }
        StartCoroutine(StartReadingDelay());
    }

    IEnumerator DelayStart() {
        yield return new WaitForSeconds(0.5f);
        readLoop = false;
        PrepareLoopForReading();
    }

    void PreparePositionList() {
        blockLoadPositions.Clear();
        blockLoadPositions.Add(infoHolder.avatarStartPosition);
    }

    public void ReadNextBlock() {
        if (blocksInLoop.Count == 0 || blocksInLoop == null || !readLoop) {
            Debug.Log("Done");
            /*if (SolutionChecker()) {
                Debug.Log("Solved");
            } else {
                Debug.Log("Not solved");
            }*/
            loopInfoText.text += "\n Færdig med at køre programmet.";
            return;
        }
        blocksRead++;
        loopInfoText.text = "Antal blokke: " + loopSize + "\n Nuværende blok: " + blocksRead + "\n ";

        switch (blocksInLoop[0].type) {
            case "forward":
                avatar.SetMove(blocksInLoop[0].GetValue(), 1, this, null);
                break;
            case "rotate-left":
                avatar.SetTurn(blocksInLoop[0].GetValue(), this, null);
                break;
            case "rotate-right":
                avatar.SetTurn(-blocksInLoop[0].GetValue(), this, null);
                break;
            case "addition":
            case "subtraction":
            case "multiplication":
            case "division":
                avatar.AddMathToNextMove(blocksInLoop[0].type, blocksInLoop[0].GetValue(), this);
                break;
            case "color":
                avatar.SetLineColor(blocksInLoop[0].GetString(), this, null);
                break;
        }
        FeedbackBlockUpdater(blocksInLoop[0].gameObject);
        blocksInLoop.RemoveAt(0);
    }

    void FeedbackBlockUpdater(GameObject block) {
        if (currentProcessingBlock != null) {
            Destroy(currentProcessingBlock);
        }
        currentProcessingBlock = Instantiate(block, feedbackBlockPosition.position, Quaternion.identity);
        currentProcessingBlock.transform.SetParent(feedbackBlockPosition);
        currentProcessingBlock.GetComponent<UIActions>().enabled = false;
        foreach (Transform child in currentProcessingBlock.transform) {
            if (child.name.Contains("Socket")) {
                child.GetComponent<UIActions>().enabled = false;
            }
        }
    }

    public void ClearBlockArray() {
        blocksInLoop.Clear();
    }

    public void ClosePlayArea() {
        avatar.StopAllActions(true);
        infoHolder.ChangeWindow("edit");
    }

    IEnumerator StartReadingDelay() {
        yield return new WaitForSeconds(0.5f);
        ReadNextBlock();
    }

    public void SaveAvatarPosition(Vector3 pos) {
        blockLoadPositions.Add(pos);
    }

    bool SolutionChecker() {
        switch (infoHolder.taskType) {
            case "square":
                for (int i = 0, j = 2; j < blockLoadPositions.Count; i++, j++) {
                    if (Mathf.Floor(Vector3.Distance(blockLoadPositions[i], blockLoadPositions[j])) == infoHolder.finalSolution &&
                        Mathf.Floor(Vector3.Distance(blockLoadPositions[i + 1], blockLoadPositions[j + 1])) == infoHolder.finalSolution) {
                        return true;
                    }
                }
                Debug.Log(Mathf.Floor(Vector3.Distance(blockLoadPositions[0], blockLoadPositions[2])));
                break;
            case "triangle":

                break;
        }

        return false;
    }
}