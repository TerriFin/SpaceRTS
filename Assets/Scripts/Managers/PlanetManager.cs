using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {
    public static List<PlanetCaptureLogic> Planets;
    public static Dictionary<string, List<PlanetCaptureLogic>> FactionPlanets;

    public static void Reset() {
        Planets = new List<PlanetCaptureLogic>();
        FactionPlanets = new Dictionary<string, List<PlanetCaptureLogic>> {
            ["Untagged"] = new List<PlanetCaptureLogic>()
        };
        foreach (Faction faction in FactionManager.Factions.Values) {
            FactionPlanets[faction.factionTag] = new List<PlanetCaptureLogic>();
        }
    }

    public static List<PlanetCaptureLogic> GetPlanetsSortedToLocation(Vector2 location) {
        Planets.Sort(delegate (PlanetCaptureLogic first, PlanetCaptureLogic second) {
            return (int)(Vector2.Distance(first.transform.position, location) - Vector2.Distance(second.transform.position, location));
        });
        return Planets;
    }

    public static List<PlanetCaptureLogic> GetPlanetsFromFactionSortedByDistanceToLocation(string faction, Vector2 location) {
        List<PlanetCaptureLogic> factionPlanets = FactionPlanets[faction];
        factionPlanets.Sort(delegate (PlanetCaptureLogic first, PlanetCaptureLogic second) {
            return (int) (Vector2.Distance(first.transform.position, location) - Vector2.Distance(second.transform.position, location)); 
        });
        return factionPlanets;
    }

    public static PlanetCaptureLogic GetFactionRandomPlanet(string faction) {
        try {
            return FactionPlanets[faction][Random.Range(0, FactionPlanets[faction].Count)];
        } catch {
            return null;
        }
    }

    public static void ResetFactionPlanetsToNeutral(string faction) {
        foreach (PlanetCaptureLogic planet in FactionPlanets[faction]) {
            planet.ResetPlanetOwnership();
            FactionPlanets["Untagged"].Add(planet);
        }

        FactionPlanets.Remove(faction);
    }
}
