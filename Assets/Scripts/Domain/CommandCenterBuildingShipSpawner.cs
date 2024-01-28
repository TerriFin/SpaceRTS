using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenterBuildingShipSpawner : MonoBehaviour {

    public GameObject BuildingShip;

    public void SpawnBuildingShip(Vector2 targetPos, GameObject building, float buildTime, float buildRadius, AsteroidField asteroidField = null) {
        GameObject constructionShip = Instantiate(BuildingShip, transform.position, Quaternion.identity);
        constructionShip.tag = tag;

        ConstructionShipAi ai = constructionShip.GetComponent<ConstructionShipAi>();
        ai.Target = targetPos;
        ai.Building = building;
        ai.BuildingTime = buildTime;
        ai.BuildingRadius = buildRadius;
        ai.AsteroidField = asteroidField;
    }
}
