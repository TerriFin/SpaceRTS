using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class TouchInputHandler : MonoBehaviour {

    public float TAP_LENGTH;
    public float MIN_ZOOM_LEVEL;
    public float MAX_ZOOM_LEVEL;
    public float SMOOTH_MAX_ZOOM_LEVEL;
    public GameObject clickIndicator;

    private float timeTouchStarted;
    private float lastDistanceBetweenTouches;
    private float currentZoomLevel;
    private Vector3 ZoomStartPos;
    private Vector3 ZoomStartCameraPos;
    private LevelBorderManager BorderManager;
    private MineralBarUi commandCenterMineralUpdater;

    private void Start() {
        timeTouchStarted = 0f;
        lastDistanceBetweenTouches = 0f;
        currentZoomLevel = Camera.main.orthographicSize;
        ZoomStartPos = Vector3.zero;
        ZoomStartCameraPos = Vector3.zero;
        BorderManager = FindObjectOfType<LevelBorderManager>();
        commandCenterMineralUpdater = FindObjectOfType<MineralBarUi>();
    }

    private void Update() {
        if (!PauseMenu.IS_PAUSED && Input.touchCount >= 1) {
            Touch firstTouch = Input.GetTouch(0);
            if (CheckTapIsInPlayArea(firstTouch)) {
                if (Input.touchCount == 1) {
                    if (firstTouch.phase == TouchPhase.Began) {
                        timeTouchStarted = Time.time;
                    }

                    if (firstTouch.phase == TouchPhase.Moved && !BuildingPlacementManager.IsBuilding) {
                        Vector3 currentCameraPos = Camera.main.transform.position;
                        currentCameraPos.x -= firstTouch.deltaPosition.x / 20;
                        currentCameraPos.y -= firstTouch.deltaPosition.y / 20;
                        if (BorderManager.LocationInsideCameraArea(currentCameraPos)) {
                            if (!BorderManager.LocationInsideCameraSoftArea(currentCameraPos)) {
                                currentCameraPos.x += firstTouch.deltaPosition.x / 40;
                                currentCameraPos.y += firstTouch.deltaPosition.y / 40;
                            }

                            Camera.main.transform.position = currentCameraPos;
                        }
                    }

                    if (firstTouch.phase == TouchPhase.Ended) {
                        if (Time.time - timeTouchStarted <= TAP_LENGTH) {
                            Ray ray = Camera.main.ScreenPointToRay(firstTouch.position);
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
                } else if (Input.touchCount == 2) {
                    // Handle the zoom
                    Touch secondTouch = Input.GetTouch(1);

                    float distanceBetweenTouches = Vector2.Distance(firstTouch.position, secondTouch.position);
                    // Minus and plus some constant so the zoom does not jitter when only moving
                    if (distanceBetweenTouches < lastDistanceBetweenTouches - 9) {
                        float zoomDistance = (Vector2.Distance(Vector2.zero, firstTouch.deltaPosition) + Vector2.Distance(Vector2.zero, secondTouch.deltaPosition)) / 30;
                        if (currentZoomLevel + zoomDistance > SMOOTH_MAX_ZOOM_LEVEL) zoomDistance /= 2;
                        currentZoomLevel += zoomDistance;
                        if (currentZoomLevel > MAX_ZOOM_LEVEL) currentZoomLevel = MAX_ZOOM_LEVEL;
                    } else if (distanceBetweenTouches > lastDistanceBetweenTouches + 9) {
                        currentZoomLevel -= (Vector2.Distance(Vector2.zero, firstTouch.deltaPosition) + Vector2.Distance(Vector2.zero, secondTouch.deltaPosition)) / 30;
                        if (currentZoomLevel < MIN_ZOOM_LEVEL) currentZoomLevel = MIN_ZOOM_LEVEL;
                    }

                    Camera.main.orthographicSize = currentZoomLevel;
                    lastDistanceBetweenTouches = distanceBetweenTouches;

                    Vector3 currentZoomPos = (firstTouch.position + secondTouch.position) / 2;
                    if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began) {
                        ZoomStartPos = currentZoomPos;
                        ZoomStartCameraPos = Camera.main.transform.position;
                    }

                    Vector3 newCameraPosition = ZoomStartCameraPos + (ZoomStartPos - currentZoomPos) / 18;
                    if (BorderManager.LocationInsideCameraArea(newCameraPosition)) {
                        Camera.main.transform.position = newCameraPosition;
                    }

                    // Handle sound
                    MusicManager.SetVolume("sfxVolume", 1.0f - 0.5f * (currentZoomLevel / MAX_ZOOM_LEVEL));
                }
            }
        }
    }

    private bool CheckTapIsInPlayArea(Touch tap) {
        return EventSystem.current.currentSelectedGameObject == null && tap.position.x > Screen.width / 4.9f;
    }
}
