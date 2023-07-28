using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockadeManagerBlock : FactionBlockWithSkips {

    public float CARGO_PERCENTAGE_TO_FORCE_TRADE;
    public float MINE_SELF_SUFFICIENCY_PERCENTAGE;
    public int MONEY_GENERATOR_AMOUNT_SELF_SUFFICIENCY_PERCENTAGE;
    public float DEFENSIVE_CIVILIAN_POSTURE_THRESHOLD;
    public float AGGRESSIVE_CIVILIAN_POSTURE_THRESHOLD;
    public float MAX_ENEMY_FACTION_SCORE;

    private BuildBuildingsBlock BuildingAi;

    public override void Initialize() {
        BuildingAi = GetComponent<BuildBuildingsBlock>();
    }

    public override void Block() {
        // If we do not have enough cargoes, resume trade
        if ((float) CargoShipManager.GetFactionCargoShipsCount(tag) / (float) FactionManager.Factions[tag].GetFactionDesiredCargoCount() < CARGO_PERCENTAGE_TO_FORCE_TRADE) {
            // print(tag + " STARTED TRADE BECAUSE NOT ENOUGH CARGOES");
            StartTradeWithAll();
            return;
        }

        // If we do not have enough mines, resume trade
        // print(tag + " HAS " + (float)BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.mine.ToString()].Count / (float)BuildingManager.Buildings[tag].Count + " PERCENTAGE OF MINE OUT OF ALL BUILDINGS");
        if ((float) BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.mine.ToString()].Count / (float) BuildingManager.Buildings[tag].Count < MINE_SELF_SUFFICIENCY_PERCENTAGE) {
            // print(tag + " STARTED TRADE BECAUSE NOT ENOUGH MINES");
            StartTradeWithAll();
            return;
        }

        // If we do not have enough money income, resume trade
        int moneyGeneratorsValues = 0;
        foreach (PlanetCaptureLogic planet in PlanetManager.FactionPlanets[tag]) moneyGeneratorsValues += planet.MoneyGenerator.factionMoneyValue;
        foreach (Selectable cc in BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.commandCenter.ToString()]) {
            MoneyGenerator moneyGenerator = cc.GetComponent<MoneyGenerator>();
            if (moneyGenerator != null) moneyGeneratorsValues += moneyGenerator.factionMoneyValue;
        }
        if (moneyGeneratorsValues < MONEY_GENERATOR_AMOUNT_SELF_SUFFICIENCY_PERCENTAGE) {
            print(tag + " STARTED TRADE BECAUSE NOT MONEY GENERATORS: " + moneyGeneratorsValues + " < " + MONEY_GENERATOR_AMOUNT_SELF_SUFFICIENCY_PERCENTAGE);
            StartTradeWithAll();
            return;
        }

        // If faction is too weak or powerfull
        float civilianScorePercentage = (float) FactionManager.FactionScoresManager.FactionCivilianScores[tag] / (float) FactionManager.FactionScoresManager.TotalFactionsCivilianScore();
        // print(civilianScorePercentage);
        if (civilianScorePercentage < DEFENSIVE_CIVILIAN_POSTURE_THRESHOLD && !RelationShipManager.IsFactionInWar(tag)) {
            // print(tag + " WENT DEFENSIVE TRADE BECAUSE THEY ARE TOO WEAK");
            // print(civilianScorePercentage);
            StartTradeWithAll();
            StopTradeWithBiggestFaction();
            return;
        } else if (civilianScorePercentage > AGGRESSIVE_CIVILIAN_POSTURE_THRESHOLD) {
            // print(tag + " STOPPED TRADING BECAUSE THEY ARE TOO STRONG");
            print(civilianScorePercentage);
            StopTradeWithAll();
            return;
        }

        // If we are not in war and other faction is over MAX_ENEMY_FACTION_SCORE, stop trading with them
        // If we are in a war, trade with all
        if (!RelationShipManager.IsFactionInWar(tag)) {
            foreach (string faction in FactionManager.Factions.Keys) {
                if (!CompareTag(faction) && (float)FactionManager.FactionScoresManager.FactionAssetScores[faction] / (float)FactionManager.FactionScoresManager.TotalFactionsAssetScore() > MAX_ENEMY_FACTION_SCORE) {
                    // print(tag + " STOPPED TRADING WITH " + faction + " BECAUSE THEY ARE TOO POWERFULL");
                    RelationShipManager.StartBlockade(tag, faction);
                }
            }
        } else {
            StartTradeWithAll();
        }
    }

    private void StartTradeWithAll() {
        foreach (Faction faction in FactionManager.Factions.Values) {
            if (!RelationShipManager.AreFactionsInWar(tag, faction.factionTag)) {
                RelationShipManager.EndBlockade(tag, faction.factionTag);
            }
        }
    }

    private void StopTradeWithAll() {
        foreach (Faction faction in FactionManager.Factions.Values) {
            if (!RelationShipManager.AreFactionsInWar(tag, faction.factionTag)) {
                RelationShipManager.StartBlockade(tag, faction.factionTag);
            }
        }
    }

    private void StopTradeWithBiggestFaction() {
        string biggestFaction = "";
        int civilianScore = int.MinValue;
        foreach (Faction faction in FactionManager.Factions.Values) {
            if (!CompareTag(faction.factionTag) && FactionManager.FactionScoresManager.FactionCivilianScores[faction.factionTag] > civilianScore) {
                biggestFaction = faction.factionTag;
                civilianScore = FactionManager.FactionScoresManager.FactionCivilianScores[faction.factionTag];
            }
        }

        if (biggestFaction != "") RelationShipManager.StartBlockade(tag, biggestFaction);
    }
}