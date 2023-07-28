using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCombatModule : CombatModule {

    public float circleAdvancementAmount;
    public float REVERSE_DISTANCE;
    public float REVERSE_COOLDOWN_TIMER;

    private bool Clockwise;
    private bool CanReverse;

    private void Start() {
        Clockwise = Random.Range(0.0f, 1.0f) < 0.5f;
        CanReverse = true;
    }

    public override void SetNewTargetArmed() {
        Collider2D target = Sensors.GetClosestEnemy();
        if (target != null) {
            if (CanReverse && Vector2.Distance(transform.position, target.transform.position) < REVERSE_DISTANCE) {
                Clockwise = !Clockwise;
                StartCoroutine(ReverseCooldown());
            }

            if (Clockwise) {
                Vector2 targetVector = transform.position - target.transform.position;
                float targetRad = Mathf.Atan2(targetVector.y, targetVector.x) + circleAdvancementAmount;

                Controls.SetSecondaryTargetPos((Vector2)target.transform.position + new Vector2(Mathf.Cos(targetRad), Mathf.Sin(targetRad)) * preferredCombatDistance);
            } else {
                Vector2 targetVector = transform.position - target.transform.position;
                float targetRad = Mathf.Atan2(targetVector.y, targetVector.x) - circleAdvancementAmount;

                Controls.SetSecondaryTargetPos((Vector2)target.transform.position + new Vector2(Mathf.Cos(targetRad), Mathf.Sin(targetRad)) * preferredCombatDistance);
            }
        }
    }

    public override void SetNewTargetNotArmed() {
        Collider2D target = Sensors.GetClosestEnemy();
        if (target != null) {
            if (Clockwise) {
                Vector2 targetVector = transform.position - target.transform.position;
                float targetRad = Mathf.Atan2(targetVector.y, targetVector.x) + circleAdvancementAmount;

                Controls.SetSecondaryTargetPos((Vector2)target.transform.position + new Vector2(Mathf.Cos(targetRad), Mathf.Sin(targetRad)) * preferredCombatDistance);
            } else {
                Vector2 targetVector = transform.position - target.transform.position;
                float targetRad = Mathf.Atan2(targetVector.y, targetVector.x) - circleAdvancementAmount;

                Controls.SetSecondaryTargetPos((Vector2)target.transform.position + new Vector2(Mathf.Cos(targetRad), Mathf.Sin(targetRad)) * preferredCombatDistance);
            }
        }
    }

    private IEnumerator ReverseCooldown() {
        CanReverse = false;
        yield return new WaitForSeconds(REVERSE_COOLDOWN_TIMER);
        CanReverse = true;
    }
}
