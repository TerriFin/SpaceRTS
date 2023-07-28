using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderCombatModuleX : MonoBehaviour, ICombatModule {

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
                if (Sensors.ArmedEnemies.Count > 0) {
                    SetNewTargetEscape();
                } else {
                    if (AttachedTurret != null && AttachedTurret.Target != null) {
                        if (Controls.AreWeThereYet()) {
                            SetNewTarget();
                        }
                    }
                }
                
            }
        }
    }

    private void SetNewTarget() {
        float randomNumber = Random.Range(0, Mathf.PI * 2);
        Vector2 randomCircleSpot = new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber) * preferredCombatDistance);
        Controls.SetSecondaryTargetPos((Vector2)AttachedTurret.Target.transform.position + randomCircleSpot);
    }

    private void SetNewTargetEscape() {
        Controls.SetSecondaryTargetPos(transform.position + (transform.position * 2 + AttachedTurret.Target.transform.position - transform.position).normalized * 3);
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
