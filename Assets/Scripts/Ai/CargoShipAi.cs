using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoShipAi : MonoBehaviour, IAi {

    public float distanceModifier;
    public int ownFactionBuyMineralsPreference;
    public int ownFactionSellMineralsPreference;
    public int otherFactionBuyMineralsPreference;
    public int otherFactionSellMineralsPreference;
    public int randomness;
    public float refreshTime;
    public float marketFillPercentage;
    public float neededDealMoneyMultiplier;

    public bool smallCargo;
    public bool mediumCargo;
    public bool bigCargo;

    private ShipMovement Controls;
    private MineralStorage Storage;
    private Sensors Sensors;
    private Market TargetMarket;

    private void OnDestroy() {
        if (smallCargo) {
            CargoShipManager.SmallCargoes[tag]--;
        } else if (mediumCargo) {
            CargoShipManager.MediumCargoes[tag]--;
        } else if (bigCargo) {
            CargoShipManager.BigCargoes[tag]--;
        }
    }

    public void InitializeAi() {
        Controls = GetComponent<ShipMovement>();
        Storage = GetComponent<MineralStorage>();
        Sensors = GetComponent<Sensors>();

        if (smallCargo) {
            CargoShipManager.SmallCargoes[tag]++;
        } else if (mediumCargo) {
            CargoShipManager.MediumCargoes[tag]++;
        } else if (bigCargo) {
            CargoShipManager.BigCargoes[tag]++;
        }

        StartCoroutine(NewMarketTimer());
    }

    public void ExecuteStep() {
        if (Sensors.ArmedEnemies.Count / 2 > Sensors.ArmedAllies.Count) {
            if (Controls.ORIGIN != null) {
                if (TargetMarket != null) {
                    TargetMarket.ReportTradeDisruption(tag);
                    TargetMarket = null;
                }
                Controls.SetSecondaryTargetPos(Controls.ORIGIN.transform.position);
            } else if (Sensors.GetClosestArmedEnemy() != null) {
                if (TargetMarket != null) {
                    TargetMarket.ReportTradeDisruption(tag);
                    TargetMarket = null;
                }
                Controls.SetSecondaryTargetPos(Sensors.GetClosestArmedEnemy().transform.position + (transform.position - Sensors.GetClosestArmedEnemy().transform.position).normalized * 12);
            }
        } else {
            if (TargetMarket == null) {
                TargetMarket = GetNewMarket();
            } else {
                if (Controls.AreWeThereYet()) {
                    if (TargetMarket.buying) {
                        TargetMarket.BuyMinerals(Storage);
                        TargetMarket = null;
                    } else if (TargetMarket.selling) {
                        TargetMarket.SellMinerals(Storage);
                        TargetMarket = null;
                    }
                } else {
                    if (TargetMarket.buying && (TargetMarket.Storage.FreeStorage() < Storage.currentMineralStorage / 1.5f || (!TargetMarket.CompareTag(tag) && FactionManager.Factions[TargetMarket.tag].money < TargetMarket.currentMineralPrice * Storage.FreeStorage() * 2))) {
                        TargetMarket = GetNewMarket();
                    } else if (TargetMarket.selling && (TargetMarket.Storage.currentMineralStorage < Storage.FreeStorage() / 2 || (!TargetMarket.CompareTag(tag) && FactionManager.Factions[tag].money < TargetMarket.currentMineralPrice * Storage.FreeStorage() * 2))) {
                        TargetMarket = GetNewMarket();
                    }
                }
            }
        }
    }

    private Market GetNewMarket() {
        if (!FactionManager.Factions.ContainsKey(tag)) {
            Destroy(gameObject);
            return null;
        }

        Market toReturn = null;
        int marketValue;
        if (Storage.currentMineralStorage < Storage.maxMineralStorage * 0.6f) {
            // Handle buying here
            marketValue = int.MaxValue;
            foreach (Market market in MarketManager.sellingMarkets) {
                if (!RelationShipManager.AreFactionsBlockading(tag, market.tag) && market.Storage.currentMineralStorage >= Storage.FreeStorage() * marketFillPercentage && 
                    (market.CompareTag(tag) || FactionManager.Factions[tag].money >= Storage.currentMineralStorage * market.currentMineralPrice * neededDealMoneyMultiplier)) {
                    int currentMarketValue = market.currentMineralPrice + (int)(Vector2.Distance(transform.position, market.transform.position) * distanceModifier) - market.CurrentTradeDisruptions[tag] + Random.Range(-randomness, randomness);
                    // If own faction, buff value
                    if (market.CompareTag(tag)) {
                        currentMarketValue -= ownFactionBuyMineralsPreference;
                    } else {
                        currentMarketValue -= otherFactionBuyMineralsPreference;
                    }
                    if (currentMarketValue < marketValue) {
                        toReturn = market;
                        marketValue = currentMarketValue;
                    }
                }
            }
        } else {
            // Handle selling here
            bool moneyStorageNotFull = FactionManager.Factions[tag].FactionMoneyStorageFillPercentage() <= 0.9f;
            marketValue = int.MinValue;
            foreach (Market market in MarketManager.buyingMarkets) {
                if (!RelationShipManager.AreFactionsBlockading(tag, market.tag) && market.Storage.FreeStorage() >= Storage.currentMineralStorage * marketFillPercentage &&
                    (market.CompareTag(tag) || (moneyStorageNotFull && FactionManager.Factions[market.tag].money >= Storage.currentMineralStorage * neededDealMoneyMultiplier))) {
                    int currentMarketValue = market.currentMineralPrice - (int)(Vector2.Distance(transform.position, market.transform.position) * distanceModifier) + market.CurrentTradeDisruptions[tag] + Random.Range(-randomness, randomness);
                    // If own faction, buff value
                    if (market.CompareTag(tag)) {
                        currentMarketValue += ownFactionSellMineralsPreference;
                    } else {
                        currentMarketValue += otherFactionSellMineralsPreference;
                    }
                    if (currentMarketValue > marketValue) {
                        toReturn = market;
                        marketValue = currentMarketValue;
                    }
                }
            }
        }

        if (toReturn != null) Controls.SetPrimaryTargetPos(toReturn.transform.position);
        return toReturn;
    }

    private IEnumerator NewMarketTimer() {
        while (true) {
            yield return new WaitForSeconds(refreshTime);
            TargetMarket = GetNewMarket();
        }
    }
}
