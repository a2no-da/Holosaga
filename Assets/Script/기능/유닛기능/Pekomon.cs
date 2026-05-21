using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Pekomonm : MonoBehaviour
{
    public float Power;
    private SkeletonAnimation skeletonAnimation;
    public GameObject healPrefab;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (Mathf.Approximately(transform.position.y, 0.9600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -3;
        }
        else if (Mathf.Approximately(transform.position.y, -1.06f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -1;
        }
        else if (Mathf.Approximately(transform.position.y, -3.08f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 1;
        }
        else if (Mathf.Approximately(transform.position.y, -5.099999f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 3;
        }
    }

    public void Initialize(float Power)
    {
        this.Power = Power;
    }

    private void Start()
    {
        StartCoroutine(PekomonmBehaviour());
    }

    private IEnumerator PekomonmBehaviour()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, "start", false);

        yield return new WaitForSeconds(1.042f);

        skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);

        yield return new WaitForSeconds(2.916f);

        skeletonAnimation.AnimationState.SetAnimation(0, "end", false);

        yield return new WaitForSeconds(1.042f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Tower tower = other.GetComponentInParent<Tower>();
        if (tower != null)
        {
            StartCoroutine(PlayAnimationAndSound(healPrefab, tower.transform.position));
            if (tower.Health < tower.MaxHealth)
            {
                //tower.Heal(Power,);
            }
            StartCoroutine(PekomonDestroy());
        }
    }

    private IEnumerator PekomonDestroy()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, "end", false);

        yield return new WaitForSeconds(1.042f);

        Destroy(gameObject);
    }

    private IEnumerator PlayAnimationAndSound(GameObject prefab, Vector3 position)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        float animationDuration = 0f;
        yield return new WaitForSeconds(animationDuration);
    }
}
