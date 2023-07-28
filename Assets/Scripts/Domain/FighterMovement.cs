using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterMovement : MonoBehaviour {

    public GameObject CARRIER;
    public float FIGHTER_RANGE;
    public float TARGET_DEAD_AREA;
    public float SPEED;
    public float TURN_RATE;
    public float CHECK_NEW_TARGET_TIMER;
    public float PEACE_CHECK_TIMER;
    public GameObject TARGET;
    public Vector2 POSITION_TARGET;

    private Rigidbody2D Body;
    private Sensors Sensors;

    private void Start() {
        TARGET = null;
        POSITION_TARGET = Vector2.zero;

        Body = GetComponent<Rigidbody2D>();
        Sensors = GetComponent<Sensors>();

        StartCoroutine(SetNewTarget());
        StartCoroutine(CheckIfStillAtWar());
    }

    private void Update() {
        if (CARRIER != null) {
            if (POSITION_TARGET != Vector2.zero && TARGET == null) {
                if (Vector2.Distance(transform.position, POSITION_TARGET) > TARGET_DEAD_AREA) {
                    GoToLocation(POSITION_TARGET);
                    return;
                } else {
                    POSITION_TARGET = Vector2.zero;
                }
            }

            float distanceToCarrier = Vector2.Distance(transform.position, CARRIER.transform.position);
            if (TARGET == null) {
                if (distanceToCarrier > TARGET_DEAD_AREA) {
                    GoToLocation(CARRIER.transform.position);
                }
            } else {
                if (distanceToCarrier < FIGHTER_RANGE) {
                    GoToLocation(TARGET.transform.position);
                } else {
                    TARGET = null;
                    GoToLocation(CARRIER.transform.position);
                }
            }
        } else {
            Hitpoints fighterHitpoints = GetComponent<Hitpoints>();
            fighterHitpoints.TakeDamage(fighterHitpoints.maxHp, Vector2.zero, tag);
        }
    }

    private void OnDestroy() {
        if (CARRIER != null) {
            CarrierLogic carrierLogic = CARRIER.GetComponent<CarrierLogic>();
            if (carrierLogic != null) {
                carrierLogic.FighterDestroyed(this);
            }
        }
    }

    public void SetPositionTarget(Vector2 position) {
        if (POSITION_TARGET == Vector2.zero && TARGET == null && (CARRIER != null && Vector2.Distance(CARRIER.transform.position, position) < FIGHTER_RANGE)) {
            POSITION_TARGET = position;
        }
    }

    private void GoToLocation(Vector2 location) {
        float angleToLocation = Vector2.SignedAngle(location - (Vector2)transform.position, transform.up);
        Body.AddForce(transform.up * SPEED * Time.deltaTime);
        if (angleToLocation < 0) {
            Body.rotation += TURN_RATE * Time.deltaTime;
        } else {
            Body.rotation -= TURN_RATE * Time.deltaTime;
        }
    }

    private IEnumerator SetNewTarget() {
        while (true) {
            yield return new WaitForSeconds(CHECK_NEW_TARGET_TIMER);
            if (Sensors.ArmedEnemies.Count > 0) {
                Collider2D armedEnemy = Sensors.GetClosestArmedEnemy();
                if (armedEnemy != null) TARGET = armedEnemy.gameObject;
            } else if (Sensors.Enemies.Count > 0) {
                Collider2D enemy = Sensors.GetRandomEnemy();
                if (enemy != null) TARGET = enemy.gameObject;
            }
        }
    }

    private IEnumerator CheckIfStillAtWar() {
        while (true) {
            yield return new WaitForSeconds(PEACE_CHECK_TIMER);
            if (TARGET != null && !RelationShipManager.AreFactionsInWar(tag, TARGET.tag)) TARGET = null;
        }
    }
}
