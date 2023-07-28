using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelBuildWindow : MonoBehaviour {

    private Image CurrentImage;
    private Button CancelButton;

    public void InitializeCancelBuildWindow(GameObject building) {
        CurrentImage = GetComponentInChildren<Image>();
        CancelButton = GetComponentInChildren<Button>();

        CurrentImage.sprite = building.GetComponent<SpriteRenderer>().sprite;
        CancelButton.onClick.AddListener(CancelBuilding);
    }

    private void CancelBuilding() {
        SelectionStats.SetCurrentlySelected(null);
        SelectionStats.ResetSelection();
        BuildingPlacementManager.StopPlacingBuilding();
    }

    private void OnDestroy() {
        CancelBuilding();
    }
}
