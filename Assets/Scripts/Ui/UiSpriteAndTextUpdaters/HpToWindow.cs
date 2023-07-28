using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpToWindow : MonoBehaviour {

    public TMP_Text hpText;

    private void Update() {
        int totalHp = 0;
        foreach (Selectable selectable in SelectionManager.selected) {
            totalHp += selectable.AttachedHitpoints.CurrentHp;
        }
        hpText.text = totalHp + "";
    }
}
