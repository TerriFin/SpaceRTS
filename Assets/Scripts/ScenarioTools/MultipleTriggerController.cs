using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTriggerController : MonoBehaviour {
    public float TIMER;

    private List<IScenarioTrigger> Triggers;
    private List<IScenarioEffect> Effects;
    private bool HasLoopingTimer = false;

    private void Start() {
        Triggers = new List<IScenarioTrigger>(GetComponents<IScenarioTrigger>());
        Triggers.ForEach((IScenarioTrigger trigger) => {
            trigger.DoNotActivate = true;
            if (trigger is ScenarioTimerTrigger) HasLoopingTimer = true;
        });
        Effects = new List<IScenarioEffect>(GetComponents<IScenarioEffect>());

        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer() {
        while (true) {
            yield return new WaitForSeconds(TIMER);
            bool allTriggered = true;
            foreach (IScenarioTrigger trigger in Triggers) {
                if (!trigger.Triggered()) {
                    allTriggered = false;
                    break;
                }
            }

            if (allTriggered) {
                Effects.ForEach((IScenarioEffect effect) => effect.Effect());
                if (!HasLoopingTimer) break;
            }
        }
    }
}
