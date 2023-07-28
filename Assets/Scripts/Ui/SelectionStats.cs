using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionStats : MonoBehaviour {

    private static GameObject DefaultWindow;
    private static GameObject CurrentlySelected;

    private void Start() {
        if (FactionManager.PlayerFaction != null) {
            DefaultWindow = (GameObject)Resources.Load("DefaultWindow");
            CurrentlySelected = Instantiate(DefaultWindow, FindObjectOfType<Canvas>().transform);
        }
    }

    public static void ResetSelection() {
        if (SelectionManager.selected.Count != 0) {
            Destroy(CurrentlySelected);
            CurrentlySelected = Instantiate(SelectionManager.selected[0].selectionWindow, FindObjectOfType<Canvas>().transform);
        } else {
            Destroy(CurrentlySelected);
            CurrentlySelected = Instantiate(DefaultWindow, FindObjectOfType<Canvas>().transform);
        }
    }

    public static void SetCurrentlySelected(GameObject current) {
        Destroy(CurrentlySelected);
        CurrentlySelected = current;
    }
}
