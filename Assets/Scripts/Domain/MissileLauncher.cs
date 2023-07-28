using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour {

    public Transform[] LAUNCH_POINTS;
    public bool POINTED;
    public GameObject MISSILE;
    public int MISSILE_MAGAZINE_SIZE;
    public float TIME_BETWEEN_SHOTS;
    public float RELOAD_TIME;
    public bool RAIDER;

    private int CurrentMissiles;
    private bool CanFireMissile;

    private Sensors sensors;

    private void Start() {
        CurrentMissiles = MISSILE_MAGAZINE_SIZE;
        CanFireMissile = true;

        sensors = GetComponent<Sensors>();
    }

    private void Update() {
        if (CanFireMissile) {
            if (RAIDER) {
                if (sensors.Enemies.Count > 0) {
                    LaunchMissile(sensors.GetRandomEnemy());
                } else if (sensors.ArmedEnemies.Count > 0) {
                    LaunchMissile(sensors.GetRandomArmedEnemy());
                }
            } else {
                if (sensors.ArmedEnemies.Count > 0) {
                    LaunchMissile(sensors.GetRandomArmedEnemy());
                } else if (sensors.Enemies.Count > 0) {
                    LaunchMissile(sensors.GetRandomEnemy());
                }
            }
        }
    }

    public void LaunchMissile(Collider2D enemy) {
        if (CanFireMissile && enemy != null) {
            foreach (Transform launcher in LAUNCH_POINTS) {
                Missile missile = Instantiate(MISSILE, launcher.position, launcher.rotation).GetComponent<Missile>();
                missile.tag = tag;
                missile.MISSILE_TARGET = enemy.gameObject;
            }
            CurrentMissiles--;
            CanFireMissile = false;
            if (CurrentMissiles == 0) {
                StartCoroutine(StartReloadMissilesTimer());
            } else {
                StartCoroutine(StartBetweenShotTimer());
            }
        }
    }

    private IEnumerator StartBetweenShotTimer() {
        CanFireMissile = false;
        yield return new WaitForSeconds(TIME_BETWEEN_SHOTS);
        CanFireMissile = true;
    }

    private IEnumerator StartReloadMissilesTimer() {
        CanFireMissile = false;
        yield return new WaitForSeconds(RELOAD_TIME);
        CurrentMissiles = MISSILE_MAGAZINE_SIZE;
        CanFireMissile = true;
    }
}
