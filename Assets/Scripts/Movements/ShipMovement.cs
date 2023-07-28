using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour, IShipMovement {

    public float speedForceAmount;
    public float rotationForceAmount;
    public float primaryDeadArea;
    public float secondaryDeadArea;
    public float allowedMissAmount;
    public float halfThrustMissAmount;
    public bool LOOKAT;
    public GameObject ORIGIN;

    private Vector2 PrimaryTargetPos;
    private Vector2 SecondaryTargetPos;
    private bool MovingForward;

    private Rigidbody2D Body;

    private void Start() {
        LOOKAT = false;
        MovingForward = false;
        Body = GetComponent<Rigidbody2D>();

        if (halfThrustMissAmount == 0) halfThrustMissAmount = 100;
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
            ApplyRotationAndForce(SecondaryTargetPos);
        } else if (PrimaryTargetPos != Vector2.zero) {
            ApplyRotationAndForce(PrimaryTargetPos);
        }

        if (AreWeThereYet()) {
            if (SecondaryTargetPos != Vector2.zero) {
                SecondaryTargetPos = Vector2.zero;
            } else if (PrimaryTargetPos != Vector2.zero) {
                PrimaryTargetPos = Vector2.zero;
            }
        }
    }

    private void ApplyRotationAndForce(Vector2 target) {
        float angleToTarget = Vector2.SignedAngle(target - (Vector2)transform.position, transform.up);
        // Check if we are facing the target, if so, apply thrust
        bool rightAngleCorrect = angleToTarget >= -allowedMissAmount;
        bool leftAngleCorrect = angleToTarget <= allowedMissAmount;
        if (rightAngleCorrect && leftAngleCorrect) {
            if (!LOOKAT) {
                Body.AddForce(speedForceAmount * Time.deltaTime * transform.up);
                MovingForward = true;
            }
        } else {
            bool rightAngleHalfThrustCorrect = angleToTarget >= -halfThrustMissAmount;
            bool leftAngleHalfThrustCorrect = angleToTarget <= halfThrustMissAmount;
            if (rightAngleHalfThrustCorrect && leftAngleHalfThrustCorrect) {
                Body.AddForce(speedForceAmount * Time.deltaTime * transform.up * 0.75f);
                MovingForward = true;
            }
            if (!rightAngleCorrect) {
                Body.rotation += rotationForceAmount * Time.deltaTime;
            } else if (!leftAngleCorrect) {
                Body.rotation -= rotationForceAmount * Time.deltaTime;
            }
        }
    }

    public bool AreWeThereYet() {
        if (SecondaryTargetPos != Vector2.zero) {
            return Vector2.Distance(transform.position, SecondaryTargetPos) < secondaryDeadArea;
        } else if (PrimaryTargetPos != Vector2.zero) {
            return Vector2.Distance(transform.position, PrimaryTargetPos) < primaryDeadArea;
        }

        MovingForward = false;
        return true;
    }

    public void SetPrimaryTargetPos(Vector2 target) {
        if (target == Vector2.zero) {
            target = new Vector2(Random.Range(0.01f, 0.02f), Random.Range(0.01f, 0.02f));
        }

        LOOKAT = false;
        PrimaryTargetPos = target;
    }

    public Vector2 GetPrimaryTargetPos() {
        return PrimaryTargetPos;
    }

    public void SetSecondaryTargetPos(Vector2 target) {
        if (target == Vector2.zero) {
            target = new Vector2(Random.Range(0.01f, 0.02f), Random.Range(0.01f, 0.02f));
        }

        LOOKAT = false;
        SecondaryTargetPos = target;
    }

    public Vector2 GetSecondaryTargetPos() {
        return SecondaryTargetPos;
    }

    public void ClearSecondaryTargetPos() {
        SecondaryTargetPos = Vector2.zero;
    }

    public void SetOnlyLook(bool look) {
        LOOKAT = look;
    }

    public void SetOrigin(GameObject origin) {
        ORIGIN = origin;
    }

    public GameObject GetOrigin() {
        return ORIGIN;
    }

    public bool ThrusterOn() {
        return MovingForward && !LOOKAT;
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
