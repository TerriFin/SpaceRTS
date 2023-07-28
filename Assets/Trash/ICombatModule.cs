using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatModule {
    void InitializeModule();
    void StartCombatModule();
    void StopCombatModule();
}
