using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrawlerCombatModule : CombatModule {
    private Hitpoints Hitpoints;

    private void Start() {
        Hitpoints = GetComponent<Hitpoints>();
    }
    public override void SetNewTargetArmed() {
        if ((FactionManager.PlayerFaction != null && CompareTag(FactionManager.PlayerFaction.factionTag)) || (float) Hitpoints.CurrentHp > (float) Hitpoints.maxHp * RETREAT_HP_PERCENTAGE || (float) (Sensors.ArmedAllies.Count + 1) * RETREAT_ENEMY_ADVANTAGE_PERCENTAGE < Sensors.ArmedEnemies.Count) {
            if (AttachedTurret.Target != null) {
                float randomNumber = Random.Range(0, Mathf.PI * 2);
                Vector2 randomCircleSpot = new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber)) * preferredCombatDistance;
                Controls.SetSecondaryTargetPos((Vector2)AttachedTurret.Target.transform.position + (randomCircleSpot + (Vector2)(transform.position - AttachedTurret.Target.transform.position).normalized * preferredCombatDistance).normalized * preferredCombatDistance);
            }
        } else {
            Vector2 factionCenterPoint = BuildingManager.GetFactionCenterPoint(tag);
            if (factionCenterPoint == Vector2.zero || Vector2.Distance(transform.position, factionCenterPoint) < 10) {
                Collider2D closestArmedEnemy = Sensors.GetClosestMilitaryEnemy();
                if (closestArmedEnemy == null) {
                    closestArmedEnemy = Sensors.GetClosestArmedEnemy();
                }

                if (closestArmedEnemy != null) {
                    Vector2 targetRetreatPos = closestArmedEnemy.transform.position + (transform.position - closestArmedEnemy.transform.position).normalized * preferredCombatDistance * 5;
                    Controls.SetSecondaryTargetPos(targetRetreatPos);
                }
            } else {
                Controls.SetSecondaryTargetPos(factionCenterPoint);
            }

        }
    }

    public override void SetNewTargetNotArmed() {
        if (AttachedTurret.Target != null) {
            Controls.SetSecondaryTargetPos(AttachedTurret.Target.transform.position);
        }
    }
}
