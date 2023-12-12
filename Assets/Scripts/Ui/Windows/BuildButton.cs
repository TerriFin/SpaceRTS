using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

public class BuildButton : MonoBehaviour, IPointerEnterHandler {

    private MineralBarUi mineralAmountManager;
    private Button button;

    public string buildingName { set; private get; }
    public int mineralCost { set; private get; }
    public int moneyCost{ set; private get; }

    private void Start() {
        mineralAmountManager = FindObjectOfType<MineralBarUi>();
        button = GetComponent<Button>();
    }

    private void Update() {
        button.enabled = mineralCost <= mineralAmountManager.CurrentMaxMinerals && moneyCost <= FactionManager.PlayerFaction.money;
    }

    private void OnDestroy() {
        if (mineralAmountManager != null) mineralAmountManager.ToggleBuildingCostTexts(false);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mineralAmountManager.ToggleBuildingCostTexts(true);
        mineralAmountManager.buildingName.text = Regex.Replace(buildingName, "(\\B[A-Z])", " $1");
        mineralAmountManager.mineralCost.text = mineralCost + "";
        mineralAmountManager.moneyCost.text = moneyCost + "";
    }
}
