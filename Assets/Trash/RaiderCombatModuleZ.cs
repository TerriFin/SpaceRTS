using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderCombatModuleZ : CombatModule {
    public override void SetNewTargetArmed() {
        if (Sensors.ArmedAllies.Count > Sensors.ArmedEnemies.Count) {
            SetNewTargetNotArmed();
        } else {
            Controls.SetSecondaryTargetPos(transform.position + (Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * (Vector2.up * 10)));
        }
    }

    public override void SetNewTargetNotArmed() {
        if (AttachedTurret.Target != null) {
            Controls.SetSecondaryTargetPos(AttachedTurret.Target.transform.position + (transform.position - AttachedTurret.Target.transform.position).normalized * preferredCombatDistance);
        }
    }
}
