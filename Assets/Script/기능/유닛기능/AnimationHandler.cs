using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;

public class AnimationHandler : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public GameObject stunn;
    public GameObject position;
    public Coroutine stunCoroutine;
    public TMP_Text hpperText;

    void Start()
    {
        GameObject stunnInstance = Instantiate(stunn, position.transform.position, Quaternion.identity, position.transform) as GameObject;
        stunn = stunnInstance;
        skeletonAnimation = stunn.GetComponent<SkeletonAnimation>();
        stunn.SetActive(false);
    }

    public IEnumerator StunAnimationSequence(float waitTime)
    {
        if (stunn.transform.parent == null)
        {
            Debug.LogError("stunn.transform.parent is null");
        }

        float startTime = Time.time;
        stunn.SetActive(true);
        if (skeletonAnimation == null)
        {
            yield break;
        }

        if (skeletonAnimation.state == null)
        {
            yield break;
        }

        skeletonAnimation.state.SetAnimation(10, "stun_start", false);
        yield return new WaitForSeconds(0.25f);
        skeletonAnimation.state.SetAnimation(10, "stun", true);
        yield return new WaitForSeconds(waitTime - 0.875f);
        skeletonAnimation.state.SetAnimation(10, "stun_end", false);
        yield return new WaitForSeconds(0.625f);
        stunn.SetActive(false);

        stunCoroutine = null;
    }
}
