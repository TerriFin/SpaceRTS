using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonerCombatModule : CombatModule {
    private Hitpoints Hitpoints;

    private void Start() {
        Hitpoints = GetComponent<Hitpoints>();
    }
    public override void SetNewTargetArmed() {
        if ((FactionManager.PlayerFaction != null && CompareTag(FactionManager.PlayerFaction.factionTag)) || ((float) Hitpoints.CurrentHp > (float) Hitpoints.maxHp * RETREAT_HP_PERCENTAGE && (float) (Sensors.ArmedAllies.Count + 1) * RETREAT_ENEMY_ADVANTAGE_PERCENTAGE > Sensors.ArmedEnemies.Count)) {
            if (AttachedTurret.Target != null) {
                Controls.SetSecondaryTargetPos(AttachedTurret.Target.transform.position + (transform.position - AttachedTurret.Target.transform.position).normalized * preferredCombatDistance);
            }
        } else {
            Collider2D closestArmedEnemy = Sensors.GetClosestMilitaryEnemy();
            if (closestArmedEnemy == null) {
                closestArmedEnemy = Sensors.GetClosestArmedEnemy();
            }

            if (closestArmedEnemy != null) {
                Vector2 targetRetreatPos = closestArmedEnemy.transform.position + (transform.position - closestArmedEnemy.transform.position).normalized * preferredCombatDistance * 2;
                Controls.SetPrimaryTargetPos(targetRetreatPos);
                Controls.SetSecondaryTargetPos(targetRetreatPos);
            }
        }
    }

    public override void SetNewTargetNotArmed() {
        if (AttachedTurret.Target != null) {
            Controls.SetSecondaryTargetPos(AttachedTurret.Target.transform.position + (transform.position - AttachedTurret.Target.transform.position).normalized * preferredCombatDistance * 0.6f);
        }
    }
}
