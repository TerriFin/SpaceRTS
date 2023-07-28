using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICallHelp {
    void CallForHelp(Vector2 location, int enemyAmount, bool important = false);
}
