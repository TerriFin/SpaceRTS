using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour {
    public float WIN_CHECK_TIMER;
    public float ASSET_PERCENTAGE_REQUIRED;
    public bool NEED_TO_CLEAR_ENEMY_MIL;

    void Start() {
        StartCoroutine(CheckForWin());
    }

    private IEnumerator CheckForWin() {
        while (true) {

            yield return new WaitForSeconds(WIN_CHECK_TIMER);

            string factionTag = FactionManager.HasFactionWon(ASSET_PERCENTAGE_REQUIRED);
            if (factionTag != null) {
                bool foundEnemyMilitary = !NEED_TO_CLEAR_ENEMY_MIL;
                foreach (Faction faction in FactionManager.Factions.Values) {
                    if (faction.factionTag != factionTag && faction.GetFactionMilitaryScore() != 0) {
                        foundEnemyMilitary = true;
                        break;
                    }
                }

                if (!foundEnemyMilitary) {
                    print("WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                    print("FACTION " + factionTag + " HAS WON! CONGRATZ!!!1!");
                }
            }
        }
    }
}
