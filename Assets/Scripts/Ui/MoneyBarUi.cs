using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyBarUi : MonoBehaviour {
    public RectTransform barRoot;
    public RectTransform barRootCost;
    public TMP_Text currentMoneyText;

    private void Update() {
        if (FactionManager.PlayerFaction != null) {
            barRoot.localScale = new Vector3(1, FactionManager.PlayerFaction.money <= FactionManager.PlayerFaction.maxMoney ? (float)FactionManager.PlayerFaction.money / (float)FactionManager.PlayerFaction.maxMoney : 1, 1);
            barRootCost.localScale = new Vector3(1, BuildingPlacementManager.IsBuilding ? (float)BuildingPlacementManager.Building.moneyCost / (float)FactionManager.PlayerFaction.maxMoney : 0, 1);
            currentMoneyText.text = "$ " + FactionManager.PlayerFaction.money + " $";
        } else {
            barRoot.localScale = new Vector3(1, 0, 1);
            barRootCost.localScale = new Vector3(1, 0, 1);
            currentMoneyText.text = "$ " + 0 + " $";
        }
    }
}
