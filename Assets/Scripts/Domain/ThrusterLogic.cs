using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterLogic : MonoBehaviour {

    private SpriteRenderer Renderer;
    private ShipMovement Movement;
    private AudioSource Source;

    private Color OnColor;
    private Color OffColor;

    private void Start() {
        Renderer = GetComponent<SpriteRenderer>();
        Movement = GetComponentInParent<ShipMovement>();
        Source = GetComponent<AudioSource>();

        OnColor = FactionManager.Factions[transform.parent.tag].factionColor;
        OnColor.a = 0.65f;
        OffColor = FactionManager.Factions[transform.parent.tag].factionColor;

        Renderer.color = OffColor;
    }

    private void Update() {
        if (Movement.ThrusterOn()) {
            Renderer.color = OnColor;
            if (!Source.isPlaying) Source.Play();
        } else {
            Renderer.color = OffColor;
            if (Source.isPlaying) Source.Pause();
        }
    }
}
