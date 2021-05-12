using UnityEngine;
using UnityEngine.UI;

public class BlockConstructor : MonoBehaviour {
    public string type;
    public string setting;

    //Called to access its child's text component, read, parse to float and return to the caller.
    public float GetValue() {
        return float.Parse(GetComponentInChildren<Text>().text);
    }

    public string GetString() {
        return GetComponentInChildren<Text>().text;
    }
}