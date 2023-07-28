using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MineralBarUi : MonoBehaviour {
    public RectTransform maxMineralBarRoot;
    public RectTransform totalMineralBarRoot;
    public RectTransform maxMineralBarRootCost;
    public RectTransform totalMineralBarRootCost;
    public GameObject buildingCosts;
    public TMP_Text buildingName;
    public TMP_Text mineralCost;
    public TMP_Text moneyCost;
    public TMP_Text currentMaxMineralsText;
    public TMP_Text currentTotalMineralsText;
    public float UPDATE_TIME;

    public int CurrentTotalMinerals { get; private set; }
    public int TotalCommandCenterMaxMinerals { get; private set; }
    public int CurrentMaxMinerals { get; private set; }
    public int CommandCenterMaxMinerals { get; private set; }

    private void Start() {
        CurrentTotalMinerals = 0;
        CurrentMaxMinerals = 0;

        StartCoroutine(MineralUpdaterLoop());
    }

    private IEnumerator MineralUpdaterLoop() {
        while (true) {
            int currentTotal = 0;
            int currentMaxMinerals = 0;
            int totalCommandCenterMaxMinerals = 1;  // This is one because it cannot be zero, and we add other mineral capacities.
            int commandCenterMaxMinerals = 999999;  // This is big so that if we do not have command centers, divided value is basically zero.
            foreach (Selectable commandCenter in BuildingManager.BuildingAmountsByFactionAndType[FactionManager.PlayerFaction.factionTag][Selectable.Types.commandCenter.ToString()]) {
                MineralStorage storage = commandCenter.GetComponent<MineralStorage>();
                if (storage != null) {
                    currentTotal += storage.currentMineralStorage;
                    totalCommandCenterMaxMinerals += storage.maxMineralStorage;
                    if (currentMaxMinerals < storage.currentMineralStorage) {
                        currentMaxMinerals = storage.currentMineralStorage;
                        commandCenterMaxMinerals = storage.maxMineralStorage;
                    }
                }
            }

            CurrentTotalMinerals = currentTotal <= totalCommandCenterMaxMinerals ? currentTotal : totalCommandCenterMaxMinerals;
            TotalCommandCenterMaxMinerals = totalCommandCenterMaxMinerals;
            CurrentMaxMinerals = currentMaxMinerals <= commandCenterMaxMinerals ? currentMaxMinerals : commandCenterMaxMinerals;
            CommandCenterMaxMinerals = commandCenterMaxMinerals;

            maxMineralBarRoot.localScale = new Vector3(1, (float)CurrentMaxMinerals / (float)CommandCenterMaxMinerals, 1);
            totalMineralBarRoot.localScale = new Vector3(1, (float)CurrentTotalMinerals / (float)TotalCommandCenterMaxMinerals, 1);
            maxMineralBarRootCost.localScale = new Vector3(1, BuildingPlacementManager.IsBuilding ? (float)BuildingPlacementManager.Building.mineralCost / (float)CommandCenterMaxMinerals : 0, 1);
            totalMineralBarRootCost.localScale = new Vector3(1, BuildingPlacementManager.IsBuilding ? (float)BuildingPlacementManager.Building.mineralCost / (float)TotalCommandCenterMaxMinerals : 0, 1);

            currentMaxMineralsText.text = CurrentMaxMinerals + "";
            currentTotalMineralsText.text = CurrentTotalMinerals + "";

            yield return new WaitForSeconds(UPDATE_TIME);
        }
    }

    public void ToggleBuildingCostTexts(bool toggle) {
        buildingCosts.SetActive(toggle);
    }

    public bool EnoughMineralsForAnotherBuilding(int mineralCost) {
        foreach (Selectable commandCenter in BuildingManager.BuildingAmountsByFactionAndType[FactionManager.PlayerFaction.factionTag][Selectable.Types.commandCenter.ToString()]) {
            MineralStorage storage = commandCenter.GetComponent<MineralStorage>();
            if (storage != null && storage.currentMineralStorage >= mineralCost) return true;
        }

        return false;
    }
}
