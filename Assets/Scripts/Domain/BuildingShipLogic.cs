using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingShipLogic : MonoBehaviour {

    public GameObject building;
    public float buildingTime;
    public AsteroidField AsteroidField { get; set; }

    private void Start() {
        StartCoroutine(ConstructBuilding());
    }

    public void AddToBuildingsManager() {
        BuildingManager.Buildings[tag].Add(GetComponent<Hitpoints>());
        Selectable deployedBuildingShipSelectable = GetComponent<Selectable>();
        BuildingManager.BuildingAmountsByFactionAndType[tag][deployedBuildingShipSelectable.selectableType.ToString()].Add(deployedBuildingShipSelectable);
    }

    private void OnDestroy() {
        if (AsteroidField != null) AsteroidField.MinesOnTheWay--;

        BuildingManager.Buildings[tag].Remove(GetComponent<Hitpoints>());
        Selectable deployedBuildingShipSelectable = GetComponent<Selectable>();
        BuildingManager.BuildingAmountsByFactionAndType[tag][deployedBuildingShipSelectable.selectableType.ToString()].Remove(deployedBuildingShipSelectable);
    }

    private IEnumerator ConstructBuilding() {
        yield return new WaitForSeconds(buildingTime);

        GameObject createdBuilding = Instantiate(building);
        createdBuilding.tag = tag;
        createdBuilding.transform.position = transform.position;

        // This is so that selection screen is updated
        GetComponent<ShipAlert>().enabled = false;
        Hitpoints hitpoints = GetComponent<Hitpoints>();
        createdBuilding.GetComponent<Hitpoints>().SetHpToPercentage(hitpoints.GetCurrentHpPercentage());
        hitpoints.DestroyThis(false);
    }
}
