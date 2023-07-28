using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAi : MonoBehaviour, IAi {

    public float timeSpentAtPoint;
    public float shipPreference;
    public float answerHelpRequestsPercentage;
    public bool patrolsMilitaryShips;

    private IShipMovement Controls;
    private FactionAiBase Ai;
    private Hitpoints PatrolTarget;
    private Coroutine WaitTimerCoroutine;
    private bool RespondingToHelp;

    public void InitializeAi() {
        Controls = GetComponent<IShipMovement>();
        Ai = FactionManager.Factions[tag].ai;
        PatrolTarget = null;
        WaitTimerCoroutine = null;
        RespondingToHelp = false;
    }

    public void ExecuteStep() {
        if (PatrolTarget == null || (!patrolsMilitaryShips && PatrolTarget.armed)) {
            PatrolTarget = GetNewPatrolTarget();
        } else {
            Controls.SetPrimaryTargetPos(PatrolTarget.transform.position);
            if (Controls.AreWeThereYet() && WaitTimerCoroutine == null) {
                WaitTimerCoroutine = StartCoroutine(StartWaitTimer());
            }
        }
    }

    private IEnumerator StartWaitTimer() {
        yield return new WaitForSeconds(timeSpentAtPoint);
        PatrolTarget = GetNewPatrolTarget();
        WaitTimerCoroutine = null;
    }

    private Hitpoints GetNewPatrolTarget() {
        // If responding to help, return null untill we have arrived to destination. 
        if (RespondingToHelp) {
            if (Controls.AreWeThereYet()) {
                RespondingToHelp = false;
            }

            return null; 
        }

        if (RelationShipManager.IsFactionInWar(tag) && Random.value < answerHelpRequestsPercentage) {
            Vector2 helpLocation = Ai != null ? Ai.GetLocationThatCalledHelp() : Vector2.zero;
            if (helpLocation != Vector2.zero) {
                Controls.SetPrimaryTargetPos(helpLocation);
                RespondingToHelp = true;
                return null;
            }
        }

        if (Random.value < shipPreference || BuildingManager.Buildings[tag].Count == 0) {
            if (((patrolsMilitaryShips && Random.value < 0.33f) || ShipsManager.CivShips[tag].Count == 0) && ShipsManager.MilShips[tag].Count > 0) {
                return ShipsManager.GetFactionRandomMilShip(tag);
            }

            if (ShipsManager.CivShips[tag].Count > 0) {
                Hitpoints ship = ShipsManager.GetFactionRandomCivShip(tag);
                Selectable selectable = ship.GetComponent<Selectable>();
                if (selectable.selectableType != Selectable.Types.police && selectable.selectableType != Selectable.Types.raider) return ship;
            }

            return null;
        } else {
            if (BuildingManager.Buildings[tag].Count > 0) {
                return BuildingManager.GetFactionRandomBuilding(tag);
            }

            return null;
        }
    }
}
