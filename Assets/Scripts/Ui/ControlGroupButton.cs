using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlGroupButton : MonoBehaviour {
    public Sprite DefaultSprite;

    private List<Selectable> content;
    private Button button;

    private void Start() {
        content = new List<Selectable>();
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    public void RemoveSelectable(Selectable selectable) {
        content.Remove(selectable);
        UpdateSprite();
    }

    private void HandleClick() {
        // Handle new control group content
        if (SelectionManager.selected.Count > 0) {
            foreach (Selectable selectable in content) {
                selectable.AssignedControlGroupButton = null;
            }
            content.Clear();
            foreach (Selectable selectable in SelectionManager.selected) {
                content.Add(selectable);
                selectable.AssignedControlGroupButton = this;
            }

            UpdateSprite();
            // Handle selecting control group content
        } else {
            SelectionManager.ClearSelection();
            foreach (Selectable selectable in content) {
                SelectionManager.AddSelection(selectable);
            }
        }
    }

    private void UpdateSprite() {
        Sprite updatedSprite = DefaultSprite;
        if (content.Count > 0) {
            updatedSprite = content[0].GetComponent<SpriteRenderer>().sprite;
        }

        Vector2 newSpriteSize = updatedSprite.bounds.size;
        if (newSpriteSize.x > 1) newSpriteSize.x = 1;
        if (newSpriteSize.x < 0.66f) newSpriteSize.x = 0.66f;
        if (newSpriteSize.y > 1) newSpriteSize.y = 1;
        if (newSpriteSize.y < 0.66f) newSpriteSize.y = 0.66f;
        button.image.rectTransform.sizeDelta = new Vector2(100, 100) * newSpriteSize;
        button.image.sprite = updatedSprite;
    }
}
