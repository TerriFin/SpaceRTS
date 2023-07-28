using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MineralsToWindow : MonoBehaviour {

    public TMP_Text mineralText;

    private void Update() {
        int totalMinerals = 0;
        foreach (Selectable selectable in SelectionManager.selected) {
            totalMinerals += selectable.AttachedMineralStorage.currentMineralStorage;
        }
        mineralText.text = totalMinerals + "";
    }
}
