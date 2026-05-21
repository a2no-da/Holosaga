using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DS : MonoBehaviour
{
    public AudioClip dragStartSound;
    public AudioClip dragEndSound;
    public AudioMixer audioMixer;

    public void PlayDragStartSound()
    {
        PlaySoundAndDestroy(dragStartSound);
    }

    public void PlayDragEndSound()
    {
        PlaySoundAndDestroy(dragEndSound);
    }

    private void PlaySoundAndDestroy(AudioClip clip)
    {
        GameObject audioObject = new GameObject("AudioObject");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        AudioMixerGroup bgmMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        audioSource.outputAudioMixerGroup = bgmMixerGroup;

        audioSource.clip = clip;
        audioSource.Play();

        Destroy(audioObject, clip.length);
    }
}
