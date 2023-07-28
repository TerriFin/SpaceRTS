using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFactionAiBlock {
    void InitializeBlock();
    void ExecuteStep();
}
