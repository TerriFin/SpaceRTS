using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    [System.Serializable]
    public struct SoundEffect {
        public string AUDIO_NAME;
        public AudioClip AUDIO;
        public int AMOUNT;
    }

    public List<SoundEffect> SOUNDS;

    private static Dictionary<string, List<AudioSource>> AUDIO_SOURCES;

    private void Awake() {
        AUDIO_SOURCES = new Dictionary<string, List<AudioSource>>();
        SOUNDS.ForEach((SoundEffect sound) => {
            List<AudioSource> audioSources = new List<AudioSource>();
            for (int i = 0; i < sound.AMOUNT; i++) {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = sound.AUDIO;
                audioSource.spatialBlend = 1;
                audioSource.dopplerLevel = 0.1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.maxDistance = 25;
                audioSources.Add(audioSource);
            }

            AUDIO_SOURCES.Add(sound.AUDIO_NAME, audioSources);
        });
    }

    public static void RequestPlaySound(string sound) {
        try {
            AUDIO_SOURCES[sound].ForEach((AudioSource source) => {
                if (!source.isPlaying) source.Play();
            });
        } catch {
            print("Something went wrong!");
        }
    }
}
