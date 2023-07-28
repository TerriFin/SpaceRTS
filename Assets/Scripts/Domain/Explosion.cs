using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Explosion : MonoBehaviour {
    public AudioClip SMALL_EXPLOSION;
    public AudioClip MEDIUM_EXPLOSION;
    public AudioClip BIG_EXPLOSION;

    private Faction faction;
    private SpriteRenderer sprite;
    private float alphaDecayRate;

    private void Start() {
        transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
    }

    public void Explode(string factionName, float size, int damage, Vector2 origin, float givenAlphaDecayRate) {
        if (!FactionManager.Factions.ContainsKey(factionName)) {
            Destroy(gameObject);
            return;
        }

        faction = FactionManager.Factions[factionName];
        sprite = GetComponent<SpriteRenderer>();
        alphaDecayRate = givenAlphaDecayRate;

        transform.localScale = new Vector3(size, size, 1);
        Color factionColor = faction.factionColor;
        factionColor.a = sprite.color.a;
        sprite.color = factionColor;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, size / Mathf.PI);
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ship") || collider.gameObject.layer == LayerMask.NameToLayer("Building") || collider.tag == "Asteroid") {
                if (collider.tag == "Asteroid" || RelationShipManager.AreFactionsInWar(factionName, collider.tag)) {
                    collider.GetComponent<Hitpoints>().TakeDamage(damage, origin, factionName);
                }
            }
        }

        AudioSource Source = GetComponent<AudioSource>();
        if (Source != null) {
            if (size < 3) {
                Source.clip = SMALL_EXPLOSION;
                Source.volume = 0.05f;
            } else if (size < 6) {
                Source.clip = MEDIUM_EXPLOSION;
                Source.volume = 0.5f;
            } else {
                Source.clip = BIG_EXPLOSION;
                Source.volume = 1f;
            }
            Source.pitch = 1.0f + Random.Range(-0.33f, 0.33f);
            Source.Play();
        }

        StartCoroutine(FadeExplosion());
    }

    private IEnumerator FadeExplosion() {
        while (true) {
            yield return new WaitForSeconds(0.1f);
            Color tmp = sprite.color;
            tmp.a -= alphaDecayRate;

            if (tmp.a <= 0.25f) {
                Destroy(gameObject);
            }

            sprite.color = tmp;
        }
    }
}
