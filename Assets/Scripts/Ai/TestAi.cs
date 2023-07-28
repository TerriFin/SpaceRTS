using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAi : MonoBehaviour, IAi {

    private ShipMovement Controls;

    public void InitializeAi() {
        Controls = GetComponent<ShipMovement>();
    }

    public void ExecuteStep() {
        Controls.SetSecondaryTargetPos(new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)));
    }
}
