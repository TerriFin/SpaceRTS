using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAreaColorIndicator : MonoBehaviour {

    private SpriteRenderer Renderer;
    private Collider2D Collider;
    private int HowManyHits;

    private void Start() {
        Renderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();
        HowManyHits = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Building")) {
            HowManyHits++;
            Renderer.color = Color.red;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Building")) {
            HowManyHits--;
            if (HowManyHits == 0) {
                Renderer.color = Color.green;
            }
        }
    }
}
