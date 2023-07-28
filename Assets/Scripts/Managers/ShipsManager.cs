using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipsManager : MonoBehaviour {

    public static Dictionary<string, List<Hitpoints>> CivShips;
    public static Dictionary<string, List<Hitpoints>> MilShips;

    public static void Reset() {
        CivShips = new Dictionary<string, List<Hitpoints>>();
        MilShips = new Dictionary<string, List<Hitpoints>>();

        foreach (Faction faction in FactionManager.Factions.Values) {
            CivShips[faction.factionTag] = new List<Hitpoints>();
            MilShips[faction.factionTag] = new List<Hitpoints>();
        }
    }

    public static void RemoveFactionShips(string faction) {
        foreach (Hitpoints civShip in CivShips[faction]) {
            Destroy(civShip.gameObject);
        }

        foreach (Hitpoints milShip in MilShips[faction]) {
            Destroy(milShip.gameObject);
        }
    }

    public static Hitpoints GetFactionRandomCivShip(string faction) {
        try {
            return CivShips[faction][Random.Range(0, CivShips[faction].Count)];
        } catch {
            return null;
        }
    }

    public static Hitpoints GetFactionRandomMilShip(string faction) {
        try {
            return MilShips[faction][Random.Range(0, MilShips[faction].Count)];
        } catch {
            return null;
        }
    }
}
