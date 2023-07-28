using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour {

    // Dictionary that contains every building for a faction
    public static Dictionary<string, List<Hitpoints>> Buildings;
    // Dictionary that contains the amount of buildings for a faction of a Selectable -type
    public static Dictionary<string, Dictionary<string, List<Selectable>>> BuildingAmountsByFactionAndType;

    public static void Reset() {
        Buildings = new Dictionary<string, List<Hitpoints>>();
        BuildingAmountsByFactionAndType = new Dictionary<string, Dictionary<string, List<Selectable>>>();

        foreach (Faction faction in FactionManager.Factions.Values) {
            Buildings[faction.factionTag] = new List<Hitpoints>();
            BuildingAmountsByFactionAndType[faction.factionTag] = new Dictionary<string, List<Selectable>>();

            foreach (Selectable.Types type in System.Enum.GetValues(typeof(Selectable.Types))) {
                BuildingAmountsByFactionAndType[faction.factionTag][type.ToString()] = new List<Selectable>();
            }
        }
    }

    public static void RemoveFactionBuildings(string faction) {
        foreach (Hitpoints building in Buildings[faction]) {
            Destroy(building.gameObject);
        }
    }

    public static int GetBuildingTotalAmountByType(Selectable.Types type) {
        int amount = 0;
        foreach (string faction in FactionManager.Factions.Keys) {
            amount += BuildingAmountsByFactionAndType[faction][type.ToString()].Count;
        }

        return amount;
    }

    public static Hitpoints GetFactionRandomBuilding(string faction) {
        try {
            return Buildings[faction][Random.Range(0, Buildings[faction].Count)];
        } catch {
            return null;
        }
    }

    public static Selectable GetFactionRandomBuildingByType(string faction, Selectable.Types type) {
        try {
            return BuildingAmountsByFactionAndType[faction][type.ToString()][Random.Range(0, BuildingAmountsByFactionAndType[faction][type.ToString()].Count)];
        } catch {
            return null;
        }
    }

    public static float PercentageOfAllBuildingTypes(string tag, Selectable.Types type) {
        try {
            return BuildingAmountsByFactionAndType[tag][type.ToString()].Count / Buildings[tag].Count;
        } catch {
            return -1f;
        }
    }

    public static float BuildingsMineralStorageFillPercentage(string tag) {
        int potentialStorage = 0;
        int filledStorage = 0;

        foreach (Hitpoints building in Buildings[tag]) {
            MineralStorage storage = building.GetComponent<MineralStorage>();
            if (storage != null) {
                potentialStorage += storage.maxMineralStorage;
                filledStorage += storage.currentMineralStorage;
            }
        }

        return (float) filledStorage / (float) potentialStorage;
    }

    public static Vector2 GetFactionCenterPoint(string faction) {
        if (Buildings[faction].Count == 0) {
            return Vector2.zero;
        } else {
            Vector2 toReturn = Vector2.zero;
            foreach(Hitpoints building in Buildings[faction]) {
                toReturn += (Vector2)building.transform.position;
            }

            return toReturn / Buildings[faction].Count;
        }
    }
}
