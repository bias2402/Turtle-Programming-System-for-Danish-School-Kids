using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskSelect : MonoBehaviour {
    [SerializeField] private TasksUIHandler handler;
    [SerializeField] [TextArea] private string taskDesription;
    [SerializeField] private Sprite taskObjective;
    [SerializeField] private float solution;
    [SerializeField] private string taskType;

    void Awake() {
        GetComponent<Button>().onClick.AddListener(SetDescription);
    }

    //Sets the description and image of the task description area
    public void SetDescription() {
        FindObjectOfType<UIInformationHolder>().finalSolution = solution;
        FindObjectOfType<UIInformationHolder>().taskType = taskType;
        handler.description.text = taskDesription;
        handler.taskObjective.sprite = taskObjective;
        handler.inTaskDescription.text = taskDesription;
        handler.inTaskObjective.sprite = taskObjective;
    }
}