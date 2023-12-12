using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionAiBase : MonoBehaviour {

    public bool ON;
    public float aiDecisionTimer;
    public int callForHelpCacheSize;

    public Vector2 FactionCenterPoint { get; private set; }

    private List<IFactionAiBlock> AiBlocks;
    [SerializeField]
    private ICallHelp[] CallHelpHandlers;
    private Vector2[] CallForHelpLocations;
    private int HelpLocationIndex;

    private void Start() {
        AiBlocks = new List<IFactionAiBlock>();
        CallHelpHandlers = GetComponents<ICallHelp>();
        CallForHelpLocations = new Vector2[callForHelpCacheSize];
        HelpLocationIndex = 0;

        for (int i = 0; i < callForHelpCacheSize; i++) {
            CallForHelpLocations[i] = Vector2.zero;
        }

        foreach (IFactionAiBlock aiBlock in GetComponents<IFactionAiBlock>()) {
            aiBlock.InitializeBlock();
            AiBlocks.Add(aiBlock);
        }

        if (ON) FactionCenterPoint = BuildingManager.GetFactionCenterPoint(tag);

        StartCoroutine(UpdateAiDecisions());
    }

    public void CallHelp(Vector2 location, int enemyAmount, bool important = false) {
        CallForHelpLocations[HelpLocationIndex] = location;
        HelpLocationIndex++;

        if (HelpLocationIndex >= callForHelpCacheSize) {
            HelpLocationIndex = 0;
        }

        if (CallHelpHandlers.Length != 0) {
            foreach (ICallHelp callHelpHandler in CallHelpHandlers) {
                callHelpHandler.CallForHelp(location, enemyAmount, important);
            }
        }
    }

    public Vector2 GetLocationThatCalledHelp() {
        if (CallForHelpLocations[Random.Range(0, callForHelpCacheSize)] == Vector2.zero) return BuildingManager.GetFactionCenterPoint(tag);
        else return CallForHelpLocations[Random.Range(0, callForHelpCacheSize)];
    }

    public Vector2 GetLocationThatCalledHelpAndRemoveIt() {
        int locationIndex = Random.Range(0, callForHelpCacheSize);
        Vector2 location = CallForHelpLocations[locationIndex];
        CallForHelpLocations[locationIndex] = Vector2.zero;
        return location;
    }

    private IEnumerator UpdateAiDecisions() {
        while (true) {
            yield return new WaitForSeconds(aiDecisionTimer);
            if (ON) {
                FactionCenterPoint = BuildingManager.GetFactionCenterPoint(tag);
                foreach (IFactionAiBlock aiBlock in AiBlocks) {
                    aiBlock.ExecuteStep();
                }
            }
        }
    }
}
