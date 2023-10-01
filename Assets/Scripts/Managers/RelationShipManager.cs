using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationShipManager : MonoBehaviour {

    [System.Serializable]
    public struct RelationShipStatus {
        public string faction1;
        public string faction2;
    }

    public static Dictionary<string, List<string>> War;
    public static Dictionary<string, List<string>> Blockade;
    public static bool Locked = false;

    public bool LOCKED;
    public List<RelationShipStatus> WARS;
    public List<RelationShipStatus> BLOCKADES;
    private void Start() {
        Locked = LOCKED;
        War = new Dictionary<string, List<string>>();
        Blockade = new Dictionary<string, List<string>>();

        foreach (RelationShipStatus status in WARS) {
            if (!War.ContainsKey(status.faction1)) War[status.faction1] = new List<string>();
            if (!War.ContainsKey(status.faction2)) War[status.faction2] = new List<string>();
            StartWar(status.faction1, status.faction2, true);
        }

        foreach (RelationShipStatus status in BLOCKADES) {
            if (!Blockade.ContainsKey(status.faction1)) Blockade[status.faction1] = new List<string>();
            if (!Blockade.ContainsKey(status.faction2)) Blockade[status.faction2] = new List<string>();
            StartBlockade(status.faction1, status.faction2, true);
        }

        foreach (Faction faction in FactionManager.Factions.Values) {
            if (!War.ContainsKey(faction.factionTag)) War[faction.factionTag] = new List<string>();
            if (!Blockade.ContainsKey(faction.factionTag)) Blockade[faction.factionTag] = new List<string>();
        }
    }

    public static void RemoveFaction(string faction) {
        War.Remove(faction);
        Blockade.Remove(faction);

        foreach (string factionName in War.Keys) {
            War[factionName].Remove(faction);
        }

        foreach (string factionName in Blockade.Keys) {
            Blockade[factionName].Remove(faction);
        }
    }

    public static void StartWar(string startingFaction, string targetFaction, bool ignoreLock = false) {
        if ((!Locked || ignoreLock) && startingFaction != targetFaction) {
            if (!War[startingFaction].Contains(targetFaction)) {
                War[startingFaction].Add(targetFaction);
                FactionOpinionManager.ModifyFactionOpinion(startingFaction, targetFaction, -75);

                // This is so that both factions need to end the war for it to be over.
                if (!War[targetFaction].Contains(startingFaction) && !(FactionManager.PlayerFaction != null && FactionManager.PlayerFaction.factionTag == targetFaction)) {
                    War[targetFaction].Add(startingFaction);
                    FactionOpinionManager.ModifyFactionOpinion(targetFaction, startingFaction, -150);
                }

                GlobalMessageManager.GlobalMessage(startingFaction + " HAS STARTED WAR AGAINST " + targetFaction);
            }
        }
    }

    public static void EndWar(string endingFaction, string targetFaction, bool ignoreLock = false) {
        if ((!Locked || ignoreLock) && endingFaction != targetFaction) {
            if (War[endingFaction].Contains(targetFaction)) {
                War[endingFaction].Remove(targetFaction);
                FactionOpinionManager.ModifyFactionOpinion(targetFaction, endingFaction, 50);

                GlobalMessageManager.GlobalMessage(endingFaction + " HAS STOPPED FIGHTING AGAINST " + targetFaction);
            }
        }
    }

    public static void StartBlockade(string startingFaction, string targetFaction, bool ignoreLock = false) {
        if ((!Locked || ignoreLock) && startingFaction != targetFaction) {
            if (!Blockade[startingFaction].Contains(targetFaction)) {
                Blockade[startingFaction].Add(targetFaction);
                FactionOpinionManager.ModifyFactionOpinion(targetFaction, startingFaction, -45);

                GlobalMessageManager.GlobalMessage(startingFaction + " HAS STARTED A BLOCKADE AGAINST " + targetFaction);
            }
        }
    }

    public static void EndBlockade(string endingFaction, string targetFaction, bool ignoreLock = false) {
        if ((!Locked || ignoreLock) && endingFaction != targetFaction) {
            if (Blockade[endingFaction].Contains(targetFaction)) {
                Blockade[endingFaction].Remove(targetFaction);
                FactionOpinionManager.ModifyFactionOpinion(targetFaction, endingFaction, 20);

                GlobalMessageManager.GlobalMessage(endingFaction + " HAS STOPPED BLOCKADING " + targetFaction);
            }
        }
    }

    public static bool IsFactionAttackingFaction(string faction1, string faction2) {
        return War[faction1].Contains(faction2);
    }

    public static bool IsFactionBlockadingFaction(string faction1, string faction2) {
        return Blockade[faction1].Contains(faction2);
    }

    public static bool AreFactionsInWar(string faction1, string faction2) {
        try {
            return War[faction1].Contains(faction2) || War[faction2].Contains(faction1);
        } catch {
            return false;
        }
    }

    public static bool AreFactionsBlockading(string faction1, string faction2) {
        try {
            return AreFactionsInWar(faction1, faction2) || Blockade[faction1].Contains(faction2) || Blockade[faction2].Contains(faction1);
        } catch {
            return false;
        }
    }

    public static bool IsFactionInWar(string faction) {
        foreach (string f in War.Keys) {
            if (f == faction) {
                if (War[f].Count > 0) return true;
            } else {
                if (War[f].Contains(faction)) return true;
            }
        }

        return false;
    }

    public static HashSet<string> HowManyWarsIsFactionIn(string faction) {
        HashSet<string> warFactions = new HashSet<string>();
        foreach (string f in War.Keys) {
            if (f == faction) {
                foreach (string warFaction in War[f]) warFactions.Add(warFaction);
            } else {
                if (War[f].Contains(faction)) warFactions.Add(f);
            }
        }

        return warFactions;
    }

    public static bool IsFactionAttacking(string faction) {
        try {
            return War[faction].Count > 0;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Returns a random faction that is being attacked by caller faction. If calling faction is on par with enemies, picks randomly. If winning, focuses on the bigger one. If losing, focuses on the smaller one.
    /// </summary>
    /// <param name="tag">Faction we want to get a random attack target for</param>
    /// <returns>Selected faction to attack</returns>
    public static string GetRandomOrGoodTargetFaction(string tag) {
        if (War[tag].Count == 0) return null;

        int totalEnemyScores = 0;
        foreach (string enemy in War[tag]) {
            totalEnemyScores += FactionManager.FactionScoresManager.FactionAssetScores[enemy];
        }

        float ownScoreRelatedToEnemyScores = (float)FactionManager.FactionScoresManager.FactionAssetScores[tag] / (float)totalEnemyScores;
        if (ownScoreRelatedToEnemyScores < 0.5f) {
            foreach (string enemy in War[tag]) {
                if (Random.Range(0.0f, 1.0f) > 0.3f && (float)FactionManager.FactionScoresManager.FactionAssetScores[enemy] / (float)totalEnemyScores >= 0.6f) return enemy;
            }
        } else if (ownScoreRelatedToEnemyScores > 0.66f) {
            foreach (string enemy in War[tag]) {
                if (Random.Range(0.0f, 1.0f) > 0.3f && (float)FactionManager.FactionScoresManager.FactionAssetScores[enemy] / (float)totalEnemyScores < 0.5f) return enemy;
            }
        }

        return War[tag][Random.Range(0, War[tag].Count)];
    }

    public static string GetRandomFactionWeAreFighting(string faction) {
        List<string> factionsAttacking = new List<string>();
        foreach (string f in War.Keys) {
            if (f == faction && War[faction].Count > 0) {
                return War[faction][Random.Range(0, War[faction].Count)];
            } else if (War[f].Contains(faction)) factionsAttacking.Add(f);
        }

        if (factionsAttacking.Count > 0) return factionsAttacking[Random.Range(0, factionsAttacking.Count)];
        return null;
    }

    public static bool DoesFactionHaveTradingPartner(string tag) {
        foreach (Faction faction in FactionManager.Factions.Values) {
            if (tag != faction.factionTag && FactionManager.FactionScoresManager.IsFactionInGame(faction.factionTag) && !AreFactionsBlockading(tag, faction.factionTag)) return true;
        }

        return false;
    }
}
