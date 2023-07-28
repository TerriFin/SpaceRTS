using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageToWindow : MonoBehaviour {

    private Image imageToShow;

    private void Start() {
        imageToShow = GetComponentInChildren<Image>();
        Sprite sprite = SelectionManager.selected[0].Sprite;

        Vector2 newSpriteSize = sprite.bounds.size;
        if (newSpriteSize.x > 1) newSpriteSize.x = 1;
        if (newSpriteSize.x < 0.66f) newSpriteSize.x = 0.66f;
        if (newSpriteSize.y > 1) newSpriteSize.y = 1;
        if (newSpriteSize.y < 0.66f) newSpriteSize.y = 0.66f;

        imageToShow.rectTransform.sizeDelta = new Vector2(300, 300) * newSpriteSize;
        imageToShow.sprite = sprite;
    }
}
