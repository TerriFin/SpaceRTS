using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioTimerTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public bool LOOPS;

    public bool DoNotActivate { get; set; } = false;

    private List<IScenarioEffect> Effects;
    private bool IsTriggered;

    public bool Triggered() {
        bool toReturn = IsTriggered;
        IsTriggered = false;
        return toReturn;
    }

    private void Start() {
        Effects = new List<IScenarioEffect>(GetComponents<IScenarioEffect>());
        IsTriggered = false;
        StartCoroutine(StartTimerLoop());
    }

    private IEnumerator StartTimerLoop() {
        if (LOOPS) {
            while (true) {
                yield return new WaitForSeconds(TIMER);
                IsTriggered = true;
                if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
            }
        } else {
            yield return new WaitForSeconds(TIMER);
            IsTriggered = true;
            if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
        }
    }
}
