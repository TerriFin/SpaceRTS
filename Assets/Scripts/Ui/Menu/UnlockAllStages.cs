using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAllStages : MonoBehaviour {
    private int ClickCounter;

    private void Start() {
        ClickCounter = 0;
    }

    public void OnClick() {
        ClickCounter++;
        if (ClickCounter > 10) {
            for (int i = 1; i <= 6; i++) {
                PlayerPrefs.SetInt("federation" + i, 1);
                PlayerPrefs.SetInt("empire" + i, 1);
                PlayerPrefs.SetInt("pirate" + i, 1);
            }
        }
    }
}
