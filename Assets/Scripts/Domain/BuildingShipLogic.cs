using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BuildingShipLogic : MonoBehaviour {

    public GameObject building;
    public float buildingTime;
    public AsteroidField AsteroidField { get; set; }

    public void Initialize(GameObject givenBuilding, float givenBuildingTime, AsteroidField givenAsteroidField = null) {
        building = givenBuilding;
        buildingTime = givenBuildingTime;
        AsteroidField = givenAsteroidField;

        if (AsteroidField != null) AsteroidField.MinesOnTheWay++;

        BuildingManager.Buildings[tag].Add(GetComponent<Hitpoints>());
        Selectable deployedBuildingShipSelectable = GetComponent<Selectable>();
        BuildingManager.BuildingAmountsByFactionAndType[tag][deployedBuildingShipSelectable.selectableType.ToString()].Add(deployedBuildingShipSelectable);

        StartCoroutine(ConstructBuilding());
    }

    private void OnDestroy() {
        if (AsteroidField != null) AsteroidField.MinesOnTheWay--;

        BuildingManager.Buildings[tag].Remove(GetComponent<Hitpoints>());
        Selectable deployedBuildingShipSelectable = GetComponent<Selectable>();
        BuildingManager.BuildingAmountsByFactionAndType[tag][deployedBuildingShipSelectable.selectableType.ToString()].Remove(deployedBuildingShipSelectable);
    }

    private IEnumerator ConstructBuilding() {
        yield return new WaitForSeconds(buildingTime);

        GameObject createdBuilding = Instantiate(building, transform.position, Quaternion.identity);
        createdBuilding.tag = tag;

        Hitpoints hitpoints = GetComponent<Hitpoints>();
        createdBuilding.GetComponent<Hitpoints>().SetHpToPercentage(hitpoints.GetCurrentHpPercentage());

        GetComponent<ShipAlert>().enabled = false;
        hitpoints.DestroyThis(false);
    }
}
