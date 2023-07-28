using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarManagerBlock : FactionBlockWithSkips {

    public float TOTAL_WAR_TIME;
    public float TOTAL_WAR_SCORE_PERCENTAGE;
    public float TOTAL_PEACE_SCORE_PERCENTAGE;
    public float GANG_UP_ON_WINNER_PERCENTAGE;
    public float ATTACK_ADVANTAGE_THRESHOLD_PERCENTAGE;
    public float PEACE_DISADVANTAGE_THRESHOLD_PERCENTAGE;
    public float OPINION_MODIFIER;
    public bool RAIDER;

    private bool RaiderStartedRaiding;
    public bool TotalWarTriggered { get; private set; }

    private FactionScoresManager FactionScores;

    private void Start() {
        RaiderStartedRaiding = false;
        TotalWarTriggered = false;

        FactionScores = FactionManager.FactionScoresManager;
    }

    public override void Initialize() {
        if (TOTAL_WAR_TIME != 0) {
            StartCoroutine(StartTotalWarTimer());
        }
    }

    public override void Block() {
        // If total war triggered, just fight
        if (TotalWarTriggered) return;

        // If only one other player, fight
        if (FactionManager.Factions.Count == 2) {
            foreach (Faction currentFaction in FactionManager.Factions.Values) {
                if (!CompareTag(currentFaction.factionTag)) {
                    RelationShipManager.StartWar(tag, currentFaction.factionTag);
                    print(tag + " STARTED FIGHTING, AS THERE IS ONLY ONE ENEMY");
                    return;
                }
            }
        }

        // Raiders want to raid, this starts it
        if (RAIDER && !RaiderStartedRaiding && !RelationShipManager.IsFactionInWar(tag) && (BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.raiderStation.ToString()].Count >= 3 || BuildingManager.BuildingAmountsByFactionAndType[tag][Selectable.Types.cruiserStation.ToString()].Count >= 1)) {
            foreach (string currentFaction in FactionManager.GetFactionsInRandomOrder()) {
                if (!CompareTag(currentFaction)) {
                    RelationShipManager.StartWar(tag, currentFaction);
                    RaiderStartedRaiding = true;
                    print(tag + " STARTED RAIDING");
                    return;
                }
            }
        }

        // If we are winning or losing so hard, we just want war/peace
        float ownMilitaryScoreShare = FactionScores.GetFactionMilitaryScoreShare(tag);
        if (ownMilitaryScoreShare >= TOTAL_WAR_SCORE_PERCENTAGE) {
            StartTotalWar();
            print(tag + " STARTED TOTAL WAR BECAUSE THEY ARE AHEAD");
            return;
        } else if (ownMilitaryScoreShare <= TOTAL_PEACE_SCORE_PERCENTAGE) {
            print(tag + " STARTED TOTAL PEACE BECAUSE THEY ARE LOSING");
            StartTotalPeace();
            return;
        }

        // If someone is winning too hard, we want to gang up on them
        foreach (Faction faction in FactionManager.Factions.Values) {
            if (!CompareTag(faction.factionTag) && FactionScores.GetFactionAssetScoreShare(faction.factionTag) >= GANG_UP_ON_WINNER_PERCENTAGE && FactionOpinionManager.CheckFactionOpinionPercentage(tag, faction.factionTag) < 0.8f) {
                StartTotalPeace();
                RelationShipManager.StartWar(tag, faction.factionTag);
                print(tag + " STARTED GANGING UP ON " + faction.factionTag + " BECAUSE THEY ARE WINNING");
                return;
            }
        }

        // All other reasons
        HashSet<string> currentWars = RelationShipManager.HowManyWarsIsFactionIn(tag);
        if (currentWars.Count == 0) {
            foreach (string currentFaction in FactionManager.GetFactionsInRandomOrder()) {
                /*
                if (!CompareTag(currentFaction)) print(tag + " MILITARY SCORE: " + FactionScores.FactionMilitaryScores[tag] + ", " + currentFaction + " MILITARY SCORE: " + FactionScores.FactionMilitaryScores[currentFaction]);
                if (!CompareTag(currentFaction)) print(tag + " WEIGHED SCORE: " + (float) FactionScores.FactionMilitaryScores[tag] / (float) FactionScores.FactionMilitaryScores[currentFaction] + " >= " + ATTACK_ADVANTAGE_THRESHOLD_PERCENTAGE + OPINION_MODIFIER * FactionOpinionManager.CheckFactionOpinionPercentage(tag, currentFaction));
                */
                if (!CompareTag(currentFaction) && 
                    FactionScores.FactionMilitaryScores[tag] > 20 && 
                    (FactionScores.FactionMilitaryScores[currentFaction] == 0 || (float) FactionScores.FactionMilitaryScores[tag] / (float) FactionScores.FactionMilitaryScores[currentFaction] >= ATTACK_ADVANTAGE_THRESHOLD_PERCENTAGE + OPINION_MODIFIER * FactionOpinionManager.CheckFactionOpinionPercentage(tag, currentFaction))) {
                    RelationShipManager.StartWar(tag, currentFaction);
                    print(tag + " STARTED ATTACKING " + currentFaction + " BECAUSE THEY ARE STRONGER");
                    return;
                }
            }
        } else if (currentWars.Count == 1) {
            foreach (string currentFaction in FactionManager.GetFactionsInRandomOrder()) {
                if (!CompareTag(currentFaction) && 
                    RelationShipManager.IsFactionAttackingFaction(tag, currentFaction) && 
                    FactionScores.FactionMilitaryScores[currentFaction] != 0 && FactionScores.FactionMilitaryScores[tag] / FactionScores.FactionMilitaryScores[currentFaction] <= PEACE_DISADVANTAGE_THRESHOLD_PERCENTAGE - OPINION_MODIFIER * FactionOpinionManager.CheckFactionOpinionPercentage(tag, currentFaction)) {
                    RelationShipManager.EndWar(tag, currentFaction);
                    print(tag + " STOPPED ATTACKING " + currentFaction + " BECAUSE THEY ARE WEAKER");
                    return;
                }
            }
        } else {
            foreach (string currentFaction in FactionManager.GetFactionsInRandomOrder()) {
                if (!CompareTag(currentFaction) && RelationShipManager.IsFactionAttackingFaction(tag, currentFaction)) {
                    RelationShipManager.EndWar(tag, currentFaction);
                    print(tag + " STOPPED ATTACKING " + currentFaction + " BECAUSE THEY ARE IN MULTIPLE WARS");
                    return;
                }
            }
        }
    }

    private IEnumerator StartTotalWarTimer() {
        yield return new WaitForSeconds(TOTAL_WAR_TIME);
        StartTotalWar();
        TotalWarTriggered = true;
    }

    private void StartTotalWar() {
        foreach (Faction faction in FactionManager.Factions.Values) {
            RelationShipManager.StartWar(tag, faction.factionTag);
        }
    }

    private void StartTotalPeace() {
        foreach (Faction faction in FactionManager.Factions.Values) {
            RelationShipManager.EndWar(tag, faction.factionTag);
        }
    }
}
