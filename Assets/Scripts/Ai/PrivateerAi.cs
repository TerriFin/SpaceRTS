using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateerAi : MonoBehaviour, IAi {

    public float REQUIRED_MINERAL_STORAGE_LEVEL;
    public float timeSpentChasing;
    public float CHANCE_TO_GO_AROUND;
    public float CHANCE_TO_TARGET_UNDEFENDED_PLANETS;

    private IShipMovement Controls;
    private MineralStorage Storage;
    private LevelBorderManager BorderManager;
    private GameObject Target;

    public void InitializeAi() {
        Controls = GetComponent<IShipMovement>();
        Storage = GetComponent<MineralStorage>();
        BorderManager = FindObjectOfType<LevelBorderManager>();
        Target = null;
    }

    public void ExecuteStep() {
        if ((float) Storage.currentMineralStorage / (float) Storage.maxMineralStorage < REQUIRED_MINERAL_STORAGE_LEVEL || RelationShipManager.War[tag].Count == 0) {
            List<AsteroidField> fields = AsteroidFieldManager.GetAsteroidFieldsSortedByDistanceToLocation(transform.position);
            bool foundField = false;
            if (fields.Count > 0) {
                for (int i = 0; i < fields.Count; i++) {
                    if (fields[i].CurrentAsteroids <= 2) continue;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(fields[i].transform.position, 5.0f);
                    bool currentOk = true;
                    foreach (Collider2D collider in colliders) {
                        if (collider.gameObject.layer == LayerMask.NameToLayer("Building") && RelationShipManager.AreFactionsInWar(tag, collider.tag)) {
                            currentOk = false;
                            break;
                        }
                    }

                    if (currentOk) {
                        Asteroid[] asteroids = fields[i].GetComponentsInChildren<Asteroid>();
                        Asteroid closestAsteroid = null;
                        float distance = float.MaxValue;
                        foreach (Asteroid asteroid in asteroids) {
                            float distanceToAsteroid = Vector2.Distance(transform.position, asteroid.transform.position);
                            if (distanceToAsteroid < distance) {
                                distance = distanceToAsteroid;
                                closestAsteroid = asteroid;
                            }
                        }

                        if (closestAsteroid != null) {
                            Target = closestAsteroid.gameObject;
                            foundField = true;
                            break;
                        }
                    }
                }

                if (!foundField) Target = fields[0].gameObject;
                Controls.SetPrimaryTargetPos(Target.transform.position);
            }
        } else {
            if (Target == null || Target.CompareTag("Untagged") || !RelationShipManager.IsFactionAttackingFaction(tag, Target.tag)) {
                Target = GetNewTarget();
            }

            if (Controls.AreWeThereYet() && Target != null) {
                if (Random.Range(0.0f, 1.0f) < CHANCE_TO_GO_AROUND) {
                    Controls.SetSecondaryTargetPos((transform.position + Target.transform.position).normalized * BorderManager.CurrentSize * Random.Range(0.75f, 0.95f));
                    Controls.SetPrimaryTargetPos(Target.transform.position);
                } else {
                    Controls.SetPrimaryTargetPos(Target.transform.position);
                }
            }
        }
    }

    public GameObject GetNewTarget() {
        if (RelationShipManager.War[tag].Count > 0) {
            string factionToRaid = RelationShipManager.GetRandomOrGoodTargetFaction(tag);
            if (Random.Range(0.0f, 1.0f) < CHANCE_TO_TARGET_UNDEFENDED_PLANETS) {
                PlanetCaptureLogic toReturn = GetClosestUndefendedFactionPlanet(factionToRaid);
                if (toReturn != null) return toReturn.gameObject; 
            }

            return GetVulnerableEnemyField(factionToRaid);
        }

        return null;
    }

    private GameObject GetVulnerableEnemyField(string faction) {
        List<AsteroidField> sortedAsteroidFields = AsteroidFieldManager.GetAsteroidFieldsSortedByDistanceToLocation(FactionManager.Factions[faction].ai.FactionCenterPoint);
        List<AsteroidField> sortedEnemyAsteroidFields = new List<AsteroidField>();
        foreach (AsteroidField field in sortedAsteroidFields) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(field.transform.position, field.FIELD_RADIUS * 1.25f);
            foreach (Collider2D collider in colliders) {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Building") && collider.gameObject.CompareTag(faction)) {
                    sortedEnemyAsteroidFields.Add(field);
                }
            }
        }

       if (sortedEnemyAsteroidFields.Count > 0) return sortedEnemyAsteroidFields[sortedEnemyAsteroidFields.Count - 1].gameObject;
       return null;
    }

    private PlanetCaptureLogic GetClosestUndefendedFactionPlanet(string faction) {
        PlanetCaptureLogic toReturn = null;
        float distance = float.MaxValue;

        foreach (PlanetCaptureLogic planet in PlanetManager.FactionPlanets[faction]) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(planet.transform.position, 8.0f);
            bool foundDefence = false;

            foreach (Collider2D collider in colliders) {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Building") && RelationShipManager.AreFactionsInWar(tag, collider.tag)) {
                    foundDefence = true;
                    break;
                }
            }

            if (!foundDefence) {
                float currentDistance = Vector2.Distance(transform.position, planet.transform.position);
                if (currentDistance < distance) {
                    toReturn = planet;
                    distance = currentDistance;
                }
            }
        }

        return toReturn;
    }
}
