using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildingPlacementManager : MonoBehaviour {

    public static bool IsBuilding;
    public static FactionBuilding Building;

    private static GameObject Ghost;
    private static GameObject BuildingBoundary;

    public static void Reset() {
        Ghost = Instantiate((GameObject)Resources.Load("BuildingGhost"));
        BuildingBoundary = Instantiate((GameObject)Resources.Load("BuildingBoundary"));

        Ghost.transform.position = Vector3.zero;
        BuildingBoundary.transform.position = Vector3.zero;

        Ghost.SetActive(false);
        BuildingBoundary.SetActive(false);
    }

    public static void StartPlacingBuilding(FactionBuilding buildingData) {

        IsBuilding = true;

        Building = buildingData;

        Ghost.SetActive(true);
        BuildingBoundary.SetActive(true);

        Ghost.GetComponent<SpriteRenderer>().sprite = Building.building.GetComponent<SpriteRenderer>().sprite;
        BuildingBoundary.transform.localScale = new Vector3(Building.buildingRadius * 4, Building.buildingRadius * 4, 1);
    }

    public static void StopPlacingBuilding() {

        IsBuilding = false;

        Ghost.SetActive(false);
        BuildingBoundary.SetActive(false);
    }
}