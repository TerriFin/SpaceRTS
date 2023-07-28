using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionAiBuildingManager : MonoBehaviour {

    private UseDefensiveFleetsBlock DefenceFleetsManager;

    private void Start() {
        DefenceFleetsManager = GetComponent<UseDefensiveFleetsBlock>();
    }

    public bool BuildBuilding(FactionBuilding buildingData, Vector2 position, float enemyBuildingCheckRadiusInPath, AsteroidField asteroidField = null) {
        if (FactionManager.Factions[tag].money >= buildingData.moneyCost) {
            List<CommandCenterBuildingShipSpawner> centers = GetCommandCenters();
            List<CommandCenterBuildingShipSpawner> centersWithMinerals = FilterCommandCentersMinerals(centers, buildingData);
            CommandCenterBuildingShipSpawner chosenCenter = GetClosestCenterToPoint(centersWithMinerals, position);
            List<Collider2D> EnemyBuildingsInBuildingPath = chosenCenter != null ? HostileBuildingsBetweenTwoPoints(chosenCenter.transform.position, position, enemyBuildingCheckRadiusInPath) : new List<Collider2D>();

            if (chosenCenter != null && (EnemyBuildingsInBuildingPath.Count == 0 || (FactionManager.PlayerFaction != null && CompareTag(FactionManager.PlayerFaction.factionTag)))) {
                MineralStorage mineralStorage = chosenCenter.GetComponent<MineralStorage>();
                mineralStorage.currentMineralStorage -= buildingData.mineralCost;
                mineralStorage.ShowOrExtendStorageBar();
                FactionManager.Factions[tag].money -= buildingData.moneyCost;
                chosenCenter.SpawnBuildingShip(position, buildingData.building, buildingData.buildTime, buildingData.buildingRadius, asteroidField);
                return true;
            } else if (chosenCenter != null && EnemyBuildingsInBuildingPath.Count > 0) {
                Collider2D target = EnemyBuildingsInBuildingPath[Random.Range(0, EnemyBuildingsInBuildingPath.Count)];
                if (target != null) DefenceFleetsManager.CheckLocation(target.transform.position, Random.Range(0, 2), target.tag);
            }
        }

        return false;
    }

    public List<Collider2D> HostileBuildingsBetweenTwoPoints(Vector2 locationOne, Vector2 locationTwo, float circleRadius) {
        float distanceBetweenLocations = Vector2.Distance(locationOne, locationTwo);
        int howManyCircles = (int) (distanceBetweenLocations / circleRadius);
        if (howManyCircles == 0) howManyCircles = 1;
        float distanceBetweenCircles = distanceBetweenLocations / howManyCircles + 1;
        Vector2 rotationVector = (locationTwo - locationOne);
        rotationVector.Normalize();

        List<Collider2D> foundEnemyBuildings = new List<Collider2D>();
        for (int i = 1; i < howManyCircles - 1; i++) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(locationOne + (rotationVector * (distanceBetweenCircles * i)), circleRadius);
            foreach (Collider2D collider in colliders) {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Building") && RelationShipManager.AreFactionsInWar(tag, collider.tag) && !foundEnemyBuildings.Contains(collider)) {
                    foundEnemyBuildings.Add(collider);
                }
            }
        }

        return foundEnemyBuildings;
    }

    private List<CommandCenterBuildingShipSpawner> GetCommandCenters() {
        List<CommandCenterBuildingShipSpawner> toReturn = new List<CommandCenterBuildingShipSpawner>();

        foreach (Hitpoints building in BuildingManager.Buildings[tag]) {
            CommandCenterBuildingShipSpawner foundSpawner = building.GetComponent<CommandCenterBuildingShipSpawner>();
            if (foundSpawner != null) {
                toReturn.Add(foundSpawner);
            }
        }

        return toReturn;
    }

    private List<CommandCenterBuildingShipSpawner> FilterCommandCentersMinerals(List<CommandCenterBuildingShipSpawner> centers, FactionBuilding buildingData) {
        List<CommandCenterBuildingShipSpawner> toReturn = new List<CommandCenterBuildingShipSpawner>();

        foreach (CommandCenterBuildingShipSpawner center in centers) {
            MineralStorage foundStorage = center.GetComponent<MineralStorage>();
            if (foundStorage != null && foundStorage.currentMineralStorage >= buildingData.mineralCost) {
                toReturn.Add(center);
            }
        }

        return toReturn;
    }

    private CommandCenterBuildingShipSpawner GetClosestCenterToPoint(List<CommandCenterBuildingShipSpawner> centers, Vector2 target) {
        if (centers.Count == 0) {
            return null;
        }

        CommandCenterBuildingShipSpawner toReturn = centers[0];
        float currentDistance = float.MaxValue;

        foreach (CommandCenterBuildingShipSpawner center in centers) {
            float distanceToCenter = Vector2.Distance(center.transform.position, target);
            if (distanceToCenter < currentDistance) {
                toReturn = center;
                currentDistance = distanceToCenter;
            }
        }

        return toReturn;
    }
}
