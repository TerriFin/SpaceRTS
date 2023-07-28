using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour {

    public bool IsFirstSelected;
    public GameObject SelectedObject { get; set; }

    private void Update() {
        if (IsFirstSelected) {
            transform.Rotate(0, 0, 0.8f);
        }
        transform.position = SelectedObject.transform.position;
    }
}
