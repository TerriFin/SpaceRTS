using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuVolumeSlider : MonoBehaviour {
    public bool MUSIC;
    public bool SFX;
    private void Awake() {
        Slider slider = GetComponent<Slider>();
        if (MUSIC) {
            slider.onValueChanged.AddListener((float value) => MusicManager.OptionsSetMusicVolume(value));
            slider.value = PlayerPrefs.GetFloat("musicVolume");
        }

        if (SFX) {
            slider.onValueChanged.AddListener((float value) => MusicManager.OptionsSetSFXVolume(value));
            slider.value = PlayerPrefs.GetFloat("sfxVolume");
        }
    }
}
