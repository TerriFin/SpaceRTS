using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioBuildingAmountTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public string FACTION;
    public int AMOUNT;
    public bool LESS_THAN;
    public bool GREATER_THAN;
    public Selectable.Types TYPE;
    public bool SPECIFIC_TYPE;

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
            if (SPECIFIC_TYPE) {
                if (LESS_THAN) {
                    if (BuildingManager.BuildingAmountsByFactionAndType[FACTION][TYPE.ToString()].Count <= AMOUNT) {
                        IsTriggered = true;
                        if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                        break;
                    }
                } else if (GREATER_THAN) {
                    if (BuildingManager.BuildingAmountsByFactionAndType[FACTION][TYPE.ToString()].Count >= AMOUNT) {
                        IsTriggered = true;
                        if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                        break;
                    }
                }
            } else {
                if (LESS_THAN) {
                    if (BuildingManager.Buildings[FACTION].Count <= AMOUNT) {
                        IsTriggered = true;
                        if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                        break;
                    }
                } else if (GREATER_THAN) {
                    if (BuildingManager.Buildings[FACTION].Count >= AMOUNT) {
                        IsTriggered = true;
                        if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                        break;
                    }
                }
            }
        }
    }
}
