using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderAi : MonoBehaviour, IAi {

    public float timeSpentChasing;
    public float CHANCE_TO_GO_AROUND;
    public float CHANCE_TO_TARGET_UNDEFENDED_PLANETS;

    private IShipMovement Controls;
    private LevelBorderManager BorderManager;
    private GameObject Target;

    public void InitializeAi() {
        Controls = GetComponent<IShipMovement>();
        BorderManager = FindObjectOfType<LevelBorderManager>();
        Target = null;
    }

    public void ExecuteStep() {
        if (PlanetManager.FactionPlanets["Untagged"].Count > 0) {
            if (Controls.AreWeThereYet()) {
                Controls.SetPrimaryTargetPos(PlanetManager.GetPlanetsFromFactionSortedByDistanceToLocation("Untagged", transform.position + new Vector3(Random.Range(-8.0f, 8.0f), Random.Range(-8.0f, 8.0f), 0.0f))[0].transform.position);
            }
        } else if (RelationShipManager.War[tag].Count > 0) {
            if (Target == null || !RelationShipManager.IsFactionAttackingFaction(tag, Target.tag)) {
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
        } else {
             if (Controls.AreWeThereYet() && Controls.GetOrigin() != null) {
                Controls.SetPrimaryTargetPos(Controls.GetOrigin().transform.position);
            }
        }
    }

    public GameObject GetNewTarget() {
        if (RelationShipManager.War[tag].Count > 0) {
            string factionToRaid = RelationShipManager.GetRandomFactionWeAreAttacking(tag);
            if (Random.Range(0.0f, 1.0f) < CHANCE_TO_TARGET_UNDEFENDED_PLANETS) {
                PlanetCaptureLogic undefendedPlanet = GetClosestUndefendedFactionPlanet(factionToRaid);
                if (undefendedPlanet != null) return undefendedPlanet.gameObject;
            } else {
                if (ShipsManager.CivShips[factionToRaid].Count > 0) {
                    return ShipsManager.GetFactionRandomCivShip(factionToRaid).gameObject;
                }
            }
        }

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
