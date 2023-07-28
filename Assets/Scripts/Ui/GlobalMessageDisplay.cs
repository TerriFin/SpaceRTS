using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalMessageDisplay : MonoBehaviour {

    public List<TMP_Text> rows;

    private void Start() {
        GlobalMessageManager.newMessageDelegate += UpdateMessages;
    }

    private void UpdateMessages(string message) {
        for (int i = 1; i < rows.Count; i++) {
            rows[rows.Count - i].text = rows[rows.Count - i - 1].text;
        }

        rows[0].text = message;
    }
}
