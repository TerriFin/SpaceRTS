using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSelectionWindow : MonoBehaviour {

    public GameObject newWindow;
    public bool DELETE_PARENT;

    private Button button;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeSelectionWindows);
    }

    private void ChangeSelectionWindows() {
        SelectionStats.SetCurrentlySelected(Instantiate(newWindow, FindObjectOfType<Canvas>().transform));
        if (DELETE_PARENT) {
            Destroy(gameObject.transform.parent.gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
