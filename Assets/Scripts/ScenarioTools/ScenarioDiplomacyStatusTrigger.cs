using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioDiplomacyStatusTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public string FACTION1;
    public string FACTION2;
    public bool WAR;
    public bool PEACE;
    public bool BLOCKADE;
    public bool NO_BLOCKADE;

    public bool DoNotActivate { get; set; } = false;

    private List<IScenarioEffect> Effects;
    private bool IsTriggered;

    public bool Triggered() {
        return IsTriggered;
    }

    private void Start() {
        Effects = new List<IScenarioEffect>(GetComponents<IScenarioEffect>());
        IsTriggered = !WAR && !PEACE && !BLOCKADE && !NO_BLOCKADE;
        StartCoroutine(StartTimerLoop());
    }

    private IEnumerator StartTimerLoop() {
        if (!IsTriggered) {
            while (true) {
                yield return new WaitForSeconds(TIMER);
                if (!FactionManager.Factions.ContainsKey(FACTION1) || !FactionManager.Factions.ContainsKey(FACTION2)) {
                    IsTriggered = true;
                    break;
                } else if ((WAR && RelationShipManager.IsFactionAttackingFaction(FACTION1, FACTION2)) || 
                (PEACE && !RelationShipManager.IsFactionAttackingFaction(FACTION1, FACTION2)) ||
                (BLOCKADE && RelationShipManager.IsFactionBlockadingFaction(FACTION1, FACTION2)) ||
                (NO_BLOCKADE && !RelationShipManager.IsFactionBlockadingFaction(FACTION1, FACTION2))) {
                    IsTriggered = true;
                    if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                    break;
                }
            }
        }
    }
}
