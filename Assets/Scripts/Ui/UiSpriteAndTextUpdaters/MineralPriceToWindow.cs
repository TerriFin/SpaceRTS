using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MineralPriceToWindow : MonoBehaviour {

    public TMP_Text mineralPriceText;

    private Market AttachedMarket;

    private void Start() {
        AttachedMarket = SelectionManager.selected[0].GetComponent<Market>();
    }

    private void Update() {
        mineralPriceText.text = AttachedMarket.currentMineralPrice + "";
    }
}
