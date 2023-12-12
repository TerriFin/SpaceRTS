using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionStats : MonoBehaviour {

    private static GameObject DefaultWindow;
    private static GameObject CurrentlySelected;
    private static List<ControlGroupButton> controlGroupButtons;

    private void Start() {
        if (FactionManager.PlayerFaction != null) {
            DefaultWindow = (GameObject)Resources.Load("DefaultWindow");
            CurrentlySelected = Instantiate(DefaultWindow, FindObjectOfType<Canvas>().transform);
            controlGroupButtons = new List<ControlGroupButton>(FindObjectsOfType<ControlGroupButton>());
            foreach (ControlGroupButton CGButton in controlGroupButtons) CGButton.HideIfNoContent();
        }
    }

    public static void ResetSelection() {
        if (SelectionManager.selected.Count != 0) {
            Destroy(CurrentlySelected);
            CurrentlySelected = Instantiate(SelectionManager.selected[0].selectionWindow, FindObjectOfType<Canvas>().transform);
            foreach (ControlGroupButton CGButton in controlGroupButtons) CGButton.transform.parent.gameObject.SetActive(true);
        } else {
            Destroy(CurrentlySelected);
            CurrentlySelected = Instantiate(DefaultWindow, FindObjectOfType<Canvas>().transform);
            foreach (ControlGroupButton CGButton in controlGroupButtons) CGButton.HideIfNoContent();
        }
    }

    public static void SetCurrentlySelected(GameObject current) {
        Destroy(CurrentlySelected);
        CurrentlySelected = current;
    }
}
