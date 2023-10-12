using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderCombatModule : CombatModule {
    public override void SetNewTargetArmed() {
        if (Sensors.ArmedAlliesMilitary.Count >= 0 || Sensors.ArmedAllies.Count + 1 >= Sensors.ArmedEnemies.Count) {
            SetNewTargetNotArmed();
        } else {
            SetEscapeTargetPosFromClosestArmedEnemy(BuildingManager.GetFactionCenterPoint(tag));
        }
    }

    public override void SetNewTargetNotArmed() {
        if (AttachedTurret.Target != null) {
            Controls.SetSecondaryTargetPos(AttachedTurret.Target.transform.position + (transform.position - AttachedTurret.Target.transform.position).normalized * preferredCombatDistance);
        }
    }

    private void SetEscapeTargetPosFromClosestArmedEnemy(Vector2 factionCenterPoint) {
        Collider2D closestArmedEnemy = Sensors.GetClosestMilitaryEnemy();
        if (closestArmedEnemy == null) {
            Controls.SetPrimaryTargetPos(factionCenterPoint);
            Controls.SetSecondaryTargetPos(factionCenterPoint);
        } else {
            Vector2 targetRetreatPos = closestArmedEnemy.transform.position + (transform.position - closestArmedEnemy.transform.position).normalized * preferredCombatDistance * 6;
            Controls.SetPrimaryTargetPos(targetRetreatPos);
            Controls.SetSecondaryTargetPos(targetRetreatPos);
        }
    }
}
