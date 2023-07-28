using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactionBlockWithSkips : MonoBehaviour, IFactionAiBlock {

    public bool WORKS_WITHOUT_BASE;
    public int minSkips;
    public int maxSkips;

    private int CurrentSkips;

    public void InitializeBlock() {
        Initialize();
        CurrentSkips = Random.Range(minSkips, maxSkips);
    }

    public void ExecuteStep() {
        if (CurrentSkips == 0) {
            if (WORKS_WITHOUT_BASE || BuildingManager.Buildings[tag].Count > 0) Block();
            CurrentSkips = Random.Range(minSkips, maxSkips);
        } else {
            CurrentSkips--;
        }
    }

    public abstract void Initialize();

    public abstract void Block();
}
