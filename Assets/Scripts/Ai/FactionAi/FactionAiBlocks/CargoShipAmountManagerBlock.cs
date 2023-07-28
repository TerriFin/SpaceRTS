using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoShipAmountManagerBlock : FactionBlockWithSkips {

    public int COMMAND_CENTER_CARGO_POINTS;
    public int MINE_CARGO_POINTS;
    public int PRODUCTION_CARGO_POINTS;

    public int SMALL_CARGO_POINT_COST;
    public int MEDIUM_CARGO_POINT_COST;
    public int BIG_CARGO_POINT_COST;

    public float PEACE_SMALL_CARGO_PERCENTAGE;
    public float WAR_SMALL_CARGO_PERCENTAGE;
    public float PEACE_BIG_CARGO_PERCENTAGE;
    public float WAR_BIG_CARGO_PERCENTAGE;

    private float PEACE_MEDIUM_CARGO_PERCENTAGE;
    private float WAR_MEDIUM_CARGO_PERCENTAGE;

    public override void Initialize() {
        PEACE_MEDIUM_CARGO_PERCENTAGE = 1f - PEACE_SMALL_CARGO_PERCENTAGE - PEACE_BIG_CARGO_PERCENTAGE;
        WAR_MEDIUM_CARGO_PERCENTAGE = 1f - WAR_SMALL_CARGO_PERCENTAGE - WAR_BIG_CARGO_PERCENTAGE;
    }
    public override void Block() {
        bool isThereActiveTradePartner = RelationShipManager.DoesFactionHaveTradingPartner(tag);
        int currentAvailableCargoPoints = CalculateAvailableCargoPoints(isThereActiveTradePartner);
        if (RelationShipManager.IsFactionInWar(tag)) {
            // If only two factions, small cargoes are just worse.
            if (FactionManager.FactionScoresManager.GetActiveFactionCount() <= 2 || !isThereActiveTradePartner) {
                SetNewDesiredCargoShips(0, (int)(currentAvailableCargoPoints * WAR_SMALL_CARGO_PERCENTAGE) + (int)(currentAvailableCargoPoints * WAR_MEDIUM_CARGO_PERCENTAGE), (int)(currentAvailableCargoPoints * WAR_BIG_CARGO_PERCENTAGE));
            } else {
                SetNewDesiredCargoShips((int)(currentAvailableCargoPoints * WAR_SMALL_CARGO_PERCENTAGE), (int)(currentAvailableCargoPoints * WAR_MEDIUM_CARGO_PERCENTAGE), (int)(currentAvailableCargoPoints * WAR_BIG_CARGO_PERCENTAGE));
            }
        } else {
            // If only two factions, small cargoes are just worse.
            if (FactionManager.FactionScoresManager.GetActiveFactionCount() <= 2 || !isThereActiveTradePartner) {
                SetNewDesiredCargoShips(0, (int)(currentAvailableCargoPoints * PEACE_SMALL_CARGO_PERCENTAGE) + (int)(currentAvailableCargoPoints * PEACE_MEDIUM_CARGO_PERCENTAGE), (int)(currentAvailableCargoPoints * PEACE_BIG_CARGO_PERCENTAGE));
            } else {
                SetNewDesiredCargoShips((int)(currentAvailableCargoPoints * PEACE_SMALL_CARGO_PERCENTAGE), (int)(currentAvailableCargoPoints * PEACE_MEDIUM_CARGO_PERCENTAGE), (int)(currentAvailableCargoPoints * PEACE_BIG_CARGO_PERCENTAGE));
            }
        }
    }

    private void SetNewDesiredCargoShips(int smallCargoPoints, int mediumCargoPoints, int bigCargoPoints) {
        FactionManager.Factions[tag].desiredSmallCargoes = smallCargoPoints / SMALL_CARGO_POINT_COST;
        FactionManager.Factions[tag].desiredMediumCargoes = mediumCargoPoints / MEDIUM_CARGO_POINT_COST;
        FactionManager.Factions[tag].desiredBigCargoes = bigCargoPoints / BIG_CARGO_POINT_COST;
    }

    private int CalculateAvailableCargoPoints(bool isThereActiveTradePartner) {
        int toReturn = 0;
        if (isThereActiveTradePartner) {
            foreach (Hitpoints building in BuildingManager.Buildings[tag]) {
                Selectable buildingData = building.GetComponent<Selectable>();

                if (buildingData.selectableType == Selectable.Types.commandCenter) {
                    toReturn += COMMAND_CENTER_CARGO_POINTS;
                } else if (buildingData.selectableType == Selectable.Types.mine) {
                    toReturn += MINE_CARGO_POINTS;
                } else if (buildingData.selectableType != Selectable.Types.defenceStation) {
                    toReturn += PRODUCTION_CARGO_POINTS;
                }
            }
        } else {
            foreach (Hitpoints building in BuildingManager.Buildings[tag]) {
                Selectable buildingData = building.GetComponent<Selectable>();

                if (buildingData.selectableType == Selectable.Types.mine) {
                    toReturn += MINE_CARGO_POINTS;
                }
            }
        }

        return toReturn;
    }
}
