using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandCenterBuildMenu : MonoBehaviour {

    public BuildButton[] buttons;
    public GameObject CancelBuildWindow;

    private void Start() {
        // Get build buttons
        buttons = GetComponentsInChildren<BuildButton>();

        for (int i = 0; i < buttons.Length; i++) {
            // Check that there are enough buttons
            if (i < BuildableBuildingsManager.BuildableBuildings[FactionManager.PlayerFaction.factionTag].Count) {

                // Get current button and associated building data
                BuildButton buttonAttributes = buttons[i];
                FactionBuilding buildingData = BuildableBuildingsManager.BuildableBuildings[FactionManager.PlayerFaction.factionTag][i];

                // Set button attributes so that it can react to events properly
                buttonAttributes.buildingName = buildingData.building.name;
                buttonAttributes.mineralCost = buildingData.mineralCost;
                buttonAttributes.moneyCost = buildingData.moneyCost;

                Button button = buttonAttributes.GetComponent<Button>();

                button.image.sprite = buildingData.building.GetComponent<SpriteRenderer>().sprite;
                // Use lambda here to pass data into function
                button.onClick.AddListener(() => StartPlacingNewBuilding(buildingData));
            } else {
                // We go here if there are less buildings than buttons
                Destroy(buttons[i].gameObject);
            }
        }
    }

    private void StartPlacingNewBuilding(FactionBuilding buildingData) {
        GameObject cancelBuildingWindow = Instantiate(CancelBuildWindow, FindObjectOfType<Canvas>().transform);
        cancelBuildingWindow.GetComponent<CancelBuildWindow>().InitializeCancelBuildWindow(buildingData.building);
        SelectionStats.SetCurrentlySelected(cancelBuildingWindow);
        Destroy(gameObject);

        BuildingPlacementManager.StartPlacingBuilding(buildingData);
    }
}
