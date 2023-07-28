using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShipMovement{
    bool AreWeThereYet();
    void SetPrimaryTargetPos(Vector2 target);
    void SetSecondaryTargetPos(Vector2 target);
    Vector2 GetPrimaryTargetPos();
    Vector2 GetSecondaryTargetPos();
    void ClearSecondaryTargetPos();
    void SetOrigin(GameObject origin);
    GameObject GetOrigin();
    void SetOnlyLook(bool look);
}
