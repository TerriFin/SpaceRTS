using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderCombatModule : CombatModule {

    private Hitpoints Hitpoints;

    private void Start() {
        Hitpoints = GetComponent<Hitpoints>();
    }
    public override void SetNewTargetArmed() {
        if ((Sensors.ArmedEnemiesMilitary.Count > 0 && Sensors.ArmedAllies.Count == 0) || (float) Hitpoints.CurrentHp < (float) Hitpoints.maxHp * RETREAT_HP_PERCENTAGE) {
            SetEscapeTargetPosFromClosestArmedEnemy(BuildingManager.GetFactionCenterPoint(tag));
        } else {
            SetNewTargetNotArmed();
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
