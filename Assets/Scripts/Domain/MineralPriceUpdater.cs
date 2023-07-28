using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralPriceUpdater : MonoBehaviour {

    public int PRICE;
    public float UPDATE_TIMER;

    // CurrentSetPrice is for player set prices
    private int CurrentSetPrice;
    private Market AttachedMarket;
    private Production AttachedProduction;
    private MineralStorage AttachedStorage;
    public bool AutomaticallyUpdatingPrice { get; private set; }

    private void Start() {
        CurrentSetPrice = MineralPriceManager.StandardMineralPrice;
        AttachedMarket = GetComponent<Market>();
        AttachedProduction = GetComponent<Production>();
        AttachedStorage = GetComponent<MineralStorage>();
        AutomaticallyUpdatingPrice = true;

        // If there is attached production (building actively uses minerals) start updating the price.
        // Otherwise (If the building is command station) set price to be standard + 5
        if (AttachedProduction != null) {
            if (PRICE == 0) StartCoroutine(StartUpdatingPrice());
            else AttachedMarket.currentMineralPrice = PRICE;
        } else {
            AttachedMarket.currentMineralPrice = MineralPriceManager.StandardMineralPrice + 5;
        }
    }

    // This is called when player modifies price
    public bool IncreasePrice(int amount) {
        if (CurrentSetPrice + amount <= MineralPriceManager.MaxMineralPrice) {
            CurrentSetPrice += amount;
            AttachedMarket.currentMineralPrice = CurrentSetPrice;
            return true;
        }

        return false;
    }

    // This is called when player modifies price
    public bool DecreasePrice(int amount) {
        if (CurrentSetPrice - amount >= MineralPriceManager.MinMineralPrice) {
            CurrentSetPrice -= amount;
            AttachedMarket.currentMineralPrice = CurrentSetPrice;
            return true;
        }

        return false;
    }

    public void ToggleAutomaticPricing(bool toggle) {
        AutomaticallyUpdatingPrice = toggle;
    }

    // Does not do anything if gameobject does not have production script (mines, commandcenters, etc..)
    private IEnumerator StartUpdatingPrice() {
        while (true) {
            yield return new WaitForSeconds(UPDATE_TIMER);
            if (AutomaticallyUpdatingPrice) {
                if (AttachedMarket.buying) {
                    if (!AttachedProduction.EnoughMineralsToProduce()) {
                        if (AttachedMarket.currentMineralPrice < MineralPriceManager.MaxMineralPrice) {
                            AttachedMarket.currentMineralPrice++;
                        }
                    } else {
                        AttachedMarket.currentMineralPrice = CurrentSetPrice;
                    }
                } else if (AttachedMarket.selling) {
                    if (AttachedStorage.currentMineralStorage > AttachedStorage.maxMineralStorage / 2) {
                        if (AttachedMarket.currentMineralPrice > MineralPriceManager.MinMineralPrice) {
                            AttachedMarket.currentMineralPrice--;
                        }
                    } else {
                        AttachedMarket.currentMineralPrice = CurrentSetPrice;
                    }
                }
            }
        }
    }
}
