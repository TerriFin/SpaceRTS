using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAlert : MonoBehaviour {

    public float UPDATE_TIMER;
    public float ALERT_RESET_TIMER;
    public int PLAYER_WAR_SHIPS_WAR_LIMIT;
    public float PLAYER_WAR_SHIPS_WAR_CHECK_RADIUS;
    public bool IMPORTANT;

    private Sensors Sensors;
    private bool CanAlert;

    private void Start() {
        Sensors = GetComponent<Sensors>();
        CanAlert = true;
        StartCoroutine(StartAlertTimer());
    }

    private IEnumerator StartAlertTimer() {
        while (true) {
            yield return new WaitForSeconds(UPDATE_TIMER);
            if (PLAYER_WAR_SHIPS_WAR_LIMIT > 0 && FactionManager.PlayerFaction != null && !CompareTag(FactionManager.PlayerFaction.factionTag) && !RelationShipManager.AreFactionsInWar(tag, FactionManager.PlayerFaction.factionTag)) {
                int foundPlayerMilitaryShips = 0;
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, PLAYER_WAR_SHIPS_WAR_CHECK_RADIUS);
                foreach (Collider2D collider in colliders) {
                    if (collider.CompareTag(FactionManager.PlayerFaction.factionTag) && collider.gameObject.layer == LayerMask.NameToLayer("Ship") && collider.GetComponent<MilitaryShipClickReact>() != null) {
                        foundPlayerMilitaryShips++;
                        if (foundPlayerMilitaryShips >= PLAYER_WAR_SHIPS_WAR_LIMIT) {
                            RelationShipManager.StartWar(tag, FactionManager.PlayerFaction.factionTag);
                            print(tag + " started war against player because they brought too many military ships to their CC!");
                        }
                    }
                }
            }

            if (FactionManager.Factions[tag].ai != null && CanAlert == true && Sensors.ArmedEnemies.Count >= Sensors.ArmedAllies.Count) {
                FactionManager.Factions[tag].ai.CallHelp(transform.position, Sensors.ArmedEnemies.Count, IMPORTANT);
                StartCoroutine(StartAlertWaitTimer());
            }
        }
    }

    private IEnumerator StartAlertWaitTimer() {
        CanAlert = false;
        yield return new WaitForSeconds(ALERT_RESET_TIMER);
        CanAlert = true;
    }
}
