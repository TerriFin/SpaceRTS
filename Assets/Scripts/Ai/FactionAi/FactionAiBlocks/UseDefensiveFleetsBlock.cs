using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ICallHelp is searched for in FactionAiBase and called when enemy fleets sighted
public class UseDefensiveFleetsBlock : FactionBlockWithSkips, ICallHelp {

    public int COMMAND_CENTER_DEFENCE_FLEETS_AMOUNT;
    public float PLANET_PATROL_PERCENTAGE;

    private FactionAiFleetManager FleetManager;
    private WarManagerBlock WarManager;

    public override void Initialize() {
        FleetManager = GetComponent<FactionAiFleetManager>();
        WarManager = GetComponent<WarManagerBlock>();
    }

    public override void Block() {
        int currentDefenceFleetsInCommandCenters = 0;
        int commandCentersAmount = BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.commandCenter.ToString()].Count;
        foreach (Fleet fleet in FleetManager.DefenceFleets) {
            if ((WarManager == null || !WarManager.TotalWarTriggered) && currentDefenceFleetsInCommandCenters < COMMAND_CENTER_DEFENCE_FLEETS_AMOUNT) {
                if (commandCentersAmount > 0) {
                    fleet.MoveOrder(BuildingManager.GetFactionRandomBuildingByType(tag, Selectable.Types.commandCenter).transform.position, tag, false);
                } else {
                    currentDefenceFleetsInCommandCenters = COMMAND_CENTER_DEFENCE_FLEETS_AMOUNT;
                }

                currentDefenceFleetsInCommandCenters++;
            } else {
                if (Random.Range(0.0f, 1.0f) < PLANET_PATROL_PERCENTAGE) {
                    PlanetCaptureLogic planet = PlanetManager.GetFactionRandomPlanet(tag);
                    if (planet != null) fleet.MoveOrder(planet.transform.position, tag);
                } else {
                    Hitpoints building = BuildingManager.GetFactionRandomBuilding(tag);
                    if (building != null) fleet.MoveOrder(building.transform.position, tag);
                }
            }
        }
    }

    public void CallForHelp(Vector2 location, int enemyAmount, bool important = false) {
        if (enemyAmount >= FleetManager.defenceFleetSize) {
            int shipAmount = 0;
            foreach (Fleet fleet in FleetManager.DefenceFleets) {
                if (fleet.MoveOrder(location, tag, false)) {
                    shipAmount += fleet.fleetSize;
                    if (shipAmount >= enemyAmount) break;
                }
            }
        }
    }

    public void CheckLocation(Vector2 location, int fleetAmount, string targetFaction = null) {
        int currentSentFleets = 0;

        foreach (Fleet fleet in FleetManager.DefenceFleets) {
            if (fleet != null && location != null && fleet.MoveOrder(location, targetFaction)) {
                currentSentFleets++;
                if (currentSentFleets >= fleetAmount) break;
            }
        }
    }
}
