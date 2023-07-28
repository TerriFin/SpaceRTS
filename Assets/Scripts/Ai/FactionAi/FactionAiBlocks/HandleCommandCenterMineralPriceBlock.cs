using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCommandCenterMineralPriceBlock : FactionBlockWithSkips {
    public int WANTED_MINERAL_AMOUNT;
    public int PRICE_CHANGE_AMOUNT;

    private int CurrentPrice;
    public override void Initialize() {
        CurrentPrice = MineralPriceManager.StandardMineralPrice;
    }
    public override void Block() {
        if (GetLargestCommandCenterMineralStorage() < WANTED_MINERAL_AMOUNT) {
            CurrentPrice += PRICE_CHANGE_AMOUNT;
            if (CurrentPrice > MineralPriceManager.MaxMineralPrice) CurrentPrice = MineralPriceManager.MaxMineralPrice;
        } else {
            CurrentPrice = MineralPriceManager.StandardMineralPrice;
        }

        UpdateAllCommandCenterPrices();
    }

    private int GetLargestCommandCenterMineralStorage() {
        int currentLargest = 0;
        foreach (Selectable commandCenter in BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.commandCenter.ToString()]) {
            MineralStorage storage = commandCenter.GetComponent<MineralStorage>();
            if (storage != null) {
                int commandCenterMineralAmount = storage.currentMineralStorage;
                if (currentLargest < commandCenterMineralAmount) {
                    currentLargest = commandCenterMineralAmount;
                }
            }
        }

        return currentLargest;
    }
    
    private void UpdateAllCommandCenterPrices() {
        foreach (Selectable commandCenter in BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.commandCenter.ToString()]) {
            Market market = commandCenter.GetComponent<Market>();
            if (market != null) {
                market.currentMineralPrice = CurrentPrice;
            }
        }
    }
}
