using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAlert : MonoBehaviour {
    public float ALERT_RANGE;
    public float ALERT_COOLDOWN;
    public bool RESPONDS;

    private bool AlertOnCooldown;
    private IShipMovement Controls;
    private FighterMovement FighterControls;
    private AiBase Ai;
    private CombatModule AttachedCombatModule;
    private Hitpoints AttachedHitpoints;
    private Sensors AttachedSensors;
    
    private void Start() {
        AlertOnCooldown = false;
        Controls = GetComponent<IShipMovement>();
        FighterControls = GetComponent<FighterMovement>();
        Ai = GetComponent<AiBase>();
        AttachedCombatModule = GetComponent<CombatModule>();
        AttachedHitpoints = GetComponent<Hitpoints>();
        AttachedSensors = GetComponent<Sensors>();
    }

    public void Alert(Vector2 location, bool first) {
        if (gameObject != null && !AlertOnCooldown && (AttachedCombatModule == null || !AttachedCombatModule.active && AttachedHitpoints.CurrentHp / AttachedHitpoints.maxHp >= AttachedCombatModule.RETREAT_HP_PERCENTAGE)) {
            if (first && ALERT_RANGE != 0) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ALERT_RANGE);
                foreach (Collider2D collider in colliders) {
                    if (tag == collider.tag) {
                        ShipAlert foundAlert = collider.GetComponent<ShipAlert>();
                        if (foundAlert != null) {
                            foundAlert.Alert(location, false);
                        }
                    }
                }
            }

            if (Vector2.Distance(transform.position, location) > AttachedSensors.sensorAreaRadius) {
                if (RESPONDS && FighterControls != null) {
                    FighterControls.SetPositionTarget(location);
                    StartCoroutine(StartCooldown());
                } else if (RESPONDS && Ai.combatModuleActive && !Ai.CombatModule.active) {
                    Controls.SetSecondaryTargetPos(location);
                    StartCoroutine(StartCooldown());
                } else if (!RESPONDS && first) {
                    StartCoroutine(StartCooldown());
                }
            }
        }
    }

    private IEnumerator StartCooldown() {
        AlertOnCooldown = true;
        yield return new WaitForSeconds(ALERT_COOLDOWN);
        AlertOnCooldown = false;
    }
}
