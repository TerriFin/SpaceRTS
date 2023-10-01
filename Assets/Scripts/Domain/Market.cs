using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour {
    public int currentMineralPrice { get; set; }
    public MineralStorage Storage { get; private set; }
    public Dictionary<string, int> CurrentTradeDisruptions { get; private set; }
    public float tradeDisruptionResetTimer;
    public bool selling;
    public bool buying;

    private void Start() {
        if (currentMineralPrice == 0) {
            currentMineralPrice = MineralPriceManager.StandardMineralPrice;
        }

        Storage = GetComponent<MineralStorage>();
        CurrentTradeDisruptions = new Dictionary<string, int>();
        foreach (string faction in FactionManager.Factions.Keys) {
            CurrentTradeDisruptions[faction] = 0;
        }

        if (selling) {
            MarketManager.sellingMarkets.Add(this);
        }

        if (buying) {
            MarketManager.buyingMarkets.Add(this);
        }

        StartCoroutine(StartFactionBiasTimer());
    }

    private void OnDestroy() {
        if (selling) {
            MarketManager.sellingMarkets.Remove(this);
        }

        if (buying) {
            MarketManager.buyingMarkets.Remove(this);
        }
    }

    private IEnumerator StartFactionBiasTimer() {
        while (true) {
            yield return new WaitForSeconds(tradeDisruptionResetTimer);
            foreach (string faction in FactionManager.Factions.Keys) {
                if (CurrentTradeDisruptions[faction] < 0) {
                    CurrentTradeDisruptions[faction] += 1;
                }
            }
        }

    }

    public void ReportTradeDisruption(string faction) {
        if (CurrentTradeDisruptions[faction] > -100) {
            CurrentTradeDisruptions[faction] -= 30;
        }
    }

    public void SellMinerals(MineralStorage trader) {
        int amountToSell = Storage.currentMineralStorage >= trader.FreeStorage() ? trader.FreeStorage() : Storage.currentMineralStorage;
        int price = currentMineralPrice * amountToSell;

        if (trader.tag == tag || FactionManager.Factions[trader.tag].money >= price) {
            FactionManager.Factions[trader.tag].money -= price;
            FactionManager.Factions[tag].ModifyMoney(price);

            Storage.TakeMinerals(amountToSell);
            trader.GiveMinerals(amountToSell);

            if (trader.CompareTag(tag)) {
                FactionManager.Factions[tag].ModifyMoney((int)(amountToSell * 2 * FactionManager.Factions[tag].aiBonusMultiplier));
            } else {
                FactionManager.Factions[tag].ModifyMoney((int)(amountToSell * 4 * FactionManager.Factions[tag].aiBonusMultiplier));
                FactionOpinionManager.ModifyFactionOpinion(tag, trader.tag, (float) amountToSell / 50.0f);
                FactionOpinionManager.ModifyFactionOpinion(trader.tag, tag, (float) amountToSell / 100.0f);
            }
        }
    }

    public void BuyMinerals(MineralStorage trader) {
        int amountToBuy = trader.currentMineralStorage >= Storage.FreeStorage() ? Storage.FreeStorage() : trader.currentMineralStorage;
        int price = currentMineralPrice * amountToBuy;

        if (trader.tag == tag || FactionManager.Factions[tag].money >= price) {
            FactionManager.Factions[trader.tag].ModifyMoney(price);
            FactionManager.Factions[tag].money -= price;

            Storage.GiveMinerals(amountToBuy);
            trader.TakeMinerals(amountToBuy);

            if (trader.CompareTag(tag)) {
                FactionManager.Factions[tag].ModifyMoney((int)(amountToBuy * 3 * FactionManager.Factions[tag].aiBonusMultiplier));
            } else {
                FactionManager.Factions[tag].ModifyMoney((int)(amountToBuy * 6 * FactionManager.Factions[tag].aiBonusMultiplier));
                FactionOpinionManager.ModifyFactionOpinion(tag, trader.tag, (float) amountToBuy / 50.0f);
                FactionOpinionManager.ModifyFactionOpinion(trader.tag, tag, (float) amountToBuy / 100.0f);
            }
        }
    }
}
