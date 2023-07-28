using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGenerator : MonoBehaviour {

    public int factionMoneyValue;
    public float timer;
    public int FACTION_MONEY_STORAGE;
    public GameObject MONEY_BAR;

    public static int FACTION_MONEY_SCORE_PER_POINT = 9;

    private void Start() {
        if (FACTION_MONEY_STORAGE == 0) FACTION_MONEY_STORAGE = factionMoneyValue * FACTION_MONEY_SCORE_PER_POINT * (int) (timer * 1.5f);
        if (!CompareTag("Untagged")) FactionManager.Factions[tag].maxMoney += FACTION_MONEY_STORAGE;
        StartCoroutine(GiveMoney());
    }

    private void OnDestroy() {
        if (!CompareTag("Untagged") && FactionManager.Factions.ContainsKey(tag)) {
            FactionManager.Factions[tag].maxMoney -= FACTION_MONEY_STORAGE;
            FactionManager.Factions[tag].ReduceMoneyToMax();
        } 
    }

    private IEnumerator GiveMoney() {
        while (true) {
            MoneyBar moneyBar = Instantiate(MONEY_BAR).GetComponent<MoneyBar>();
            float randomedTime = Random.Range(timer - 5f, timer + 5f);
            moneyBar.MONEY_TIME = randomedTime;
            moneyBar.MONEY_GENERATOR = this;
            yield return new WaitForSeconds(randomedTime);

            if (FactionManager.Factions.ContainsKey(tag)) {
                FactionManager.Factions[tag].ModifyMoney((int)(factionMoneyValue * FACTION_MONEY_SCORE_PER_POINT * randomedTime * FactionManager.Factions[tag].aiBonusMultiplier));
            }
        }
    }
}
