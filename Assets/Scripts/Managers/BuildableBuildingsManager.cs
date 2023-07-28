using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildableBuildingsManager : MonoBehaviour {

    // Dictionary with keys as factions, values as building that faction can build
    public static Dictionary<string, List<FactionBuilding>> BuildableBuildings;

    static BuildableBuildingsManager() {
        BuildableBuildings = new Dictionary<string, List<FactionBuilding>>();

        foreach (Faction faction in FactionManager.Factions.Values) {
            BuildableBuildings[faction.factionTag] = new List<FactionBuilding>();
        }

        FactionBuilding[] foundBuildings = Resources.LoadAll<FactionBuilding>("ScriptableObjects/FactionBuildings");

        foreach (FactionBuilding building in foundBuildings) {
            if (building != null && FactionManager.Factions.ContainsKey(building.faction)) {
                BuildableBuildings[building.faction].Add(building);
            }
        }
    }
}
