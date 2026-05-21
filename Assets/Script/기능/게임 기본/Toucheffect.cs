using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Toucheffect : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public string[] animationNames;

    public AudioClip[] sounds;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        int animationIndex = Random.Range(0, animationNames.Length);
        skeletonAnimation.AnimationState.SetAnimation(0, animationNames[animationIndex], false);

        int soundIndex = Random.Range(0, sounds.Length);
        audioSource.clip = sounds[soundIndex];
        audioSource.Play();

        skeletonAnimation.AnimationState.Complete += OnAnimationComplete;
    }

    private void Update()
    {
        skeletonAnimation.Update(Time.unscaledDeltaTime);
    }

    private void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
