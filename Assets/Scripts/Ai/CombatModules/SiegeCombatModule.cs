using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeCombatModule : CombatModule {

    public bool TURRET;

    public override void SetNewTargetArmed() {
        if (TURRET || Sensors.ArmedAllies.Count >= Sensors.ArmedEnemies.Count) {
            SetNewTargetNotArmed();
        } else {
            Collider2D closestEnemy = GetClosestEnemy();
            if (closestEnemy != null) {
                if (Vector2.Distance(transform.position, closestEnemy.transform.position) <= preferredCombatDistance * 0.75f) {
                    Controls.SetSecondaryTargetPos(closestEnemy.transform.position + (transform.position - closestEnemy.transform.position).normalized * preferredCombatDistance);
                } else {
                    Controls.SetSecondaryTargetPos(closestEnemy.transform.position);
                    Controls.SetOnlyLook(true);
                }
            }
        }
    }

    public override void SetNewTargetNotArmed() {
        Collider2D target = GetBuildingTarget();
        if (target == null) target = GetClosestEnemy();
        if (target != null) {
            Controls.SetSecondaryTargetPos(target.transform.position);
            Controls.SetOnlyLook(true);
        }
    }

    private Collider2D GetBuildingTarget() {
        foreach (Collider2D enemy in Sensors.Enemies) {
            if (enemy != null && enemy.gameObject.layer == LayerMask.NameToLayer("Building")) {
                return enemy;
            }
        }

        return null;
    }

    private Collider2D GetClosestEnemy() {
        Collider2D closestEnemy = Sensors.GetClosestMilitaryEnemy();
        if (closestEnemy == null) {
            closestEnemy = Sensors.GetClosestArmedEnemy();
            if (closestEnemy == null) {
                closestEnemy = Sensors.GetClosestEnemy();
            }
        }

        return closestEnemy;
    }
}
