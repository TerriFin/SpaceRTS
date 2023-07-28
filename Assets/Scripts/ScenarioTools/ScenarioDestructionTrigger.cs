using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioDestructionTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public List<GameObject> OBJECTS_TO_DESTROY;

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
            bool allDestroyed = true;
            foreach (GameObject destroyable in OBJECTS_TO_DESTROY) {
                if (destroyable != null) {
                    allDestroyed = false;
                    break;
                }
            }

            if (allDestroyed) {
                IsTriggered = true;
                if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                break;
            }
        }
    }
}
