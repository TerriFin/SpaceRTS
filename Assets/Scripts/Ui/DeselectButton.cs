using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeselectButton : MonoBehaviour {
    private Button Button;

    private void Start() {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(ResetSelection);
    }

    private void ResetSelection() {
        SelectionManager.ClearSelection();
    }
}
