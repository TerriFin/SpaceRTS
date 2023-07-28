using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public void InitializeMinimap(LevelBorderManager BorderManager) {
        Camera MinimapCam = GetComponent<Camera>();
        MinimapCam.orthographicSize = BorderManager.CurrentSize;
    }
}
