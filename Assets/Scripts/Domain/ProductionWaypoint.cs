using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionWaypoint : MonoBehaviour {

    public GameObject waypointMarker;

    public bool WaypointSet { get; private set; }
    public Vector2 CurrentWaypoint { get; private set; }

    private GameObject CurrentWaypointMarker;

    private void Start() {
        ResetWaypoint();
        CurrentWaypointMarker = null;
    }

    public void SetWaypoint(Vector2 target) {
        WaypointSet = true;
        CurrentWaypoint = target;

        if (CurrentWaypointMarker != null) {
            Destroy(CurrentWaypointMarker);
        }

        CurrentWaypointMarker = Instantiate(waypointMarker, transform);
        CurrentWaypointMarker.transform.position = CurrentWaypoint;
    }

    public void ResetWaypoint() {
        WaypointSet = false;
        CurrentWaypoint = Vector2.zero;

        if (CurrentWaypointMarker != null) {
            Destroy(CurrentWaypointMarker);
        }
    }
}
