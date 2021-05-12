using UnityEngine;

public class SoundEffectHandler : MonoBehaviour {
    [SerializeField] private AudioClip[] clips;

    private AudioSource source;

    void Start() {
        source = GetComponent<AudioSource>();
    }

    public void SetAndPlaySoundClip(AudioClip clip) {
        source.clip = clip;
        source.Play();
    }

    public void StopPlaying() {
        source.Stop();
    }
}