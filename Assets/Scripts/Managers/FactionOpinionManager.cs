using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionOpinionManager : MonoBehaviour {
    public static float MAX_VALUE = 100.0f;
    public static float MIN_VALUE = -200.0f;

    public float TIMER;
    public float MAX_FACTION_SIZE_PENALTY;
    public float OPINION_DECAY_PERCENTAGE;

    public static Dictionary<string, Dictionary<string, float>> FactionOpinions;

    private FactionScoresManager factionScores;

    private void Start() {
        factionScores = FindObjectOfType<FactionScoresManager>();
        StartCoroutine(UpdateFactionOpinions());
    }

    private IEnumerator UpdateFactionOpinions() {
        while (true) {
            yield return new WaitForSeconds(TIMER);

            AddFactionSizePenalty();
            AddConflictedAsteroidFieldsOpinionPenalties();
            AddConflictedPlanetsOpinionPenalties();
            AddConflictPartnerOpinionBoosts();

            foreach (string faction1 in new List<string>(FactionOpinions.Keys)) {
                foreach (string faction2 in new List<string>(FactionOpinions[faction1].Keys)) {
                    if (FactionOpinions[faction1][faction2] > 0) {
                        if (FactionOpinions[faction1][faction2] > MAX_VALUE) FactionOpinions[faction1][faction2] = MAX_VALUE;
                        FactionOpinions[faction1][faction2] -= FactionOpinions[faction1][faction2] * OPINION_DECAY_PERCENTAGE;
                    } else if (FactionOpinions[faction1][faction2] < 0) {
                        if (FactionOpinions[faction1][faction2] < MIN_VALUE) FactionOpinions[faction1][faction2] = MIN_VALUE;
                        if (!RelationShipManager.AreFactionsInWar(faction1, faction2)) FactionOpinions[faction1][faction2] -= FactionOpinions[faction1][faction2] * OPINION_DECAY_PERCENTAGE;
                    }
                }
            }
            foreach (string faction1 in new List<string>(FactionOpinions.Keys)) {
                foreach (string faction2 in new List<string>(FactionOpinions.Keys)) {
                    if (faction1 != faction2) {
                        print(faction1 + " OPINION OF " + faction2 + ": " + FactionOpinions[faction1][faction2]);
                    }
                }
            }
        }
    }

    private void AddFactionSizePenalty() {
        foreach (string faction1 in new List<string>(FactionOpinions.Keys)) {
            foreach (string faction2 in new List<string>(FactionOpinions[faction1].Keys)) {
                if (faction1 != faction2) {
                    FactionOpinions[faction1][faction2] -= MAX_FACTION_SIZE_PENALTY * factionScores.GetFactionAssetScoreShare(faction2);
                }
            }
        }
    }

    private void AddConflictedAsteroidFieldsOpinionPenalties() {
        foreach (AsteroidField field in AsteroidFieldManager.AsteroidFields) {
            HashSet<string> factionsInField = field.FactionsPresentInField();
            foreach (string faction1 in new List<string>(FactionOpinions.Keys)) {
                foreach (string faction2 in new List<string>(FactionOpinions[faction1].Keys)) {
                    if (factionsInField.Contains(faction1) && factionsInField.Contains(faction2)) FactionOpinions[faction1][faction2] -= 8;
                    // if (factionsInField.Contains(faction1) && factionsInField.Contains(faction2)) print(faction1 + " HATES " + faction2 + " MINE AT " + field.transform.position);
                }
            }
        }
    }

    private void AddConflictedPlanetsOpinionPenalties() {
        foreach (string faction in new List<string>(FactionOpinions.Keys)) {
            if (BuildingManager.BuildingAmountsByFactionAndType[faction][Selectable.Types.commandCenter.ToString()].Count > 0) {
                List<PlanetCaptureLogic> ownPlanets = PlanetManager.GetPlanetsSortedToLocation(BuildingManager.GetFactionCenterPoint(faction));
                int ownPlanetsCount = (ownPlanets.Count / FactionManager.Factions.Count) - 1;
                if (ownPlanetsCount == 0) ownPlanetsCount = 1;
                for (int i = 0; i < ownPlanetsCount; i++) {
                    if (!ownPlanets[i].CompareTag("Untagged") && !ownPlanets[i].CompareTag(faction)) {
                        FactionOpinions[faction][ownPlanets[i].tag] -= ownPlanets[i].MoneyGenerator.factionMoneyValue;
                        // print(faction + " LOATHES " + ownPlanets[i].tag + " CONTROL OF PLANET AT " + ownPlanets[i].transform.position);
                    }
                }
            }
        }
    }

    private void AddConflictPartnerOpinionBoosts() {
        foreach (string faction1 in new List<string>(FactionOpinions.Keys)) {
            foreach (string faction2 in new List<string>(FactionOpinions.Keys)) {
                if (RelationShipManager.AreFactionsInWar(faction1, faction2)) {
                    foreach (string faction3 in new List<string>(FactionOpinions.Keys)) {
                        if (faction3 != faction1 && faction3 != faction2 && !RelationShipManager.AreFactionsInWar(faction3, faction1) && RelationShipManager.AreFactionsInWar(faction3, faction2)) FactionOpinions[faction1][faction3] += 15;
                    }
                }
            }
        }
    }

    public static void Reset() {
        FactionOpinions = new Dictionary<string, Dictionary<string, float>>();
        foreach (string faction in FactionManager.Factions.Keys) {
            FactionOpinions[faction] = new Dictionary<string, float>();
            foreach (string otherFaction in FactionManager.Factions.Keys) {
                if (faction != otherFaction) FactionOpinions[faction][otherFaction] = 0;
            }
        }
    }

    public static void ModifyFactionOpinion(string faction1, string faction2, float amount) {
        FactionOpinions[faction1][faction2] += amount;
    }

    public static float CheckFactionOpinionPercentage(string faction1, string faction2) {
        if (FactionOpinions[faction1][faction2] < 0) {
            return FactionOpinions[faction1][faction2] / MIN_VALUE;
        } else if (FactionOpinions[faction1][faction2] > 0) {
            return FactionOpinions[faction1][faction2] / MAX_VALUE;
        }

        return 0.0f;
    }

    public static void RemoveFaction(string faction) {
        foreach (string faction1 in new List<string>(FactionOpinions.Keys)) {
            if (faction1 != faction) {
                FactionOpinions[faction1].Remove(faction);
            }
        }

        FactionOpinions.Remove(faction);
    }
}
