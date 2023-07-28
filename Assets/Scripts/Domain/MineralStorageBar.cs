using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralStorageBar : MonoBehaviour {

    public GameObject bar;
    public float timeToShow;
    public MineralStorage Storage { get; set; }

    private bool CanDisappear;

    private void Start() {
        if (Storage.currentMineralStorage < Storage.maxMineralStorage) {
            bar.transform.localScale = new Vector3(1, (float)Storage.currentMineralStorage / (float)Storage.maxMineralStorage, 1);
        } else {
            bar.transform.localScale = new Vector3(1, 1, 1);
        }
        transform.position = Storage.transform.position + (transform.right / 1.1f);
        CanDisappear = true;
        StartCoroutine(StartDisappear());
    }

    private void Update() {
        transform.position = Storage.transform.position + (transform.right / 1.1f);
    }

    private IEnumerator StartDisappear() {
        while (true) {
            yield return new WaitForSeconds(timeToShow);
            if (Storage.currentMineralStorage == 0 && CanDisappear) {
                Destroy(gameObject);
            }

            CanDisappear = true;
        }
    }

    public void ResetTimer() {
        if (Storage.FreeStorage() >= 0) {
            bar.transform.localScale = new Vector3(1, (float)Storage.currentMineralStorage / (float)Storage.maxMineralStorage, 1);
        } else {
            bar.transform.localScale = new Vector3(1, 1, 1);
        }
        CanDisappear = false;
    }
}
