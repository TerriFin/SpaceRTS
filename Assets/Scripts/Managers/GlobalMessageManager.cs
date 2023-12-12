using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMessageManager : MonoBehaviour {

    public delegate void UpdateNewMessage(string message);
    public static UpdateNewMessage newMessageDelegate;

    public static void Reset() {
        newMessageDelegate = null;
    }

    public static void GlobalMessage(string message) {
        if (newMessageDelegate != null) newMessageDelegate(message);
    }
}
