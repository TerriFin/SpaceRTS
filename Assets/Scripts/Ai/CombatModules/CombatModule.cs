using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatModule : MonoBehaviour {

    public bool active;
    public float preferredCombatDistance;
    public int howManyLoopsIgnoreNewTargetsAfterFindingOne;
    public float RETREAT_HP_PERCENTAGE;
    public float RETREAT_ENEMY_ADVANTAGE_PERCENTAGE;
    public float updateTime;

    public IShipMovement Controls { get; private set; }
    public Sensors Sensors { get; private set; }
    public Turret AttachedTurret { get; private set; }

    private int HowManyIgnoreLoopsLeft;
    private Coroutine CurrentCoroutine;
    private bool Started = false;

    private IEnumerator UpdateCombatModule() {
        while (true) {
            if (!Started) {
                Controls = GetComponent<IShipMovement>();
                Sensors = GetComponent<Sensors>();
                AttachedTurret = GetComponentInChildren<Turret>();
                HowManyIgnoreLoopsLeft = 0;
                Started = true;
            }

            if (Sensors.Enemies.Count > 0 && HowManyIgnoreLoopsLeft == 0) {
                if (Sensors.ArmedEnemies.Count > 0) {
                    SetNewTargetArmed();
                    HowManyIgnoreLoopsLeft = howManyLoopsIgnoreNewTargetsAfterFindingOne;
                } else {
                    SetNewTargetNotArmed();
                    HowManyIgnoreLoopsLeft = howManyLoopsIgnoreNewTargetsAfterFindingOne;
                }
            }

            if (HowManyIgnoreLoopsLeft > 0) HowManyIgnoreLoopsLeft--;

            yield return new WaitForSeconds(updateTime);
        }
    }

    public abstract void SetNewTargetNotArmed();

    public abstract void SetNewTargetArmed();

    public void StartCombatModule() {
        if (CurrentCoroutine == null) {
            CurrentCoroutine = StartCoroutine(UpdateCombatModule());
            active = true;
        }
    }

    public void StopCombatModule() {
        if (CurrentCoroutine != null) {
            StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
            Controls.ClearSecondaryTargetPos();
            Controls.SetOnlyLook(false);
            active = false;
        }
    }
}
