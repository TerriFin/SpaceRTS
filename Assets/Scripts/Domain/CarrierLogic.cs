using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierLogic : MonoBehaviour {

    public GameObject FIGHTER;
    public float FIGHTER_DEAD_ZONE;
    public int MAX_AMOUNT;
    public float PRODUCTION_TIME;
    public float CHECK_ENEMIES_TIMER;

    public List<FighterMovement> Fighters { get; private set; }
    private Sensors Sensors;

    private void Start() {
        Fighters = new List<FighterMovement>();
        Sensors = GetComponent<Sensors>();

        // Apply bonus to difficult AI
        float aiDifficultyModifier = FactionManager.Factions[tag].aiBonusMultiplier - 1;
        if (aiDifficultyModifier != 0) {
            PRODUCTION_TIME -= PRODUCTION_TIME * aiDifficultyModifier;
        }

        StartCoroutine(ProduceFighters());
        StartCoroutine(UseFighters());
    }

    public void FighterDestroyed(FighterMovement fighter) {
        Fighters.Remove(fighter);
    }

    private IEnumerator ProduceFighters() {
        while (true) {
            yield return new WaitForSeconds(PRODUCTION_TIME);
            if (Fighters.Count < MAX_AMOUNT) {
                FighterMovement createdFighter = Instantiate(FIGHTER, transform.position, transform.rotation).GetComponent<FighterMovement>();
                createdFighter.tag = tag;
                createdFighter.CARRIER = gameObject;
                createdFighter.TARGET_DEAD_AREA = FIGHTER_DEAD_ZONE;
                Fighters.Add(createdFighter);
            }
        }
    }

    private IEnumerator UseFighters() {
        while (true) {
            yield return new WaitForSeconds(CHECK_ENEMIES_TIMER);
            if (Sensors.ArmedEnemies.Count > 0) {
                Collider2D armedEnemy = Sensors.GetClosestMilitaryEnemy();
                if (armedEnemy == null) {
                    armedEnemy = Sensors.GetClosestArmedEnemy();
                }

                if (armedEnemy != null) {
                    foreach (FighterMovement fighter in Fighters) {
                        if (fighter.TARGET == null) fighter.TARGET = armedEnemy.gameObject;
                    }
                }
            } else if (Sensors.Enemies.Count > 0) {
                Collider2D enemy = Sensors.GetRandomEnemy();
                if (enemy != null) {
                    foreach (FighterMovement fighter in Fighters) {
                        if (fighter.TARGET == null) fighter.TARGET = enemy.gameObject;
                    }
                }
            }
        }
    }
}
