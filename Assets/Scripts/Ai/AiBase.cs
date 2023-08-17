using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBase : MonoBehaviour {

    public bool aiActive;
    public bool combatModuleActive;
    public int combatModuleActivationNeededCivilians;
    public float combatModuleActivationTime;
    public float updateTime;

    private IAi Ai;
    public CombatModule CombatModule { get; private set; }
    public Production ProductionComponent { get; set; }
    private Sensors Sensors;
    private IShipMovement Controls;
    private Coroutine CurrentCombatModuleActivationCoroutine;

    private void Start() {
        Ai = GetComponent<IAi>();
        CombatModule = GetComponent<CombatModule>();
        Sensors = GetComponent<Sensors>();
        Controls = GetComponent<IShipMovement>();
        CurrentCombatModuleActivationCoroutine = null;

        if (Ai != null) {
            Ai.InitializeAi();
        }

        StartCoroutine(AiLoop());
    }

    private IEnumerator AiLoop() {
        while (true) {
            yield return new WaitForSeconds(updateTime);
            // If production component is set, follow it and attack if possible.
            if (ProductionComponent != null) {
                if (Sensors.Enemies.Count > 0 && CurrentCombatModuleActivationCoroutine == null && (Controls.AreWeThereYet() || Sensors.ArmedEnemiesMilitary.Count > 0 || Sensors.Enemies.Count > combatModuleActivationNeededCivilians)) {
                    CurrentCombatModuleActivationCoroutine = StartCoroutine(CombatModuleActivationTimer());
                } else {
                    CombatModule.StopCombatModule();
                    Controls.SetPrimaryTargetPos(ProductionComponent.transform.position);
                }
            } else if (aiActive && combatModuleActive && CombatModule != null) {
                if (Sensors.Enemies.Count > 0 && CurrentCombatModuleActivationCoroutine == null && (Controls.AreWeThereYet() || Sensors.ArmedEnemiesMilitary.Count > 0 || Sensors.Enemies.Count > combatModuleActivationNeededCivilians)) {
                    CurrentCombatModuleActivationCoroutine = StartCoroutine(CombatModuleActivationTimer());
                } else {
                    CombatModule.StopCombatModule();
                    Ai.ExecuteStep();
                }
            } else if (combatModuleActive && CombatModule != null) {
                if (Sensors.Enemies.Count > 0 && CurrentCombatModuleActivationCoroutine == null && (Controls.AreWeThereYet() || Sensors.ArmedEnemiesMilitary.Count > 0 || Sensors.Enemies.Count > combatModuleActivationNeededCivilians)) {
                    CurrentCombatModuleActivationCoroutine = StartCoroutine(CombatModuleActivationTimer());
                } else {
                    CombatModule.StopCombatModule();
                }
            } else if (aiActive) {
                Ai.ExecuteStep();
            }
        }
    }

    private IEnumerator CombatModuleActivationTimer() {
        yield return new WaitForSeconds(updateTime);
        if (combatModuleActive && Sensors.Enemies.Count > 0) {
            CombatModule.StartCombatModule();
        }

        CurrentCombatModuleActivationCoroutine = null;
    }

    public void SetAiActive(bool toggle) {
        if (toggle == false) {
            if (Controls == null) {
                Controls = GetComponent<IShipMovement>();
            }

            Controls.SetPrimaryTargetPos(transform.position);
        }

        aiActive = toggle;
    }

    public void SetCombatModuleActive(bool toggle) {
        if (toggle == false) {
            CombatModule.StopCombatModule();
        }

        combatModuleActive = toggle;
    }
}
