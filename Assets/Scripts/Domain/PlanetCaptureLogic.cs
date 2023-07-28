using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCaptureLogic : MonoBehaviour {

    public float refreshTime;
    public float radius;
    public float ALERT_RESET_TIMER;
    public float PLANET_DISTANCE_REQUIREMENT;

    public MoneyGenerator MoneyGenerator { get; private set; }

    private bool CanAlert;
    private List<FactionLight> lights;
    private MinimapIcon minimapIcon;

    private void Start() {
        PlanetManager.Planets.Add(this);
        PlanetManager.FactionPlanets[tag].Add(this);
        SetMoneyGenerator();
        CanAlert = true;
        lights = new List<FactionLight>(GetComponentsInChildren<FactionLight>());
        minimapIcon = GetComponentInChildren<MinimapIcon>();
        ResetLights();

        StartCoroutine(CaptureLogic());
    }

    public void SetMoneyGenerator() {
        if (MoneyGenerator == null) MoneyGenerator = GetComponent<MoneyGenerator>();
    }

    public void ResetPlanetOwnership() {
        tag = "Untagged";
        foreach (FactionLight light in lights) {
            light.ChangeColor(light.defaultColor);
        }

        minimapIcon.ResetColorToOriginal();
    }

    private IEnumerator CaptureLogic() {
        while (true) {
            yield return new WaitForSeconds(refreshTime);
            if (tag == "Untagged" || RelationShipManager.IsFactionInWar(tag)) {
                bool canCapture = true;
                Dictionary<string, int> armedEnemyShips = new Dictionary<string, int>();
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

                foreach (Collider2D collider in colliders) {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Ship") || collider.gameObject.layer == LayerMask.NameToLayer("Building")) {
                        if (collider.GetComponent<Hitpoints>().armed) {
                            if (collider.tag != tag) {
                                if (tag == "Untagged" || RelationShipManager.AreFactionsInWar(tag, collider.tag)) {
                                    if (!armedEnemyShips.ContainsKey(collider.tag)) armedEnemyShips.Add(collider.tag, 0);
                                    armedEnemyShips[collider.tag]++;
                                }
                            } else {
                                canCapture = false;
                                break;
                            }
                        }
                    }
                }

                if (canCapture) {
                    int largestEnemyPresence = 0;
                    string capturerFactionName = tag;

                    foreach (string faction in armedEnemyShips.Keys) {
                        if (armedEnemyShips[faction] > largestEnemyPresence) {
                            largestEnemyPresence = armedEnemyShips[faction];
                            capturerFactionName = faction;
                        }
                    }

                    if (!CompareTag("Untagged") && FactionManager.Factions.ContainsKey(tag)) {
                        FactionManager.Factions[tag].maxMoney -= MoneyGenerator.FACTION_MONEY_STORAGE;
                        FactionManager.Factions[tag].ReduceMoneyToMax();
                    }
                    if (capturerFactionName != "Untagged") FactionManager.Factions[capturerFactionName].maxMoney += MoneyGenerator.FACTION_MONEY_STORAGE;

                    PlanetManager.FactionPlanets[tag].Remove(this);
                    gameObject.tag = capturerFactionName;
                    PlanetManager.FactionPlanets[tag].Add(this);
                    ResetLights();
                } else if (armedEnemyShips.Keys.Count > 0) {
                    int enemyShips = 0;
                    foreach (string faction in armedEnemyShips.Keys) {
                        enemyShips += armedEnemyShips[faction];
                    }

                    if (CanAlert == true && enemyShips > 0) {
                        FactionManager.Factions[tag].ai.CallHelp(transform.position, enemyShips);
                        StartCoroutine(StartAlertWaitTimer());
                    }
                }
            }
        }
    }

    private IEnumerator StartAlertWaitTimer() {
        CanAlert = false;
        yield return new WaitForSeconds(ALERT_RESET_TIMER);
        CanAlert = true;
    }

    private void ResetLights() {
        if (tag != "Untagged") {
            foreach (FactionLight light in lights) {
                light.ChangeColor(FactionManager.Factions[tag].factionColor);
            }

            minimapIcon.ChangeIconColor(FactionManager.Factions[tag].factionColor);
        }
    }
}
