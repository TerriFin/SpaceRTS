using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public int damage;
    public float timeExisting;
    public float projectileSpeed;
    public Vector2 origin;
    public bool MINING_PROJECTILE;

    public MineralStorage AttachedShipStorage { get; set; }

    private void Start() {
        origin = (Vector2)transform.position;
        StartCoroutine(StartProjectileDecay());
    }

    private void Update() {
        transform.position = transform.position + transform.up * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Asteroid" || collision.gameObject.layer == LayerMask.NameToLayer("Ship") || collision.gameObject.layer == LayerMask.NameToLayer("Building")) {
            if (collision.tag == "Asteroid") {
                Hitpoints hitTarget = collision.GetComponent<Hitpoints>();
                hitTarget.TakeDamage(damage, origin, tag);
                if (AttachedShipStorage != null && MINING_PROJECTILE) {
                    AttachedShipStorage.GiveMinerals((int) (damage / 10));
                    float factionAiModifier = FactionManager.Factions[tag].aiBonusMultiplier - 1;
                    if (factionAiModifier > 0 && Random.Range(0.0f, 1.0f) < factionAiModifier) {
                        AttachedShipStorage.GiveMinerals(5);
                    } else if (factionAiModifier < 0 && Random.Range(-1.0f, 0.0f) > factionAiModifier) {
                        AttachedShipStorage.TakeMinerals(1);
                    }
                }
                Destroy(gameObject);
            } else if (!MINING_PROJECTILE && RelationShipManager.AreFactionsInWar(tag, collision.tag)) {
                Hitpoints hitTarget = collision.GetComponent<Hitpoints>();
                hitTarget.TakeDamage(damage, origin, tag);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator StartProjectileDecay() {
        yield return new WaitForSeconds(timeExisting);
        Destroy(gameObject);
    }
}
