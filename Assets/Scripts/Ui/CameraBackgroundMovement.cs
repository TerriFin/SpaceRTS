using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundMovement : MonoBehaviour {
    public float MOVEMENT_PERCENTAGE;
    private Transform ParentTransform;
    private float OriginalPositionZ;

    private void Start() {
        ParentTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        OriginalPositionZ = transform.position.z;
    }

    private void Update() {
        Vector3 newPos = ParentTransform.position * MOVEMENT_PERCENTAGE;
        newPos.z = OriginalPositionZ;
        transform.position = newPos;
    }
}
