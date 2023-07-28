using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetSelectionButton : MonoBehaviour {

    private Button button;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(ResetMenu);
    }

    private void ResetMenu() {
        SelectionStats.ResetSelection();
        Destroy(gameObject);
    }
}
