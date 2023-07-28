using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour {

    public Color COLOR;

    private SpriteRenderer Renderer;

    private void Start() {
        Renderer = GetComponent<SpriteRenderer>();
        
        if (COLOR.a == 0) {
            string Tag = transform.parent.tag;

            if (Tag == "MainCamera") {
                Color SpriteColor = Color.yellow;
                SpriteColor.a = 0.5f;
                Renderer.color = SpriteColor;
            } else if (Tag == "Asteroid") {
                Color SpriteColor = Color.gray;
                SpriteColor.a = 0.5f;
                Renderer.color = SpriteColor;
            } else {
                Color SpriteColor = FactionManager.Factions[Tag].factionColor;

                int Layer = transform.parent.gameObject.layer;
                if (LayerMask.LayerToName(Layer) != "Building") {
                    SpriteColor.a = 0.75f;
                } else {
                    SpriteColor.a = 1f;
                }

                Renderer.color = SpriteColor;
            }
        } else {
            Renderer.color = COLOR;
        }
    }

    public void ChangeIconColor(Color color) {
        color.a = Renderer.color.a;
        Renderer.color = color;
    }

    public void ResetColorToOriginal() {
        Renderer.color = COLOR;
    }
}
