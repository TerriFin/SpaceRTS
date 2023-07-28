using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitpointsBar : MonoBehaviour {

    public GameObject bar;
    public float timeToShow;
    public SpriteRenderer background;
    public SpriteRenderer barSprite;
    public Hitpoints Hitpoints { get; set; }

    private bool CanDisappear;

    private void Start() {
        transform.position = Hitpoints.transform.position + transform.up;
        CanDisappear = true;
        StartCoroutine(StartDisappear());
    }

    private void Update() {
        if (Hitpoints != null) {
            bar.transform.localScale = new Vector3((float)Hitpoints.CurrentHp / (float)Hitpoints.maxHp, 1, 1);
            transform.position = Hitpoints.transform.position + transform.up;
        }
    }

    private IEnumerator StartDisappear() {
        while (true) {
            yield return new WaitForSeconds(timeToShow);
            if (CanDisappear) {
                Destroy(gameObject);
            }

            CanDisappear = true;
        }
    }

    public void ResetTimer() {
        CanDisappear = false;
    }

    public void SetAsBuilding() {
        background.sortingOrder += 2;
        barSprite.sortingOrder += 2;
    }
}
