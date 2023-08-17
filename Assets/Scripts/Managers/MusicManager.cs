using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {

    [System.Serializable]
    public struct MusicData {
        public string NAME;
        public AudioClip AUDIO_CLIP;
    }

    public List<MusicData> MUSIC_DATAS;
    public AudioMixer MASTER_MIXER;

    public static AudioSource Source;

    private static Dictionary<string, AudioClip> MusicDatas;
    private static AudioMixer MasterMixer;
    private static MusicManager Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            if (!PlayerPrefs.HasKey("musicVolume")) PlayerPrefs.SetFloat("musicVolume", 0.8f);
            if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("sfxVolume", 0.8f);
            MusicDatas = new Dictionary<string, AudioClip>();
            foreach (MusicData musicData in MUSIC_DATAS) {
                MusicDatas.Add(musicData.NAME, musicData.AUDIO_CLIP);
            }

            MasterMixer = MASTER_MIXER;

            Source = GetComponent<AudioSource>();
            if (Source != null) {
                DontDestroyOnLoad(transform.gameObject);
                Instance = this;
            }
        }
    }

    public static void PlayTheme(string themeName) {
        if (MusicDatas != null && MusicDatas.ContainsKey(themeName)) {
            Source.clip = MusicDatas[themeName];
            Source.Play();
        }
    }

    // Set SFX/Music current volume
    public static void SetVolume(string par, float volume) {
        if (MasterMixer == null) return;
        volume = -25.0f * (1.0f - PlayerPrefs.GetFloat(par) * volume);
        if (volume == -25.0f) volume = -80.0f;
        MasterMixer.SetFloat(par, volume);
    }

    // Set playerpref volume
    public static void OptionsSetMusicVolume(float input) {
        PlayerPrefs.SetFloat("musicVolume", input);
        SetVolume("musicVolume", 1.0f);
    }

    public static void OptionsSetSFXVolume(float input) {
        PlayerPrefs.SetFloat("sfxVolume", input);
        SetVolume("sfxVolume", 1.0f);
    }
}
