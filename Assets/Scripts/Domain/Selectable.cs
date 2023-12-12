using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {
    public enum Types {
        commandCenter,
        mine,
        frigateStation,
        cruiserStation,
        patrolStation,
        raiderStation,
        defenceStation,
        smallCargo,
        mediumCargo,
        bigCargo,
        buildingShip,
        miner,
        frigate,
        cruiser,
        police,
        raider,
        fighter,
        specialBuilding,
        specialShip,
        nothing,
    }

    public Types selectableType = Types.commandCenter;
    public bool Clickable;
    public bool controlable;
    public GameObject selectedIndicator;
    public GameObject selectionWindow;

    public Sprite Sprite { get; private set; }
    public Hitpoints AttachedHitpoints { get; private set; }
    public MineralStorage AttachedMineralStorage { get; private set; }
    public GameObject CurrentSelectedIndicator { get; private set; }
    public List<ControlGroupButton> AssignedControlGroupButtons { get; set; }
    private IReactToClick clickHandler;

    private void Start() {
        Sprite = GetComponent<SpriteRenderer>().sprite;
        AttachedHitpoints = GetComponent<Hitpoints>();
        AttachedMineralStorage = GetComponent<MineralStorage>();

        CurrentSelectedIndicator = null;
        AssignedControlGroupButtons = new List<ControlGroupButton>();
        clickHandler = gameObject.GetComponent<IReactToClick>();
    }

    private void OnDestroy() {
        if (AssignedControlGroupButtons.Count > 0) foreach (ControlGroupButton CGButton in AssignedControlGroupButtons) CGButton.RemoveSelectable(this);
    }

    public void WhenSelected() {
        CurrentSelectedIndicator = Instantiate(selectedIndicator);
        CurrentSelectedIndicator.GetComponent<SelectionIndicator>().SelectedObject = gameObject;

        Rect rect = Sprite.rect;
        if (rect.height >= 190) {
            CurrentSelectedIndicator.transform.localScale = new Vector3(3, 3, 1);
        } else if (rect.height >= 160) {
            CurrentSelectedIndicator.transform.localScale = new Vector3(2, 2, 1);
        }
    }

    public void WhenDeSelected() {
        Destroy(CurrentSelectedIndicator);
    }

    public void ReactToClick(Vector2 targetPos) {
        if (clickHandler != null) {
            clickHandler.ReactToClick(targetPos);
        }
    }

    private void OnBecameVisible() {
        if (FactionManager.PlayerFaction != null && FactionManager.PlayerFaction.factionTag.Equals(gameObject.tag)) {
            SelectionManager.onScreen.Add(this);
        }
    }

    private void OnBecameInvisible() {
        if (FactionManager.PlayerFaction != null && FactionManager.PlayerFaction.factionTag.Equals(gameObject.tag)) {
            SelectionManager.onScreen.Remove(this);
        }
    }
}
