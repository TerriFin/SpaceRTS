using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralGenerator : MonoBehaviour {
    public float timer;
    public int mineralAmount;

    private MineralStorage Storage;

    private void Start() {
        Storage = GetComponent<MineralStorage>();

        StartCoroutine(GiveMinerals());
    }

    private IEnumerator GiveMinerals() {
        while (true) {
            yield return new WaitForSeconds(timer);

            if (Storage != null) {
                Storage.GiveMinerals(mineralAmount);
            }
        }
    }
}
