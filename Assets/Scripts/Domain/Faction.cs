using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction : MonoBehaviour {

    public string factionTag;
    public int money;
    public int maxMoney;
    public int mineMineralScore;
    public bool playerFaction;
    public float aiBonusMultiplier;
    public FactionAiBase ai;
    public Sprite factionLogo;
    public Sprite factionLeader;
    public Color factionColor;

    public GameObject smallCargo;
    public GameObject mediumCargo;
    public GameObject bigCargo;
    public GameObject frigate;
    public GameObject cruiser;

    public float smallCargoProductionTime;
    public float mediumCargoProductionTime;
    public float bigCargoProductionTime;

    public int smallCargoMineralPrice;
    public int smallCargoMoneyPrice;
    public int mediumCargoMineralPrice;
    public int mediumCargoMoneyPrice;
    public int bigCargoMineralPrice;
    public int bigCargoMoneyPrice;

    public int desiredSmallCargoes;
    public int desiredMediumCargoes;
    public int desiredBigCargoes;

    public void ModifyMoney(int amount) {
        money += amount;
        if (money > maxMoney) money = maxMoney;
        if (money < 0) money = 0;
    }

    public void ReduceMoneyToMax() {
        if (money > maxMoney) {
            money = maxMoney;
        }
    }

    public float FactionMoneyStorageFillPercentage() {
        return (float) money / (float) maxMoney;
    }

    public int GetFactionAssetScore() {
        int toReturn = 0;

        // Building Ships
        foreach (Hitpoints ship in ShipsManager.CivShips[factionTag]) {
            ConstructionShipAi constructionAi = ship.GetComponent<ConstructionShipAi>();
            if (constructionAi != null) {
                toReturn++;
            }
        }

        // Buildings
        foreach (Hitpoints building in BuildingManager.Buildings[factionTag]) {
            Selectable buildingData = building.GetComponent<Selectable>();
            toReturn += buildingData.selectableType switch {
                Selectable.Types.commandCenter => 20,
                Selectable.Types.mine => 14,
                Selectable.Types.frigateStation => 8,
                Selectable.Types.cruiserStation => 12,
                Selectable.Types.patrolStation => 4,
                Selectable.Types.raiderStation => 4,
                Selectable.Types.defenceStation => 3,
                _ => 0,
            };
        }

        // Planets
        foreach (PlanetCaptureLogic planet in PlanetManager.FactionPlanets[factionTag]) {
            toReturn += planet.MoneyGenerator.factionMoneyValue / 2;
        }

        // Military ships (small values)
        foreach (Hitpoints ship in ShipsManager.MilShips[factionTag]) {
            Selectable shipData = ship.GetComponent<Selectable>();
            toReturn += shipData.selectableType switch {
                Selectable.Types.frigate => 2,
                Selectable.Types.cruiser => 4,
                _ => 0,
            };
        }

        return toReturn;
    }

    public int GetFactionMilitaryScore() {
        int toReturn = 0;

        foreach (Hitpoints ship in ShipsManager.MilShips[factionTag]) {
            Selectable shipData = ship.GetComponent<Selectable>();
            toReturn += shipData.selectableType switch {
                Selectable.Types.frigate => 6,
                Selectable.Types.cruiser => 10,
                _ => 0,
            };
        }

        foreach (Hitpoints ship in ShipsManager.CivShips[factionTag]) {
            Selectable shipData = ship.GetComponent<Selectable>();
            toReturn += shipData.selectableType switch {
                Selectable.Types.police => 1,
                Selectable.Types.raider => 1,
                Selectable.Types.fighter => 0,
                _ => 0,
            };
        }

        return toReturn;
    }

    public int GetFactionCivilianScore() {
        int toReturn = 0;

        foreach (Hitpoints ship in ShipsManager.CivShips[factionTag]) {
            Selectable shipData = ship.GetComponent<Selectable>();
            toReturn += shipData.selectableType switch {
                Selectable.Types.smallCargo => 1,
                Selectable.Types.mediumCargo => 2,
                Selectable.Types.bigCargo => 4,
                _ => 0,
            };
        }

        foreach (PlanetCaptureLogic planet in PlanetManager.FactionPlanets[factionTag]) {
            toReturn += planet.GetComponent<MoneyGenerator>().factionMoneyValue;
        }

        toReturn += BuildingManager.BuildingAmountsByFactionAndType[factionTag][Selectable.Types.mine.ToString()].Count * 14;

        toReturn += money / 5000;

        return toReturn;
    }

    public int GetFactionMineralResourcesAmount() {
        int toReturn = 0;

        // Building ships
        foreach (Hitpoints ship in ShipsManager.CivShips[factionTag]) {
            ConstructionShipAi constructionAi = ship.GetComponent<ConstructionShipAi>();
            if (constructionAi != null) {
                toReturn += CheckBuildingMineralValue(constructionAi.Building);
            }
        }

        // Buildings
        foreach (Hitpoints building in BuildingManager.Buildings[factionTag]) {
            toReturn += CheckBuildingMineralValue(building.gameObject);
        }

        // Mines
        toReturn += BuildingManager.BuildingAmountsByFactionAndType[factionTag][Selectable.Types.mine.ToString()].Count * mineMineralScore;

        return toReturn;
    }

    public int GetFactionMoneyResourcesAmount() {
        int toReturn = 0;

        // Building ships
        foreach (Hitpoints ship in ShipsManager.CivShips[factionTag]) {
            ConstructionShipAi constructionAi = ship.GetComponent<ConstructionShipAi>();
            if (constructionAi != null) {
                toReturn += CheckBuildingMoneyValue(constructionAi.Building);
            }
        }

        // Buildings
        foreach (Hitpoints building in BuildingManager.Buildings[factionTag]) {
            toReturn += CheckBuildingMoneyValue(building.gameObject);
        }

        // Planets
        foreach (PlanetCaptureLogic planet in PlanetManager.FactionPlanets[factionTag]) {
            MoneyGenerator moneyGenerator = planet.GetComponent<MoneyGenerator>();
            if (moneyGenerator != null) {
                toReturn += moneyGenerator.factionMoneyValue * MoneyGenerator.FACTION_MONEY_SCORE_PER_POINT;
            }
        }

        return toReturn;
    }

    private int CheckBuildingMineralValue(GameObject building) {
        int toReturn = 0;

        Production production = building.GetComponent<Production>();
        if (production != null) {
            toReturn += production.FactionMineralValue;
        }

        return toReturn;
    }

    private int CheckBuildingMoneyValue(GameObject building) {
        int toReturn = 0;

        Production production = building.GetComponent<Production>();
        if (production != null) {
            toReturn += production.FactionMoneyValue;
        }

        MoneyGenerator moneyGenerator = building.GetComponent<MoneyGenerator>();
        if (moneyGenerator != null) {
            toReturn += moneyGenerator.factionMoneyValue * MoneyGenerator.FACTION_MONEY_SCORE_PER_POINT;
        }

        return toReturn;
    }

    public int GetFactionDesiredCargoCount() {
        return desiredSmallCargoes + desiredMediumCargoes + desiredBigCargoes;
    }
}
