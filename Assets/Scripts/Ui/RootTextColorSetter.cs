using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RootTextColorSetter : MonoBehaviour {
    private void OnEnable() {
        if (FactionManager.PlayerFaction != null) {
            foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>()) {
                Color newColor = FactionManager.PlayerFaction.factionColor;
                newColor.a = 1;
                text.color = newColor;
            }

            foreach (Text text in GetComponentsInChildren<Text>()) {
                Color newColor = FactionManager.PlayerFaction.factionColor;
                newColor.a = 1;
                text.color = newColor;
            }
        }
    }

    private void Start() {
        if (FactionManager.PlayerFaction != null) {
            foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>()) {
                Color newColor = FactionManager.PlayerFaction.factionColor;
                newColor.a = 1;
                text.color = newColor;
            }

            foreach (Text text in GetComponentsInChildren<Text>()) {
                Color newColor = FactionManager.PlayerFaction.factionColor;
                newColor.a = 1;
                text.color = newColor;
            }
        }
    }
}
