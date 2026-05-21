using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Event = Spine.Event;
using UnityEngine.Audio;

[System.Serializable]
public class AnimationPair
{
    public EventDataReferenceAsset animationEvent;
    public AudioClip sound;
}

public class AnimationController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] animations;
    public AnimationPair[] animationEventPairs;
    public AudioMixer audioMixer;

    public void PlayAnimation(int minIndex, int maxIndex)
    {
        int index = Random.Range(minIndex, maxIndex + 1);
        AnimationReferenceAsset animation = animations[index];
        skeletonAnimation.state.SetAnimation(0, animation, false);

        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
        AudioMixerGroup bgmMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        tempAudioSource.outputAudioMixerGroup = bgmMixerGroup;
        tempAudioSource.clip = animationEventPairs[index].sound;
        tempAudioSource.Play();

        float animationDuration = skeletonAnimation.state.GetCurrent(0).Animation.Duration;

        StartCoroutine(DestroyAfterSound(tempAudioSource, animationDuration));
        StartCoroutine(DestroyAfterAnimation(animationDuration));
    }

    private IEnumerator DestroyAfterSound(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(audioSource);
    }

    private IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}