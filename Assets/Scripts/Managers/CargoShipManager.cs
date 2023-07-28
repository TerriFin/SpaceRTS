using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoShipManager : MonoBehaviour {

    public static Dictionary<string, int> SmallCargoes;
    public static Dictionary<string, int> MediumCargoes;
    public static Dictionary<string, int> BigCargoes;

    public static Dictionary<string, List<CommandCenterCargoProduction>> SmallCargoesBeingBuilt;
    public static Dictionary<string, List<CommandCenterCargoProduction>> MediumCargoesBeingBuilt;
    public static Dictionary<string, List<CommandCenterCargoProduction>> BigCargoesBeingBuilt;

    public static void Reset() {
        SmallCargoes = new Dictionary<string, int>();
        MediumCargoes = new Dictionary<string, int>();
        BigCargoes = new Dictionary<string, int>();

        SmallCargoesBeingBuilt = new Dictionary<string, List<CommandCenterCargoProduction>>();
        MediumCargoesBeingBuilt = new Dictionary<string, List<CommandCenterCargoProduction>>();
        BigCargoesBeingBuilt = new Dictionary<string, List<CommandCenterCargoProduction>>();

        foreach (Faction faction in FactionManager.Factions.Values) {
            SmallCargoes[faction.factionTag] = 0;
            MediumCargoes[faction.factionTag] = 0;
            BigCargoes[faction.factionTag] = 0;

            SmallCargoesBeingBuilt[faction.factionTag] = new List<CommandCenterCargoProduction>();
            MediumCargoesBeingBuilt[faction.factionTag] = new List<CommandCenterCargoProduction>();
            BigCargoesBeingBuilt[faction.factionTag] = new List<CommandCenterCargoProduction>();
        }
    }

    public static int GetFactionCargoShipsCount(string faction) {
        return SmallCargoes[faction] + MediumCargoes[faction] + BigCargoes[faction];
    }
}
