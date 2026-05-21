using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Soulball : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private bool End;
    private float destroyTime = 5f;
    public bool bos;
    public AudioSource audioSource;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        End = false;
    }

    private void Update()
    {
        if (!End && destroyTime > 0)
        {
            destroyTime -= Time.deltaTime; 
            if (destroyTime <= 0) 
            {
                Destroy(gameObject); 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tower") && !End)
        {
            Kali kali = other.GetComponentInParent<Kali>();
            if (kali != null)
            {
                if(bos)
                {
                    kali.Bossgetsoul();
                }
                else
                {
                    kali.getsoul();
                }
                StartCoroutine(Destroy());
            }
        }
    }

    private IEnumerator Destroy()
    {
        End = true;
        skeletonAnimation.AnimationState.SetAnimation(0, "pick", false);
        audioSource.Play();

        yield return new WaitForSeconds(1.667f);

        Destroy(gameObject);
    }
}
