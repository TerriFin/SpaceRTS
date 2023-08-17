using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FactionScoresManager : MonoBehaviour {
    public float CACHE_REFRESH_TIME;
    public float FACTION_LOSS_ASSET_SCORE_PERCENTAGE;
    public float FACTION_LOSS_DUEL_ASSET_SCORE_PERCENTAGE;
    public GameObject GAME_END_SCREEN;
    public TMP_Text FACTION_WON_TEXT;

    public Dictionary<string, int> FactionAssetScores { get; private set; }
    public Dictionary<string, int> FactionMilitaryScores { get; private set; }
    public Dictionary<string, int> FactionCivilianScores { get; private set; }

    private bool FirstLoop;

    private void Start() {
        FactionAssetScores = new Dictionary<string, int>();
        FactionMilitaryScores = new Dictionary<string, int>();
        FactionCivilianScores = new Dictionary<string, int>();

        FirstLoop = true;

        StartCoroutine(UpdateFactionScores());
    }

    private IEnumerator UpdateFactionScores() {
        while (true) {
            foreach (Faction faction in FactionManager.Factions.Values) {
                FactionAssetScores.Remove(faction.factionTag);
                FactionMilitaryScores.Remove(faction.factionTag);
                FactionCivilianScores.Remove(faction.factionTag);

                FactionAssetScores.Add(faction.factionTag, faction.GetFactionAssetScore());
                FactionMilitaryScores.Add(faction.factionTag, faction.GetFactionMilitaryScore());
                FactionCivilianScores.Add(faction.factionTag, faction.GetFactionCivilianScore());
            }

            /*
            print("Pirate ASSET SCORES: " + FactionAssetScores["Pirate"]);
            print("Federation ASSET SCORES: " + FactionAssetScores["Federation"]);
            print("Empire ASSET SCORES: " + FactionAssetScores["Empire"]);
            print("Pirate Military SCORES: " + FactionMilitaryScores["Pirate"]);
            print("Federation Military SCORES: " + FactionMilitaryScores["Federation"]);
            print("Empire Military SCORES: " + FactionMilitaryScores["Empire"]);
            /*
            print("Pirate Civilian SCORES: " + FactionCivilianScores["Pirate"]);
            print("Federation Civilian SCORES: " + FactionCivilianScores["Federation"]);
            print("Empire Civilian SCORES: " + FactionCivilianScores["Empire"]);
            */

            yield return new WaitForSeconds(CACHE_REFRESH_TIME);
            // Turn faction AI off if they have lost
            if (!FirstLoop) {
                Faction[] factions = new Faction[FactionManager.Factions.Count];
                FactionManager.Factions.Values.CopyTo(factions, 0);
                foreach (Faction faction in factions) {
                    if (!IsFactionInGame(faction.factionTag)) {
                        faction.ai.ON = false;
                        BuildingManager.RemoveFactionBuildings(faction.factionTag);
                        ShipsManager.RemoveFactionShips(faction.factionTag);
                        PlanetManager.ResetFactionPlanetsToNeutral(faction.factionTag);
                        RelationShipManager.RemoveFaction(faction.factionTag);
                        FactionManager.RemoveFactionFromGame(faction.factionTag);
                        FactionOpinionManager.RemoveFaction(faction.factionTag);
                        GlobalMessageManager.GlobalMessage(faction.factionTag + " HAS BEEN DEFEATED");
                        continue;
                    }
                }
            }

            if (FactionManager.Factions.Count == 1) {
                PauseMenu.CAN_PAUSE = false;
                GAME_END_SCREEN.SetActive(true);
                GAME_END_SCREEN.GetComponent<RectTransform>().SetAsLastSibling();
                FACTION_WON_TEXT.text = new List<Faction>(FactionManager.Factions.Values)[0].factionTag + "\n HAS WON";
                MusicManager.SetVolume("sfxVolume", 0.0f);
                MusicManager.SetVolume("musicVolume", 0.75f);
                Time.timeScale = 0f;
            }

            FirstLoop = false;
        }
    }

    public void ContinueAfterGameEnd() {
        PauseMenu.CAN_PAUSE = true;
        SceneManager.LoadScene(0);
    }

    public int TotalFactionsAssetScore() {
        int totalFactionScore = 0;

        foreach (int assetScore in FactionAssetScores.Values) {
            totalFactionScore += assetScore;
        }

        return totalFactionScore;
    }

    public int ActiveFactionMedianAssetScore() {
        int totalFactionScore = 0;
        int totalFactionsInGame = 0;

        foreach (Faction faction in FactionManager.Factions.Values) {
            int factionScore = FactionAssetScores[faction.factionTag];
            totalFactionScore += factionScore;

            // Command center gives 20, so it is the benchmark of being in the game.
            if (factionScore >= 20) {
                totalFactionsInGame++;
            }
        }

        if (totalFactionsInGame == 0) return totalFactionScore / FactionManager.Factions.Count;
        return totalFactionScore / totalFactionsInGame;
    }

    public int GetActiveFactionAssetScoreComparedToOthers(string factionTag) {
        return FactionAssetScores[factionTag] - ActiveFactionMedianAssetScore();
    }

    public float GetFactionAssetScoreAdvantageComparedToOther(string faction1, string faction2) {
        return FactionAssetScores[faction1] - FactionAssetScores[faction2];
    }

    public float GetFactionAssetScoreShare(string factionTag) {
        if (FactionAssetScores[factionTag] == 0) return 0;
        return (float) FactionAssetScores[factionTag] / (float) TotalFactionsAssetScore();
    }

    public int TotalFactionsMilitaryScore() {
        int totalMilitaryScore = 0;

        foreach (int militaryScore in FactionMilitaryScores.Values) {
            totalMilitaryScore += militaryScore;
        }

        return totalMilitaryScore;
    }

    public int ActiveFactionMedianMilitaryScore() {
        int totalMilitaryScore = 0;
        int totalFactionsInGame = 0;

        foreach (Faction faction in FactionManager.Factions.Values) {
            int militaryScore = FactionMilitaryScores[faction.factionTag];
            totalMilitaryScore += militaryScore;

            if (militaryScore >= 8) {
                totalFactionsInGame++;
            }
        }

        if (totalFactionsInGame == 0) return totalMilitaryScore / FactionManager.Factions.Count;
        return totalMilitaryScore / totalFactionsInGame;
    }

    public int GetActiveFactionMilitaryScoreComparedToOthers(string factionTag) {
        return FactionMilitaryScores[factionTag] - ActiveFactionMedianMilitaryScore();
    }

    public float GetFactionMilitaryScoreAdvantageComparedToOther(string faction1, string faction2) {
        return FactionMilitaryScores[faction1] - FactionMilitaryScores[faction2];
    }

    public float GetFactionMilitaryScoreShare(string factionTag) {
        if (FactionMilitaryScores[factionTag] == 0) return 0;
        return (float) FactionMilitaryScores[factionTag] / (float) TotalFactionsMilitaryScore();
    }

    public int TotalFactionsCivilianScore() {
        int totalCivilianScore = 0;

        foreach (int civilianScore in FactionCivilianScores.Values) {
            totalCivilianScore += civilianScore;
        }

        return totalCivilianScore;
    }

    public int ActiveFactionMedianCivilianScore() {
        int totalCivilianScore = 0;
        int totalFactionsInGame = 0;

        foreach (Faction faction in FactionManager.Factions.Values) {
            int civilianScore = FactionCivilianScores[faction.factionTag];
            totalCivilianScore += civilianScore;

            if (civilianScore >= 12) {
                totalFactionsInGame++;
            }
        }

        if (totalFactionsInGame == 0) return totalCivilianScore / FactionManager.Factions.Count;
        return totalCivilianScore / totalFactionsInGame;
    }

    public int GetActiveFactionCivilianScoreComparedToOthers(string factionTag) {
        return FactionCivilianScores[factionTag] - ActiveFactionMedianCivilianScore();
    }

    public float GetFactionCivilianScoreAdvantageComparedToOther(string faction1, string faction2) {
        return FactionCivilianScores[faction1] - FactionCivilianScores[faction2];
    }

    public float GetFactionCivilianScoreShare(string factionTag) {
        if (FactionCivilianScores[factionTag] == 0) return 0;
        return (float) FactionCivilianScores[factionTag] / (float) TotalFactionsCivilianScore();
    }

    public bool IsFactionInGame(string faction) {
        if (FACTION_LOSS_ASSET_SCORE_PERCENTAGE == 0 && FACTION_LOSS_DUEL_ASSET_SCORE_PERCENTAGE == 0) return true;
        if (FactionManager.PlayerFaction != null && FactionManager.PlayerFaction.factionTag == faction) return (float)FactionAssetScores[faction] / (float)TotalFactionsAssetScore() > FACTION_LOSS_ASSET_SCORE_PERCENTAGE / 2;
        if (FactionManager.Factions.Count == 2) return (float)FactionAssetScores[faction] / (float)TotalFactionsAssetScore() > FACTION_LOSS_DUEL_ASSET_SCORE_PERCENTAGE;
        return BuildingManager.BuildingAmountsByFactionAndType[faction][Selectable.Types.commandCenter.ToString()].Count > 0 || (float)FactionAssetScores[faction] / (float)TotalFactionsAssetScore() > FACTION_LOSS_ASSET_SCORE_PERCENTAGE;
    }

    public int GetActiveFactionCount() {
        int toReturn = FactionManager.Factions.Count;

        foreach (Faction faction in FactionManager.Factions.Values) {
            if (!IsFactionInGame(faction.factionTag)) toReturn--;
        }

        return toReturn;
    }
}
