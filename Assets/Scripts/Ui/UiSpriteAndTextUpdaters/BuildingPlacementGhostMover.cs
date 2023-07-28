using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementGhostMover : MonoBehaviour {
    private void Update() {
        if (gameObject.activeSelf) {
            Vector3 pointedCoordinates = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.touchCount > 0) {
                pointedCoordinates = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }

            pointedCoordinates.z = 0;

            gameObject.transform.position = pointedCoordinates;
        }
    }
}
