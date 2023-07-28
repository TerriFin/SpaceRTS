using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontShootingLogic : MonoBehaviour {

    public GameObject PROJECTILE;
    public Transform PROJECTILE_ORIGIN;
    public float SHOOTING_DISTANCE;
    public float RELOAD_TIMER;
    public bool SHOOTS_SHIPS;
    public bool SHOOTS_BUILDINGS;
    public List<GameObject> RAYCAST_SOURCES;

    private bool CanFire;
    private Sensors Sensors;
    private AudioSource Source;

    private void Start() {
        CanFire = true;
        Sensors = GetComponent<Sensors>();
        Source = GetComponent<AudioSource>();
    }

    private void Update() {
        if (CanFire && Sensors.Enemies.Count > 0 && EnemyInFront()) {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot() {
        GameObject createdProjectile = Instantiate(PROJECTILE, PROJECTILE_ORIGIN.position, PROJECTILE_ORIGIN.rotation);
        createdProjectile.tag = tag;
        CanFire = false;

        if (Source != null) {
            Source.pitch = 1.0f + Random.Range(-0.1f, 0.1f);
            Source.Play();
        }

        yield return new WaitForSeconds(RELOAD_TIMER);
        CanFire = true;
    }

    private bool EnemyInFront() {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        foreach (GameObject raycastSource in RAYCAST_SOURCES) {
            hits.AddRange(Physics2D.RaycastAll(raycastSource.transform.position, transform.rotation * Vector2.up, SHOOTING_DISTANCE));
        }

        foreach (RaycastHit2D hit in hits) {
            if (RelationShipManager.AreFactionsInWar(tag, hit.transform.tag)) {
                if (SHOOTS_SHIPS && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ship")) {
                    return true;
                }
                if (SHOOTS_BUILDINGS && hit.transform.gameObject.layer == LayerMask.NameToLayer("Building")) {
                    return true;
                }
            }
        }

        return false;
    }
}
