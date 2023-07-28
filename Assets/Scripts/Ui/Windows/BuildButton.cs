using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

public class BuildButton : MonoBehaviour, IPointerEnterHandler {

    private MineralBarUi buildingCosts;
    private Button button;
    private MineralBarUi commandCenterMineralUpdater;

    public string buildingName { set; private get; }
    public int mineralCost { set; private get; }
    public int moneyCost{ set; private get; }

    private void Start() {
        buildingCosts = FindObjectOfType<MineralBarUi>();
        button = GetComponent<Button>();
        commandCenterMineralUpdater = FindObjectOfType<MineralBarUi>();
    }

    private void Update() {
        button.enabled = mineralCost <= commandCenterMineralUpdater.CurrentMaxMinerals && moneyCost <= FactionManager.PlayerFaction.money;
    }

    private void OnDestroy() {
        if (buildingCosts != null) buildingCosts.ToggleBuildingCostTexts(false);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        buildingCosts.ToggleBuildingCostTexts(true);
        buildingCosts.buildingName.text = Regex.Replace(buildingName, "(\\B[A-Z])", " $1");
        buildingCosts.mineralCost.text = mineralCost + "";
        buildingCosts.moneyCost.text = moneyCost + "";
    }
}
