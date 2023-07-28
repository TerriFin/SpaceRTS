using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSelectionButton : MonoBehaviour {
    public Selectable.Types type;
    
    private void Start() {
        SpriteRenderer SelectionObjectSpriteRenderer = null;
        if (type == Selectable.Types.frigate) {
            SelectionObjectSpriteRenderer = FactionManager.PlayerFaction.frigate.GetComponent<SpriteRenderer>();
        } else if (type == Selectable.Types.cruiser) {
            SelectionObjectSpriteRenderer = FactionManager.PlayerFaction.cruiser.GetComponent<SpriteRenderer>();
        }

        Button button = GetComponent<Button>();
        Vector2 newSpriteSize = SelectionObjectSpriteRenderer.size;
        print(newSpriteSize);
        if (newSpriteSize.x > 1) newSpriteSize.x = 1;
        if (newSpriteSize.x < 0.66f) newSpriteSize.x = 0.66f;
        if (newSpriteSize.y > 1) newSpriteSize.y = 1;
        if (newSpriteSize.y < 0.66f) newSpriteSize.y = 0.66f;
        button.image.rectTransform.sizeDelta = new Vector2(150, 150) * newSpriteSize;
        button.image.sprite = SelectionObjectSpriteRenderer.sprite;
        button.onClick.AddListener(() => SelectionManager.SelectAllOfType(type));
    }
}
