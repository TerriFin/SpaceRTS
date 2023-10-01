using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkirmishMenuDropdown : MonoBehaviour {

    public bool FACTION_TYPE_DROPDOWN;
    public bool FACTION_STARTING_RESOURCES_DROPDOWN;
    public bool MAP_GENERATION_SIZE_DROPDOWN;
    public bool MAP_GENERATION_ASTEROIDS_DROPDOWN;
    public bool MAP_GENERATION_PLANETS_DROPDOWN;
    public bool MAP_GENERATION_TOTAL_WAR_TIMER_DROPDOWN;
    public bool MAP_GENERATION_STARTING_SHIPS_AMOUNT_DROPDOWN;
    public bool MAP_GENERATION_MIRRORED_TOGGLE;

    public List<TMP_Dropdown> OTHER_DROPDOWNS;

    private MapGeneratorManager MAP_MANAGER;

    private void Awake() {
        MAP_MANAGER = FindObjectOfType<MapGeneratorManager>();
    }

    private void Start() {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        if (FACTION_TYPE_DROPDOWN) dropdown.value = MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].statusIndex;
        else if (FACTION_STARTING_RESOURCES_DROPDOWN) dropdown.value = MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].startingResourcesIndex;
        else if (MAP_GENERATION_SIZE_DROPDOWN) dropdown.value = MAP_MANAGER.MAP_SIZE_LAST_INDEX;
        else if (MAP_GENERATION_ASTEROIDS_DROPDOWN) dropdown.value = MAP_MANAGER.MAP_ASTEROIDS_LAST_INDEX;
        else if (MAP_GENERATION_PLANETS_DROPDOWN) dropdown.value = MAP_MANAGER.MAP_PLANETS_LAST_INDEX;
        else if (MAP_GENERATION_TOTAL_WAR_TIMER_DROPDOWN) dropdown.value = MAP_MANAGER.TOTAL_WAR_TIMER_LAST_INDEX;
        else if (MAP_GENERATION_STARTING_SHIPS_AMOUNT_DROPDOWN) dropdown.value = MAP_MANAGER.STARTING_SHIPS_AMOUNT_INDEX;
        else if (MAP_GENERATION_MIRRORED_TOGGLE) GetComponent<Toggle>().isOn = MAP_MANAGER.MIRRORED;
    }

    public void UpdateFactionStatus(int index) {
        MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag] = new MapGeneratorManager.FactionData(index, MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].startingResourcesIndex);
        print(MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].statusIndex + ", " + MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].startingResourcesIndex);
    }

    public void ResetPlayerStatus() {
        if (MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].statusIndex == 1) {
            foreach (TMP_Dropdown dropdown in OTHER_DROPDOWNS) {
                if (MAP_MANAGER.FactionDatas[dropdown.gameObject.transform.parent.tag].statusIndex == 1) {
                    dropdown.value = 3;
                    MAP_MANAGER.FactionDatas[dropdown.gameObject.transform.parent.tag] = new MapGeneratorManager.FactionData(3, MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].startingResourcesIndex);
                }
            }
        }
    }

    public void ResetDisabledStatus() {
        if (MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].statusIndex == 0) {
            List<TMP_Dropdown> disabledOtherFactions = new List<TMP_Dropdown>();
            foreach (TMP_Dropdown dropdown in OTHER_DROPDOWNS) {
                if (MAP_MANAGER.FactionDatas[dropdown.gameObject.transform.parent.tag].statusIndex == 0) disabledOtherFactions.Add(dropdown);
            }

            if (OTHER_DROPDOWNS.Count - disabledOtherFactions.Count <= 1) {
                TMP_Dropdown randomDropdown = disabledOtherFactions[Random.Range(0, disabledOtherFactions.Count)];
                randomDropdown.value = 3;
                MAP_MANAGER.FactionDatas[randomDropdown.gameObject.transform.parent.tag] = new MapGeneratorManager.FactionData(3, MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].startingResourcesIndex);
            }
        }
    }

    public void UpdateFactionStartingResources(int index) {
        MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag] = new MapGeneratorManager.FactionData(MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].statusIndex, index);
        print(MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].statusIndex + ", " + MAP_MANAGER.FactionDatas[gameObject.transform.parent.tag].startingResourcesIndex);
    }

    public void UpdateMapSize(int index) {
        switch (index) {
            case 0:
                MAP_MANAGER.MAP_SIZE = 25;
                MAP_MANAGER.CENTER_DEAD_ZONE = 0.33f;
                MAP_MANAGER.CENTER_ASTEROID_FIELD_CHANCE = 0.33f;
                MAP_MANAGER.CENTER_PLANET_CHANCE = 0.33f;
                break;
            case 1:
                MAP_MANAGER.MAP_SIZE = 40;
                MAP_MANAGER.CENTER_DEAD_ZONE = 0.26f;
                MAP_MANAGER.CENTER_ASTEROID_FIELD_CHANCE = 0.5f;
                MAP_MANAGER.CENTER_PLANET_CHANCE = 0.5f;
                break;
            case 2:
                MAP_MANAGER.MAP_SIZE = 50;
                MAP_MANAGER.CENTER_DEAD_ZONE = 0.2f;
                MAP_MANAGER.CENTER_ASTEROID_FIELD_CHANCE = 0.66f;
                MAP_MANAGER.CENTER_PLANET_CHANCE = 0.66f;
                break;
            case 3:
                MAP_MANAGER.MAP_SIZE = 65;
                MAP_MANAGER.CENTER_DEAD_ZONE = 0.15f;
                MAP_MANAGER.CENTER_ASTEROID_FIELD_CHANCE = 0.75f;
                MAP_MANAGER.CENTER_PLANET_CHANCE = 0.75f;
                break;
            case 4:
                MAP_MANAGER.MAP_SIZE = 80;
                MAP_MANAGER.CENTER_DEAD_ZONE = 0.1f;
                MAP_MANAGER.CENTER_ASTEROID_FIELD_CHANCE = 0.9f;
                MAP_MANAGER.CENTER_PLANET_CHANCE = 0.9f;
                break;
        }

        MAP_MANAGER.MAP_SIZE_LAST_INDEX = index;
        print(MAP_MANAGER.MAP_SIZE);
    }

    public void UpdateAsteroidsAmount(int index) {
        switch (index) {
            case 0:
                MAP_MANAGER.DESIRED_ASTEROID_FIELDS_AMOUNT = 5;
                MAP_MANAGER.SMALL_ASTEROID_FIELD_CHANCE = 0.33f;
                MAP_MANAGER.MEDIUM_ASTEROID_FIELD_CHANCE = 0.85f;
                break;
            case 1:
                MAP_MANAGER.DESIRED_ASTEROID_FIELDS_AMOUNT = 9;
                MAP_MANAGER.SMALL_ASTEROID_FIELD_CHANCE = 0.3f;
                MAP_MANAGER.MEDIUM_ASTEROID_FIELD_CHANCE = 0.8f;
                break;
            case 2:
                MAP_MANAGER.DESIRED_ASTEROID_FIELDS_AMOUNT = 16;
                MAP_MANAGER.SMALL_ASTEROID_FIELD_CHANCE = 0.25f;
                MAP_MANAGER.MEDIUM_ASTEROID_FIELD_CHANCE = 0.75f;
                break;
            case 3:
                MAP_MANAGER.DESIRED_ASTEROID_FIELDS_AMOUNT = 30;
                MAP_MANAGER.SMALL_ASTEROID_FIELD_CHANCE = 0.2f;
                MAP_MANAGER.MEDIUM_ASTEROID_FIELD_CHANCE = 0.7f;
                break;
            case 4:
                MAP_MANAGER.DESIRED_ASTEROID_FIELDS_AMOUNT = 100;
                MAP_MANAGER.SMALL_ASTEROID_FIELD_CHANCE = 0.2f;
                MAP_MANAGER.MEDIUM_ASTEROID_FIELD_CHANCE = 0.65f;
                break;
        }

        MAP_MANAGER.MAP_ASTEROIDS_LAST_INDEX = index;
        print(MAP_MANAGER.DESIRED_ASTEROID_FIELDS_AMOUNT);
    }

    public void UpdatePlanetsAmount(int index) {
        switch (index) {
            case 0:
                MAP_MANAGER.DESIRED_PLANET_FACTION_SCORE = 30;
                break;
            case 1:
                MAP_MANAGER.DESIRED_PLANET_FACTION_SCORE = 60;
                break;
            case 2:
                MAP_MANAGER.DESIRED_PLANET_FACTION_SCORE = 90;
                break;
            case 3:
                MAP_MANAGER.DESIRED_PLANET_FACTION_SCORE = 150;
                break;
            case 4:
                MAP_MANAGER.DESIRED_PLANET_FACTION_SCORE = 300;
                break;
        }

        MAP_MANAGER.MAP_PLANETS_LAST_INDEX = index;
        print(MAP_MANAGER.DESIRED_PLANET_FACTION_SCORE);
    }

    public void UpdateTotalWarTimer(int index) {
        switch (index) {
            case 0:
                MAP_MANAGER.TOTAL_WAR_TIMER = 0;
                break;
            case 1:
                MAP_MANAGER.TOTAL_WAR_TIMER = 1;
                break;
            case 2:
                MAP_MANAGER.TOTAL_WAR_TIMER = 90;
                break;
            case 3:
                MAP_MANAGER.TOTAL_WAR_TIMER = 300;
                break;
            case 4:
                MAP_MANAGER.TOTAL_WAR_TIMER = 600;
                break;
            case 5:
                MAP_MANAGER.TOTAL_WAR_TIMER = 900;
                break;
        }

        MAP_MANAGER.TOTAL_WAR_TIMER_LAST_INDEX = index;
        print(MAP_MANAGER.TOTAL_WAR_TIMER);
    }

    public void UpdateStartingShipsAmounts(int index) {
        MAP_MANAGER.STARTING_SHIPS_AMOUNT_INDEX = index;
        print(MAP_MANAGER.STARTING_SHIPS_AMOUNT_INDEX);
    }

    public void ToggleMirrored(bool toggle) {
        MAP_MANAGER.MIRRORED = toggle;
    }
}
