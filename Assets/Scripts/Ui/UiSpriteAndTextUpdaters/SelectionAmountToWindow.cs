using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionAmountToWindow : MonoBehaviour {

    public TMP_Text amountText;

    private void Update() {
        amountText.text = SelectionManager.selected.Count + "";
    }
}
