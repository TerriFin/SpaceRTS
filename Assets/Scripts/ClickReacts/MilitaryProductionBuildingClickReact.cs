using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryProductionBuildingClickReact : MonoBehaviour, IReactToClick {

    private ProductionWaypoint Waypoint;

    private void Start() {
        Waypoint = GetComponent<ProductionWaypoint>();
    }

    public void ReactToClick(Vector2 targetPos) {
        Waypoint.SetWaypoint(targetPos);
    }
}
