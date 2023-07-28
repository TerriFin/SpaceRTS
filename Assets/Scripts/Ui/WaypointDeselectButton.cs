using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointDeselectButton : MonoBehaviour {

    private ProductionWaypoint attachedProductionWaypoint;

    private void Start() {
        attachedProductionWaypoint = SelectionManager.selected[0].GetComponent<ProductionWaypoint>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
        if (attachedProductionWaypoint != null) {
            button.interactable = true;
        }
    }

    private void HandleClick() {
        foreach (Selectable selected in SelectionManager.selected) {
            ProductionWaypoint waypoint = selected.GetComponent<ProductionWaypoint>();
            waypoint.ResetWaypoint();
        }
    }
}
