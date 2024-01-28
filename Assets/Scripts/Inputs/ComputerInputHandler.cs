using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerInputHandler : MonoBehaviour {

    public float TAP_LENGTH;
    public GameObject clickIndicator;
    public float enemyBuildingCheckRadiusInPath;
    public string SCREENSHOT_PATH;
    public GameObject MESSAGE_SYSTEM;

    private float timeClickStarted = 0f;
    private LevelBorderManager BorderManager;
    private MineralBarUi commandCenterMineralUpdater;

    private void Start() {
        if (SystemInfo.deviceType == DeviceType.Handheld) Destroy(this);
        BorderManager = FindObjectOfType<LevelBorderManager>();
        commandCenterMineralUpdater = FindObjectOfType<MineralBarUi>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ScreenCapture.CaptureScreenshot(SCREENSHOT_PATH + System.Guid.NewGuid().ToString() + ".png", 1);
            print("SCREENSHOT TAKEN!");
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            Camera.main.orthographicSize = 25;
            if (MESSAGE_SYSTEM != null) Destroy(MESSAGE_SYSTEM);
        }

        if (!PauseMenu.IS_PAUSED && CheckTapIsInPlayArea(Input.mousePosition)) {
            if (Input.GetMouseButtonDown(0)) {
                timeClickStarted = Time.time;
            }

            if (Input.GetMouseButton(0)) {
                Vector3 currentCameraPos = Camera.main.transform.position;
                currentCameraPos.x -= Input.GetAxis("Mouse X");
                currentCameraPos.y -= Input.GetAxis("Mouse Y");
                if (BorderManager.LocationInsideCameraArea(currentCameraPos)) {
                    if (!BorderManager.LocationInsideCameraSoftArea(currentCameraPos)) {
                        currentCameraPos.x += Input.GetAxis("Mouse X") / 2;
                        currentCameraPos.y += Input.GetAxis("Mouse Y") / 2;
                    }

                    Camera.main.transform.position = currentCameraPos;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (Time.time - timeClickStarted <= TAP_LENGTH) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                    if (hit) {
                        if (FactionManager.PlayerFaction != null && FactionManager.PlayerFaction.factionTag.Equals(hit.transform.tag)) {
                            Selectable selected = hit.transform.GetComponent<Selectable>();
                            if (selected != null) {
                                SelectionManager.HandleSelection(selected);
                                return;
                            }
                        }
                    }

                    Vector3 worldCoordinates = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    worldCoordinates.z = 0;
                    if (BorderManager.LocationInsideBuildArea(worldCoordinates)) {
                        if (SelectionManager.selected.Count != 0) {
                            foreach (Selectable selected in SelectionManager.selected) {
                                selected.ReactToClick(worldCoordinates);
                            }
                            Instantiate(clickIndicator, worldCoordinates, Quaternion.identity);
                        } else if (BuildingPlacementManager.IsBuilding) {
                            int mineralCost = BuildingPlacementManager.Building.mineralCost;
                            FactionManager.PlayerFaction.ai.GetComponent<FactionAiBuildingManager>().BuildBuilding(BuildingPlacementManager.Building, worldCoordinates, 5.0f);
                            Instantiate(clickIndicator, worldCoordinates, Quaternion.identity);

                            if (!commandCenterMineralUpdater.EnoughMineralsForAnotherBuilding(mineralCost) || FactionManager.PlayerFaction.money < BuildingPlacementManager.Building.moneyCost) {
                                BuildingPlacementManager.StopPlacingBuilding();
                                SelectionStats.ResetSelection();
                            }
                        }
                    }
                }
            }
        }
    }

    private bool CheckTapIsInPlayArea(Vector3 tap) {
        return EventSystem.current.currentSelectedGameObject == null && tap.x > Screen.width / 5.1f;
    }
}
