using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralStorage : MonoBehaviour {

    public GameObject mineralStorageBar;
    public int currentMineralStorage;
    public int maxMineralStorage;

    private GameObject CurrentMineralStorageBar;

    private void Start() {
        if (mineralStorageBar != null && currentMineralStorage != 0) {
            ShowOrExtendStorageBar();
        }
    }

    private void OnDestroy() {
        if (CurrentMineralStorageBar != null) {
            Destroy(CurrentMineralStorageBar);
        }
    }

    public int FreeStorage() {
        return maxMineralStorage - currentMineralStorage;
    }

    public int GiveMinerals(int amount) {
        if (currentMineralStorage < maxMineralStorage) {
            int toReturn;
            if (currentMineralStorage + amount > maxMineralStorage) {
                toReturn = currentMineralStorage + amount - maxMineralStorage;
                currentMineralStorage = maxMineralStorage;
            } else {
                toReturn = 0;
                currentMineralStorage += amount;
            }

            ShowOrExtendStorageBar();
            return toReturn;
        }

        ShowOrExtendStorageBar();
        return amount;
    }

    public int TakeMinerals(int amount) {
        ShowOrExtendStorageBar();
        int toReturn;
        if (amount > currentMineralStorage) {
            toReturn = currentMineralStorage;
            currentMineralStorage = 0;
        } else {
            toReturn = amount;
            currentMineralStorage -= amount;
        }

        ShowOrExtendStorageBar();
        return toReturn;
    }

    public void ShowOrExtendStorageBar() {
        if (CurrentMineralStorageBar == null && mineralStorageBar != null) {
            CurrentMineralStorageBar = Instantiate(mineralStorageBar);
            CurrentMineralStorageBar.GetComponent<MineralStorageBar>().Storage = this;
        } else {
            CurrentMineralStorageBar.GetComponent<MineralStorageBar>().ResetTimer();
        }
    }
}
