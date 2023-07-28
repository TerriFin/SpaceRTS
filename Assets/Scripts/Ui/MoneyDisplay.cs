using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour {

    public Text moneyDisplay;

    private void Update() {
        if (FactionManager.PlayerFaction != null) moneyDisplay.text = FactionManager.PlayerFaction.money + "$";
    }
}
