using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FactionRemoval : MonoBehaviour, IScenarioEffect {
    public string FACTION;
    public void Effect() {
        Faction faction = FactionManager.Factions[FACTION];
        faction.ai.ON = false;
        BuildingManager.RemoveFactionBuildings(faction.factionTag);
        ShipsManager.RemoveFactionShips(faction.factionTag);
        PlanetManager.ResetFactionPlanetsToNeutral(faction.factionTag);
        RelationShipManager.RemoveFaction(faction.factionTag);
        FactionManager.RemoveFactionFromGame(faction.factionTag);
        FactionOpinionManager.RemoveFaction(faction.factionTag);
        GlobalMessageManager.GlobalMessage(faction.factionTag + " HAS BEEN DEFEATED");
    }
}
