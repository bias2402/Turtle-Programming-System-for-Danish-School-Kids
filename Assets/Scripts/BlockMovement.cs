using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMovement : MonoBehaviour {
    [HideInInspector] public bool hoveringUIElement = false;
    [HideInInspector] public GameObject currentUIElement;
    [HideInInspector] public bool holdingBlock = false;
    [HideInInspector] public GameObject currentBlock;

    //Make sure to check, if a block is picked up, so it can be moved along the mouse, though behind it, so you can still interact and click on things.
    void Update() {
        if (holdingBlock && hoveringUIElement && Input.GetMouseButtonDown(0) && currentUIElement.tag == "Socket") {
            currentBlock.GetComponent<UIActions>().SetBlock(currentUIElement.transform, currentBlock);
        }

        if (holdingBlock) {
            currentBlock.transform.position = new Vector3(Input.mousePosition.x - 65, Input.mousePosition.y - 65, Input.mousePosition.z);
        }
    }
}