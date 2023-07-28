using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Turret : MonoBehaviour {

    public GameObject projectile;
    public float rotationSpeed;
    public float firingSpeed;
    public float turretInaccuracy;
    public float allowedMissDegree;
    public float kickBack;
    public float firingRange;
    public float forwardCompensation;
    public bool facesForward;
    public bool miningTurret;

    public Collider2D Target { get; private set; }
    public IShipMovement TargetMovement { get; private set; }

    private Sensors Sensors;
    private MineralStorage Storage;
    private AudioSource Source;
    private bool AllowedToFire;

    private void Start() {
        Sensors = GetComponentInParent<Sensors>();
        Storage = GetComponentInParent<MineralStorage>();
        Source = GetComponent<AudioSource>();
        Target = null;
        TargetMovement = null;
        AllowedToFire = true;

        tag = transform.parent.tag;
    }

    private void Update() {
        if (miningTurret) {
            if (Sensors.Asteroids.Count > 0) {
                if (Target == null || Target != Sensors.GetClosestAsteroid()) {
                    Target = Sensors.GetClosestAsteroid();
                }

                if (Target != null) {
                    float angleToTarget = Vector2.SignedAngle(Target.transform.position - transform.position, transform.up);
                    if (angleToTarget > allowedMissDegree) {
                        transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
                    } else if (angleToTarget < -allowedMissDegree) {
                        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
                    } else {
                        if (AllowedToFire && Vector2.Distance(transform.position, Target.transform.position) < 2f && Storage.FreeStorage() > 0) {
                            FireTurret();
                        }
                    }
                }
            } else if (facesForward) {
                Target = null;
                if (transform.localEulerAngles.z > allowedMissDegree) {
                    transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
                } else if (transform.localEulerAngles.z < -allowedMissDegree) {
                    transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
                }
            } else {
                Target = null;
            }
        } else {
            if (Sensors.Enemies != null && Sensors.Enemies.Count > 0) {
                Collider2D closestMilitaryEnemy = Sensors.GetClosestMilitaryEnemy();
                Collider2D closestArmedEnemy = Sensors.GetClosestArmedEnemy();
                Collider2D closestEnemy = Sensors.GetClosestEnemy();
                if (closestMilitaryEnemy != null) {
                    Target = closestMilitaryEnemy;
                    TargetMovement = Target.GetComponent<IShipMovement>();
                } else if (closestArmedEnemy != null) {
                    Target = closestArmedEnemy;
                    TargetMovement = Target.GetComponent<IShipMovement>();
                } else if (closestEnemy != null) {
                    Target = closestEnemy;
                    TargetMovement = Target.GetComponent<IShipMovement>();
                }

                if (Target != null) {
                    float angleToTarget = 0.0f;
                    if (TargetMovement != null && !TargetMovement.AreWeThereYet()) {
                        angleToTarget = Vector2.SignedAngle((Target.transform.position + Target.transform.up * forwardCompensation) - transform.position, transform.up);
                    } else {
                        angleToTarget = Vector2.SignedAngle(Target.transform.position - transform.position, transform.up);
                    }
                    
                    if (angleToTarget > allowedMissDegree) {
                        transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
                    } else if (angleToTarget < -allowedMissDegree) {
                        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
                    } else {
                        if (AllowedToFire && (firingRange == 0 || Vector2.Distance(transform.position, Target.transform.position) < firingRange)) {
                            FireTurret();
                        }
                    }
                }
            } else if (facesForward) {
                Target = null;
                TargetMovement = null;
                if (transform.localEulerAngles.z > allowedMissDegree) {
                    transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
                } else if (transform.localEulerAngles.z < -allowedMissDegree) {
                    transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
                }
            } else {
                Target = null;
                TargetMovement = null;
            }
        }
    }

    private void FireTurret() {
        AllowedToFire = false;
        Quaternion newProjectileRotation = transform.rotation * Quaternion.Euler(0, 0, transform.rotation.z + Random.Range(-turretInaccuracy, turretInaccuracy));
        GameObject createdProjectile = Instantiate(projectile, transform.position, newProjectileRotation);
        createdProjectile.tag = tag;

        if (Storage != null) {
            createdProjectile.GetComponent<Projectile>().AttachedShipStorage = Storage;
        }

        transform.Rotate(new Vector3(0, 0, Random.Range(-kickBack, kickBack)));
        if (Source != null) {
            Source.pitch = 1.0f + Random.Range(-0.1f, 0.1f);
            Source.Play();
        }

        StartCoroutine(ResetTurretFireSpeed());
    }

    private IEnumerator ResetTurretFireSpeed() {
        yield return new WaitForSeconds(firingSpeed);
        AllowedToFire = true;
    }

    private void OnDrawGizmos() {
        if (Target != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.transform.position);
            Gizmos.DrawWireSphere(Target.transform.position, 0.25f);
        }
    }
}
