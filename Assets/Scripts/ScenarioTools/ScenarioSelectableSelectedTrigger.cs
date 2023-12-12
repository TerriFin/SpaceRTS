using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioSelectableSelectedTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public Selectable TARGET;
    public Selectable.Types TYPE;

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
            if (TARGET != null && SelectionManager.selected.Contains(TARGET)) {
                IsTriggered = true;
                if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                break;
            } else if (TARGET == null) {
                bool found = false;
                foreach (Selectable selected in SelectionManager.selected) {
                    if (TYPE == selected.selectableType) {
                        found = true;
                        break;
                    }
                }

                if (found) {
                    IsTriggered = true;
                    if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                    break;
                }
            } 
        }
    }
}
