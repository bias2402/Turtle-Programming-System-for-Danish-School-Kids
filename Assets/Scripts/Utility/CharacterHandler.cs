using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterHandler : MonoBehaviour {
    [HideInInspector] public bool moveCharacter = false;
    [HideInInspector] public bool turnCharacter = false;
    [HideInInspector] public bool resetCharacterNow = false;
    [HideInInspector] public UIInformationHolder infoHolder;

    [SerializeField] private GameObject UILinePrefab;
    [SerializeField] private Image spriteColorArea;

    private bool usingSound = false;
    private AudioClip soundClip;
    private float soundsPlayDelay;
    private float speed = 1;
    private bool playSound = true;
    private RectTransform currentUILine;
    private Color UILineColor = Color.red;
    private Quaternion newRotation;
    private float counter;
    private float moveTime;
    private int direction = 1;
    private bool addMathToMove = false;
    private string mathType = "";
    private float mathValue = 0;
    private bool drawNext = true;
    private float UILineLength = 1;
    private Vector3 newPos;
    private LoopReader loopReader;
    private ConsoleScriptHandler consoleHandler;

    //Save the reset information to the infoHolder
    void Awake() {
        loopReader = FindObjectOfType<LoopReader>();
        consoleHandler = FindObjectOfType<ConsoleScriptHandler>();
        loopReader.avatar = this;
        consoleHandler.avatar = this;
        SetUpAvatar();
    }

    void SetUpAvatar() {
        AvatarConstructor ac = GetComponent<AvatarConstructor>();
        if (ac.usingSound) {
            soundClip = ac.soundClip;
            soundsPlayDelay = ac.soundPlayDelay;
            usingSound = true;
        }
        if (spriteColorArea != null) {
            spriteColorArea.color = UILineColor;
        }
    }

    void OnEnable() {
        GetComponent<SpriteCycler>().StartAnimation();
    }

    public void StopAllActions(bool reset) {
        moveCharacter = false;
        turnCharacter = false;
        drawNext = true;
        playSound = true;
        if (loopReader != null) {
            loopReader.ClearBlockArray();
        }
        if (consoleHandler != null) {
            consoleHandler.ClearCommandList();
        }
        if (reset) {
            ResetCharacter();
        }
    }

    //Check if the character should move or turn. FixedUpdate works fine with this in this combination. Calls the Turn or Move method if needed.
    void FixedUpdate() {
        if (moveCharacter) {
            CheckIfInsideArea();

            if (counter < moveTime) {
                Move();
                counter += 1 * Time.deltaTime * speed;
                if (drawNext) {
                    drawNext = false;
                    StartCoroutine(DrawLine(0));
                }
            } else {
                StartCoroutine(DrawLine(currentUILine.rect.height / 2));
                moveCharacter = false;
                counter = 0;
                if (loopReader == null) {
                    StartCoroutine(NextLine());
                } else {
                    StartCoroutine(NextBlock());
                }
            }
        }

        if (turnCharacter) {
            if (transform.localRotation != newRotation) {
                Turn();
            } else {
                turnCharacter = false;
                if (loopReader == null) {
                    StartCoroutine(NextLine());
                } else {
                    StartCoroutine(NextBlock());
                }
            }
        }

        if (!moveCharacter && !turnCharacter) {
            infoHolder.soundEffectHandler.StopPlaying();
        }
    }

    void CheckIfInsideArea() {
        if (transform.localPosition.x < infoHolder.playArea.rect.xMin) {
            transform.localPosition = new Vector3(infoHolder.playArea.rect.xMax, transform.localPosition.y, transform.localPosition.z);
            CreateANewLine();
        }

        if (transform.localPosition.x > infoHolder.playArea.rect.xMax) {
            transform.localPosition = new Vector3(infoHolder.playArea.rect.xMin, transform.localPosition.y, transform.localPosition.z);
            CreateANewLine();
        }

        if (transform.localPosition.y < infoHolder.playArea.rect.yMin) {
            transform.localPosition = new Vector3(transform.localPosition.x, infoHolder.playArea.rect.yMax, transform.localPosition.z);
            CreateANewLine();
        }

        if (transform.localPosition.y > infoHolder.playArea.rect.yMax) {
            transform.localPosition = new Vector3(transform.localPosition.x, infoHolder.playArea.rect.yMin, transform.localPosition.z);
            CreateANewLine();
        }
    }

    //Reset the character using the saved information from the infoHolder.
    public void ResetCharacter() {
        transform.localPosition = infoHolder.avatarStartPosition;
        transform.localRotation = infoHolder.avatarStartRotation;
        UILineColor = Color.red;
        UILineLength = 1;
        foreach (Transform lineChild in transform.parent) {
            if (lineChild.name.Contains("UI Line")) {
                Destroy(lineChild.gameObject);
            }
        }
    }

    //Set the movement settings and toggle moveCharacter, so it will be moved. Also adds math, if told to do so. And now it even set ups the line drawing.
    public void SetMove(float moveTime, int direction, LoopReader lr, ConsoleScriptHandler csh) {
        loopReader = lr;
        consoleHandler = csh;
        if (addMathToMove) {
            switch (mathType) {
                case "addition":
                    this.moveTime = moveTime + mathValue;
                    break;
                case "subtraction":
                    this.moveTime = moveTime - mathValue;
                    break;
                case "multiplication":
                    this.moveTime = moveTime * mathValue;
                    break;
                case "division":
                    this.moveTime = moveTime / mathValue;
                    break;
            }
            addMathToMove = false;
        } else {
            this.moveTime = moveTime;
            Debug.Log(moveTime);
        }
        this.direction = direction;
        moveCharacter = true;
        CreateANewLine();
    }

    void CreateANewLine() {
        GameObject line = Instantiate(UILinePrefab, transform.parent);
        currentUILine = line.GetComponent<RectTransform>();
        currentUILine.GetComponent<Image>().color = UILineColor;
        currentUILine.localRotation = transform.localRotation;
        currentUILine.position = transform.position;
        currentUILine.SetAsFirstSibling();
        if (direction == -1) {
            currentUILine.rotation = Quaternion.Euler(currentUILine.rotation.x, currentUILine.rotation.y, currentUILine.rotation.z + 180);
        }
    }

    //Set the turning by calculating the new angle to reach, then toggle turnCharacter, so it be turned. Also adds math, if told to do so.
    public void SetTurn(float degreeToRotate, LoopReader lr, ConsoleScriptHandler csh) {
        loopReader = lr;
        consoleHandler = csh;
        if (addMathToMove) {
            switch (mathType) {
                case "addition":
                    newRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + (degreeToRotate + mathValue));
                    break;
                case "subtraction":
                    newRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + (degreeToRotate - mathValue));
                    break;
                case "multiplication":
                    newRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + (degreeToRotate * mathValue));
                    break;
                case "division":
                    newRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + (degreeToRotate / mathValue));
                    break;
            }
            addMathToMove = false;
        } else {
            newRotation = Quaternion.AngleAxis(transform.eulerAngles.z + degreeToRotate, Vector3.forward);
        }        
        turnCharacter = true;
    }

    public void SetLineColor(string colorName, LoopReader lr, ConsoleScriptHandler csh) {
        loopReader = lr;
        consoleHandler = csh;
        switch (colorName.ToLower()) {
            case "rød":
                UILineColor = Color.red;
                break;
            case "blå":
                UILineColor = Color.blue;
                break;
            case "grøn":
                UILineColor = Color.green;
                break;
            case "gul":
                UILineColor = Color.yellow;
                break;
            case "sort":
                UILineColor = Color.black;
                break;
            case "hvid":
                UILineColor = Color.white;
                break;
            case "grå":
                UILineColor = Color.grey;
                break;
            case "lilla":
                UILineColor = Color.magenta;
                break;
            case "turkis":
                UILineColor = Color.cyan;
                break;
        }
        if (spriteColorArea != null) {
            spriteColorArea.color = UILineColor;
        }
        if (loopReader == null) {
            StartCoroutine(NextLine());
        } else {
            StartCoroutine(NextBlock());
        }
    }

    //Sets up the math to add to the next move, if a math block is read in the LoopReader, which then calls this.
    public void AddMathToNextMove(string type, float value, LoopReader caller) {
        addMathToMove = true;
        mathType = type;
        mathValue = value;
        loopReader = caller;
        StartCoroutine(NextBlock());
    }

    public void SetSpeed(float newSpeed, LoopReader lr, ConsoleScriptHandler csh) {
        loopReader = lr;
        consoleHandler = csh;
        speed = newSpeed;

        if (loopReader == null) {
            StartCoroutine(NextLine());
        } else {
            StartCoroutine(NextBlock());
        }
    }

    //Called to move the character.
    void Move() {
        transform.Translate(Vector3.right * direction * speed);
        if (usingSound) {
            PlaySound();
        }
    }

    //Called to turn the character.
    void Turn() {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, newRotation, 1);
        if (usingSound) {
            PlaySound();
        }
    }

    void PlaySound() {
        if (playSound) {
            infoHolder.soundEffectHandler.SetAndPlaySoundClip(soundClip);
            playSound = false;
            StartCoroutine(SoundBreak());
        }
    }

    IEnumerator SoundBreak() {
        yield return new WaitForSeconds(soundsPlayDelay);
        playSound = true;
    }
    
    //Update and then draw the line after a delay
    IEnumerator DrawLine(float additionToWidth) {
        Vector3 pos = transform.localPosition - currentUILine.localPosition;
        float width = pos.magnitude + additionToWidth;
        currentUILine.sizeDelta = new Vector2(width, currentUILine.rect.height);
        yield return new WaitForSeconds(0.05f);
        drawNext = true;
    }

    IEnumerator NextLine() {
        yield return new WaitForSeconds(0.5f);
        consoleHandler.SendNextLine();
    }

    //Timer to wait before reading the next block (so the user can actually see the break between the blocks).
    IEnumerator NextBlock() {
        yield return new WaitForSeconds(0.5f);
        loopReader.ReadNextBlock();
        if (transform.position != newPos) {
            loopReader.SaveAvatarPosition(transform.position);
        }
        newPos = transform.position;
    }
}