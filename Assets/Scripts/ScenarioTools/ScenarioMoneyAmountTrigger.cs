using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioMoneyAmountTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public string FACTION;
    public int AMOUNT;
    public bool LESS_THAN;
    public bool GREATER_THAN;
    public bool LOOPS;

    public bool DoNotActivate { get; set; } = false;

    private List<IScenarioEffect> Effects;
    private bool IsTriggered;

    public bool Triggered() {
        bool toReturn = IsTriggered;
        if (LOOPS) IsTriggered = false;
        return toReturn;
    }

    private void Start() {
        Effects = new List<IScenarioEffect>(GetComponents<IScenarioEffect>());
        IsTriggered = false;
        StartCoroutine(StartTimerLoop());
    }

    private IEnumerator StartTimerLoop() {
        while (true) {
            yield return new WaitForSeconds(TIMER);
            if ((LESS_THAN && FactionManager.Factions[FACTION].money <= AMOUNT) || (GREATER_THAN && FactionManager.Factions[FACTION].money >= AMOUNT)) {
                IsTriggered = true;
                if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                if (!LOOPS) break;
            }
        }
    }
}
