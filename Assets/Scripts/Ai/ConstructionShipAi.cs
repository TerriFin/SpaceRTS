using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionShipAi : MonoBehaviour, IAi {

    public GameObject deployedBuildingShipPrefab;

    public GameObject Building { get; set; }
    public float BuildingTime { get; set; }
    public float BuildingRadius { get; set; }
    public AsteroidField AsteroidField { get; set; }
    public Vector2 Target { get; set; }

    private ShipMovement Controls;
    private bool SpaceOccupied;

    private float BUILDING_RANGE;
    private LevelBorderManager BorderManager;
    public void InitializeAi() {
        Controls = GetComponent<ShipMovement>();
        SpaceOccupied = false;

        BUILDING_RANGE = 0.15f;
        BorderManager = FindObjectOfType<LevelBorderManager>();
    }

    private void OnDestroy() {
        if (AsteroidField != null) AsteroidField.MinesOnTheWay--;
    }

    public void ExecuteStep() {
        // If we are inside the building range...
        if (Vector2.Distance(transform.position, Target) <= BUILDING_RANGE) {
            // Check if there are buildings too close...
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, BuildingRadius, 1 << LayerMask.NameToLayer("Building"));
            // If not, start building, if there is, set the space occupied and start going away
            if (colliders.Length == 0) {
                StartBuilding();
                return;
            } else {
                SpaceOccupied = true;
                Target = CalculateAllColliderPosInArea(colliders) + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            }
        }

        // Check if building ship is out of bounds
        if (!BorderManager.LocationInsideBuildArea(transform.position)) {
            // Set a new target inside build area 
            if (!BorderManager.LocationInsideBuildArea(Target)) {
                if (Building.GetComponent<Selectable>().selectableType == Selectable.Types.mine) {
                    Target = (Vector2)AsteroidFieldManager.GetClosestAsteroidField(transform.position).transform.position + GetRandomCirclePos() * Random.value;
                    Controls.SetPrimaryTargetPos(Target);
                } else {
                    SpaceOccupied = true;
                    Target = BuildingManager.GetFactionCenterPoint(tag) * Random.Range(0.75f, 1f) + GetRandomCirclePos() * Random.Range(0f, 8f);
                    Controls.SetPrimaryTargetPos(Target);
                }
            }
        }

        // If we have arrived at current destination, set a new one (because the ship starts at destination this is instantly called)
        if (Controls.AreWeThereYet()) { 
            Controls.SetPrimaryTargetPos(Target);
        }

        // If previous space was occupied and we are now flying away, grab first opportunity to start building
        if (SpaceOccupied && Physics2D.OverlapCircle(transform.position, BuildingRadius, 1 << LayerMask.NameToLayer("Building")) == null) {
            StartBuilding();
            return;
        }
    }

    private void StartBuilding() {
        GameObject deployedBuildingShip = Instantiate(deployedBuildingShipPrefab);
        deployedBuildingShip.transform.rotation = transform.rotation;
        deployedBuildingShip.transform.position = transform.position;
        deployedBuildingShip.tag = tag;
        deployedBuildingShip.GetComponent<Hitpoints>().SetHpToPercentage(GetComponent<Hitpoints>().GetCurrentHpPercentage());

        deployedBuildingShip.GetComponent<BuildingShipLogic>().Initialize(Building, BuildingTime, AsteroidField);

        // This is so that selection screen is updated
        GetComponent<ShipAlert>().enabled = false;
        GetComponent<Hitpoints>().DestroyThis(false);
    }

    private Vector2 CalculateAllColliderPosInArea(Collider2D[] colliders) {
        Vector2 currentVector = Vector2.zero;

        foreach (Collider2D collider in colliders) {
            Vector2 vectorAwayFromShip = transform.position * 2 - collider.transform.position - transform.position;
            currentVector += vectorAwayFromShip;
        }

        return (Vector2) transform.position + currentVector;
    }

    private Vector2 GetRandomCirclePos() {
        float randomNumber = Random.Range(0, Mathf.PI * 2);
        return new Vector2(Mathf.Sin(randomNumber), Mathf.Cos(randomNumber));
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 1f, 0f, 0.33f);

        Gizmos.DrawLine(transform.position, Target);
        Gizmos.DrawWireSphere(Target, 0.75f);
    }
}
