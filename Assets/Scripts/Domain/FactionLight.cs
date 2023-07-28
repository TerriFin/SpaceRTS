using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionLight : MonoBehaviour {

    public Color defaultColor;

    private SpriteRenderer sprite;

    private void Start() {
        if (sprite == null) {
            sprite = GetComponent<SpriteRenderer>();
            ChangeColor(defaultColor);
        }
    }

    public void ChangeColor(Color color) {
        if (sprite == null) {
            sprite = GetComponent<SpriteRenderer>();
        }

        color.a = 0.5f;
        sprite.color = color;
    }
}
