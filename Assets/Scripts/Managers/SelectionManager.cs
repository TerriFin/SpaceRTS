using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager {

    public static List<Selectable> selected;
    public static HashSet<Selectable> onScreen;

    public static void Reset() {
        selected = new List<Selectable>();
        onScreen = new HashSet<Selectable>();
    }

    public static void HandleSelection(Selectable selection) {
        if (!selection.Clickable) return;

        if (selected.Count == 0) {
            AddSelection(selection);
        } else {
            if (selected.Contains(selection)) {
                if (selected[0].Equals(selection)) {
                    foreach (Selectable inView in onScreen) {
                        if (!selected.Contains(inView) && selected[0].selectableType == inView.selectableType) {
                            AddSelection(inView);
                        }
                    }
                } else {
                    MakeSelectionFirst(selection);
                }
            } else if (selected[0].selectableType == selection.selectableType || (selected[0].controlable && selection.controlable)) {
                AddSelection(selection);
            } else {
                ClearSelection();
                AddSelection(selection);
            }
        }
    }

    public static void HandleDestruction(Selectable selection) {
        if (selected.Contains(selection)) {
            SelectionStats.SetCurrentlySelected(null);
            HandleDeSelection(selection);
        }
    }

    public static void MakeSelectionFirst(Selectable selection) {
        int index = selected.IndexOf(selection);
        Selectable temp = selected[0];
        selected[0] = selected[index];
        selected[index] = temp;
        ResetFirstSelected();
        SelectionStats.ResetSelection();
    }

    public static void AddSelection(Selectable selection) {
        selected.Add(selection);
        selection.WhenSelected();
        ResetFirstSelected();
        SelectionStats.ResetSelection();
    }

    public static void HandleDeSelection(Selectable selection) {
        selected.Remove(selection);
        selection.WhenDeSelected();
        ResetFirstSelected();
        SelectionStats.ResetSelection();
    }

    public static void ClearSelection() {
        foreach (Selectable currentlySelected in selected) {
            currentlySelected.WhenDeSelected();
        }
        selected.Clear();
        SelectionStats.ResetSelection();
    }

    public static void SelectAllOfType(Selectable.Types type) {
        ClearSelection();
        if (type == Selectable.Types.cruiser) {
            foreach (Selectable inView in onScreen) {
                if (!selected.Contains(inView) && (Selectable.Types.cruiser == inView.selectableType || Selectable.Types.specialShip == inView.selectableType)) {
                    AddSelection(inView);
                }
            }
        } else {
            foreach (Selectable inView in onScreen) {
                if (!selected.Contains(inView) && type == inView.selectableType) {
                    AddSelection(inView);
                }
            }
        }
        SelectionStats.ResetSelection();
    }

    private static void ResetFirstSelected() {
        if (selected.Count != 0) {
            foreach (Selectable s in selected) {
                s.CurrentSelectedIndicator.GetComponent<SelectionIndicator>().IsFirstSelected = false;
            }

            selected[0].CurrentSelectedIndicator.GetComponent<SelectionIndicator>().IsFirstSelected = true;
        }
    }
}
