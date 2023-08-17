using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FactionVictory : MonoBehaviour, IScenarioEffect {
    public string WINNING_FACTION;
    public string WON_STAGE;
    public GameObject GAME_END_SCREEN;
    public TMP_Text FACTION_WON_TEXT;
    public void Effect() {
        PauseMenu.CAN_PAUSE = false;
        GAME_END_SCREEN.SetActive(true);
        GAME_END_SCREEN.GetComponent<RectTransform>().SetAsLastSibling();
        Faction winningFaction = FactionManager.Factions[WINNING_FACTION];
        MapGeneratorManager mapManager = FindObjectOfType<MapGeneratorManager>();
        FACTION_WON_TEXT.text = winningFaction.factionTag + "\n HAS WON";
        if (FactionManager.PlayerFaction != null && winningFaction.factionTag == FactionManager.PlayerFaction.factionTag) PlayerPrefs.SetInt(WON_STAGE, 1);
        MusicManager.SetVolume("sfxVolume", 0.0f);
        MusicManager.SetVolume("musicVolume", 0.75f);
        Time.timeScale = 0f;
    }
}
