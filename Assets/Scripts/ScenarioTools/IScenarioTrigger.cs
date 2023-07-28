using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IScenarioTrigger {
    public bool DoNotActivate { get; set; }
    bool Triggered();
}
