using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandCenterMineralUpdater : MonoBehaviour {

    public Text TOTAL_MINERALS;
    public Text CURRENT_MAX_MINERALS;
    public float UPDATE_TIME;

    public int CurrentTotalMinerals { get; private set; }
    public int CurrentMaxMinerals { get; private set; }

    private void Start() {
        CurrentTotalMinerals = 0;
        CurrentMaxMinerals = 0;

        StartCoroutine(MineralUpdaterLoop());
    }

    private IEnumerator MineralUpdaterLoop() {
        while (true) {
            int currentTotal = 0;
            int currentMaxMinerals = 0;
            foreach (Selectable commandCenter in BuildingManager.BuildingAmountsByFactionAndType[FactionManager.PlayerFaction.factionTag][Selectable.Types.commandCenter.ToString()]) {
                MineralStorage storage = commandCenter.GetComponent<MineralStorage>();
                if (storage != null) {
                    currentTotal += storage.currentMineralStorage;
                    if (currentMaxMinerals < storage.currentMineralStorage) {
                        currentMaxMinerals = storage.currentMineralStorage;
                    }
                }
            }

            CurrentTotalMinerals = currentTotal;
            CurrentMaxMinerals = currentMaxMinerals;

            TOTAL_MINERALS.text = currentTotal.ToString();
            CURRENT_MAX_MINERALS.text = currentMaxMinerals.ToString();

            yield return new WaitForSeconds(UPDATE_TIME);
        }
    }

    public bool EnoughMineralsForAnotherBuilding(int mineralCost) {
        foreach (Selectable commandCenter in BuildingManager.BuildingAmountsByFactionAndType[FactionManager.PlayerFaction.factionTag][Selectable.Types.commandCenter.ToString()]) {
            MineralStorage storage = commandCenter.GetComponent<MineralStorage>();
            if (storage != null && storage.currentMineralStorage >= mineralCost) return true;
        }

        return false;
    }
}
