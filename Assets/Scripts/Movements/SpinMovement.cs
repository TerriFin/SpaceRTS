using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMovement : MonoBehaviour, IShipMovement {

    public float SPIN_SPEED;
    public float MOVEMENT_FORCE;
    public float PRIMARY_DEAD_AREA;
    public float SECONDARY_DEAD_AREA;
    public GameObject ORIGIN;

    public Vector2 PrimaryTargetPos { get; private set; }
    public Vector2 SecondaryTargetPos { get; private set; }

    private Rigidbody2D Body;

    private void Start() {
        Body = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy() {
        if (ORIGIN != null) {
            Production productionScript = ORIGIN.GetComponent<Production>();

            if (productionScript != null) {
                productionScript.ShipDestroyed();
            }
        }
    }

    private void Update() {
        if (SecondaryTargetPos != Vector2.zero) {
            ApplyMotion(SecondaryTargetPos);
        } else if (PrimaryTargetPos != Vector2.zero) {
            ApplyMotion(PrimaryTargetPos);
        }

        if (AreWeThereYet()) {
            if (SecondaryTargetPos != Vector2.zero) {
                SecondaryTargetPos = Vector2.zero;
            } else if (PrimaryTargetPos != Vector2.zero) {
                PrimaryTargetPos = Vector2.zero;
            }
        }
    }

    private void ApplyMotion(Vector2 target) {
        // Add spin cuz cool
        Body.rotation += SPIN_SPEED * Time.deltaTime;
        // Check if we are there yet
        float distanceToTarget = Vector2.Distance(transform.position, target);
        if (distanceToTarget > PRIMARY_DEAD_AREA) {
            Vector2 direction = new Vector2(target.x - transform.position.x, target.y - transform.position.y).normalized;
            Body.AddForce(MOVEMENT_FORCE * Time.deltaTime * direction);
        }
    }

    public bool AreWeThereYet() {
        if (SecondaryTargetPos != Vector2.zero) {
            return Vector2.Distance(transform.position, SecondaryTargetPos) < SECONDARY_DEAD_AREA;
        } else if (PrimaryTargetPos != Vector2.zero) {
            return Vector2.Distance(transform.position, PrimaryTargetPos) < PRIMARY_DEAD_AREA;
        }

        return true;
    }

    public void SetPrimaryTargetPos(Vector2 target) {
        if (target == Vector2.zero) {
            target = new Vector2(Random.Range(0.01f, 0.02f), Random.Range(0.01f, 0.02f));
        }

        PrimaryTargetPos = target;
    }

    public Vector2 GetPrimaryTargetPos() {
        return PrimaryTargetPos;
    }

    public void SetSecondaryTargetPos(Vector2 target) {
        if (target == Vector2.zero) {
            target = new Vector2(Random.Range(0.01f, 0.02f), Random.Range(0.01f, 0.02f));
        }

        SecondaryTargetPos = target;
    }

    public Vector2 GetSecondaryTargetPos() {
        return SecondaryTargetPos;
    }

    public void ClearSecondaryTargetPos() {
        SecondaryTargetPos = Vector2.zero;
    }

    public void SetOnlyLook(bool look) {
        return;
    }

    public void SetOrigin(GameObject origin) {
        ORIGIN = origin;
    }

    public GameObject GetOrigin() {
        return ORIGIN;
    }

    private void OnDrawGizmosSelected() {
        if (PrimaryTargetPos != Vector2.zero) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, PrimaryTargetPos);
        }

        if (SecondaryTargetPos != Vector2.zero) {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.position, SecondaryTargetPos);
        }
    }
}
