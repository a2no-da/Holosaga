using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine;
using Spine.Unity;

public class StartScene : MonoBehaviour
{
    public SkeletonGraphic skeletonGraphic;

    public void Shop()
    {
        SceneLoader.Instance.LoadScene("Shop");
    }

    public void Collection()
    {
        SceneLoader.Instance.LoadScene("Collection");
    }

    public void GoMain()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("Main");
    }

    public void Artifact()
    {
        SceneLoader.Instance.LoadScene("Artifact");
    }

    public void PlaySpineUIAnimationOnce(string animationName)
    {
        if (skeletonGraphic == null)
        {
            return;
        }

        skeletonGraphic.AnimationState.ClearTracks();
        skeletonGraphic.AnimationState.SetAnimation(0, animationName, false);
    }
}
