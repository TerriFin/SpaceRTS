using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonerCombatModuleX : MonoBehaviour, ICombatModule {

    public bool active;
    public float preferredCombatDistance;
    public float updateTime;

    private ShipMovement Controls;
    private Sensors Sensors;
    private Turret AttachedTurret;
    private Coroutine CurrentCoroutine;

    public void InitializeModule() {
        Controls = GetComponent<ShipMovement>();
        Sensors = GetComponent<Sensors>();
        AttachedTurret = GetComponentInChildren<Turret>();
        if (active) {
            CurrentCoroutine = StartCoroutine(GetNewAttackPos());
        }
    }

    private IEnumerator GetNewAttackPos() {
        while (true) {
            yield return new WaitForSeconds(updateTime);
            if (Sensors.Enemies.Count != 0) {
                if (AttachedTurret != null && AttachedTurret.Target != null) {
                    SetNewTarget();
                }
            }
        }
    }

    private void SetNewTarget() {
        Controls.SetSecondaryTargetPos(AttachedTurret.Target.transform.position + (transform.position * 2 - AttachedTurret.Target.transform.position - transform.position).normalized * preferredCombatDistance);
    }

    public void StartCombatModule() {
        if (CurrentCoroutine == null) {
            SetNewTarget();
            CurrentCoroutine = StartCoroutine(GetNewAttackPos());
            active = true;
        }
    }

    public void StopCombatModule() {
        if (CurrentCoroutine != null) {
            StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
            active = false;
        }
    }
}
