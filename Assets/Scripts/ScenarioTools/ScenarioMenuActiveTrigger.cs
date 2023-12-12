using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioMenuActiveTrigger : MonoBehaviour, IScenarioTrigger {
    public float TIMER;
    public bool CURRENTLY_BUILDING;
    public bool BUILDING_MENU;
    public bool DIPLOMACY_MENU;
    public bool CARGO_SHIP_MENU;

    public bool DoNotActivate { get; set; } = false;

    private List<IScenarioEffect> Effects;
    private bool IsTriggered;

    public bool Triggered() {
        return IsTriggered;
    }

    private void Start() {
        Effects = new List<IScenarioEffect>(GetComponents<IScenarioEffect>());
        IsTriggered = !CURRENTLY_BUILDING && !BUILDING_MENU && !DIPLOMACY_MENU && !CARGO_SHIP_MENU;
        StartCoroutine(StartTimerLoop());
    }

    private IEnumerator StartTimerLoop() {
        if (!IsTriggered) {
            while (true) {
                yield return new WaitForSeconds(TIMER);
                if ((CURRENTLY_BUILDING && BuildingPlacementManager.IsBuilding) || 
                (BUILDING_MENU && FindObjectOfType<CommandCenterBuildMenu>() != null) || 
                (DIPLOMACY_MENU && FindObjectOfType<RelationShipWindowManager>() != null) ||
                (CARGO_SHIP_MENU && FindObjectOfType<CargoIndicatorUpdater>() != null)) {
                    IsTriggered = true;
                    if (!DoNotActivate) foreach (IScenarioEffect effect in Effects) effect.Effect();
                    break; 
                }
            }
        }
    }
}
