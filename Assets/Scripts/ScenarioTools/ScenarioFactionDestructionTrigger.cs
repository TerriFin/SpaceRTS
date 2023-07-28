using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioFactionDestructionTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public string FACTION;

    public bool DoNotActivate { get; set; } = false;

    private List<IScenarioEffect> Effects;
    private bool IsTriggered;

    public bool Triggered() {
        return IsTriggered;
    }

    private void Start() {
        Effects = new List<IScenarioEffect>(GetComponents<IScenarioEffect>());
        IsTriggered = false;
        StartCoroutine(StartTimerLoop());
    }

    private IEnumerator StartTimerLoop() {
        while (true) {
            yield return new WaitForSeconds(TIMER);
            if (!FactionManager.Factions.ContainsKey(FACTION)) {
                IsTriggered = true;
                if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                break;
            }
        }
    }
}
