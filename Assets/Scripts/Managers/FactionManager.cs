using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour {

    public static Dictionary<string, Faction> Factions { get; private set; }
    public static Faction PlayerFaction { get; set; }
    public static FactionScoresManager FactionScoresManager { get; private set; }

    public static void Reset() {
        Factions = new Dictionary<string, Faction>();
        PlayerFaction = null;
        FactionScoresManager = null;

        GameObject factionsGameobject = GameObject.FindGameObjectWithTag("FactionManager");
        List<Faction> foundFactions = new List<Faction>(factionsGameobject.GetComponents<Faction>());
        MapGeneratorManager mapGenerator = FindObjectOfType<MapGeneratorManager>();
        bool playerInGame = false;
        foreach (Faction faction1 in foundFactions) {
            if (mapGenerator == null || mapGenerator.FactionDatas[faction1.factionTag].statusIndex != 0) {
                Factions[faction1.factionTag] = faction1;
                if (faction1.playerFaction) {
                    PlayerFaction = faction1;
                    MusicManager.PlayTheme(faction1.factionTag);
                    playerInGame = true;
                }
            }
        }

        if (!playerInGame) MusicManager.PlayTheme(foundFactions[Random.Range(0, foundFactions.Count)].factionTag);

        FactionScoresManager = factionsGameobject.GetComponent<FactionScoresManager>();
    }

    public static List<string> GetFactionsInRandomOrder() {
        List<string> toReturn = new List<string>(Factions.Keys);
        toReturn.Shuffle();
        return toReturn;
    }

    public static void RemoveFactionFromGame(string faction) {
        if (PlayerFaction == Factions[faction]) PlayerFaction = null;
        Factions.Remove(faction);
    }

    public static string HasFactionWon(float victoryThresholdMultiplier) {
        foreach (Faction faction in Factions.Values) {
            if (FactionScoresManager.FactionAssetScores[faction.factionTag] >= FactionScoresManager.TotalFactionsAssetScore() * victoryThresholdMultiplier) {
                return faction.factionTag;
            }
        }

        return null;
    }
}
