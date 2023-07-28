using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBorderManager : MonoBehaviour {
    public float CLEAR_OBJECTS_TIMER;
    public float CurrentSize;

    private void Awake() {
        InitializeMap(CurrentSize);
    }

    public void InitializeMap(float size) {
        CurrentSize = size;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = true;
        float spriteSize = renderer.sprite.bounds.size.x;
        transform.localScale = new Vector2(size * 2.308f / spriteSize, size * 2.308f / spriteSize);
        StartCoroutine(StartClearObjectsTimer());

        FindObjectOfType<Minimap>().InitializeMinimap(this);
    }

    private IEnumerator StartClearObjectsTimer() {
        while (true) {
            yield return new WaitForSeconds(CLEAR_OBJECTS_TIMER);
            foreach (GameObject gameObject in FindObjectsOfType<GameObject>()) {
                if (Vector2.Distance(Vector2.zero, gameObject.transform.position) > CurrentSize * 1.5f) {
                    Hitpoints hitpoints = gameObject.GetComponent<Hitpoints>();
                    if (hitpoints != null && !hitpoints.IGNORES_STAGE_BORDERS) hitpoints.TakeDamage(hitpoints.maxHp * 2, gameObject.transform.position, gameObject.tag);
                }
            }
        }
    }

    public bool LocationInsideCameraSoftArea(Vector2 location) {
        return Vector2.Distance(Vector2.zero, location) < CurrentSize - 10;
    }

    public bool LocationInsideCameraArea(Vector2 location) {
        return Vector2.Distance(Vector2.zero, location) < CurrentSize - 2.5f;
    }

    public bool LocationInsideBuildArea(Vector2 location) {
        return Vector2.Distance(Vector2.zero, location) < CurrentSize;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, CurrentSize - 5);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CurrentSize * 1.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CurrentSize);
    }
}
