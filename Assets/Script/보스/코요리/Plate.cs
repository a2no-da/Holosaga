using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Plate : MonoBehaviour
{
    public event Action OnDeactivate;
    public enum PlateType { Poison, Heal }
    public PlateType type;
    private LayerMask layerMask;
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    public GameObject healPrefab;
    private float damage;
    private float heal;
    public AudioSource po;

    private SkeletonAnimation skeletonAnimation;

    public void Initialize(float damage, float heal)
    {
        this.damage = damage;
        this.heal = heal;
    }

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    public void Start()
    {
        layerMask = LayerMask.GetMask("Tower");
    }

    public void Update()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        Vector2 raycastEnd = raycastStart - new Vector2(transform.right.x, transform.right.y) * raycastDistance;
        Vector2 direction = (raycastEnd - raycastStart).normalized;

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
        Debug.DrawLine(raycastStart, raycastEnd, Color.blue);
    }

    public virtual void Activate()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        Vector2 raycastEnd = raycastStart - new Vector2(transform.right.x, transform.right.y) * raycastDistance;
        Vector2 direction = (raycastEnd - raycastStart).normalized;

        Debug.DrawLine(raycastStart, raycastEnd, Color.blue);

        if (type == PlateType.Poison)
        {
            StartCoroutine(DoActionEveryHalfSecond(() =>
            {
                RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, direction, raycastDistance, layerMask);
                foreach (RaycastHit2D hit in hits)
                {
                    Tower tower = hit.collider.gameObject.GetComponentInParent<Tower>();
                    if (tower != null)
                    {
                        tower.TakeDamage(damage);
                    }
                }
            }, 6));
        }
        else if (type == PlateType.Heal)
        {
            StartCoroutine(DoActionEveryHalfSecond(() =>
            {
                RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, direction, raycastDistance, layerMask);
                foreach (RaycastHit2D hit in hits)
                {
                    Tower tower = hit.collider.gameObject.GetComponentInParent<Tower>();
                    if (tower != null)
                    {
                        tower.Heal(heal, null);
                        GameObject instance = Instantiate(healPrefab, Vector3.zero, Quaternion.identity, tower.transform);
                        instance.transform.localPosition = Vector3.zero;
                    }
                }
            }, 6));
        }
    }

    public void SetAnimation(string animationName)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);

        float delay;
        if (animationName == "damaging")
        {
            delay = 5f;
            po.Play();
        }
        else
        {
            delay = 5f;
        }

        StartCoroutine(DestroyAfterDelay(delay));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnDeactivate?.Invoke();
        Destroy(gameObject);
    }

    private IEnumerator DoActionEveryHalfSecond(Action action, int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            action();
            yield return new WaitForSeconds(0.5f);
        }
    }
}