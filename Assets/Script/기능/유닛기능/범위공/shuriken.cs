using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class shuriken : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public Kanata kanata;

    private int damageEventCount = 0;
    private float Power;
    private LayerMask layerMask;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        layerMask = LayerMask.GetMask("Enemy");
    }

    public void Initialize(Kanata kanata, float Power)
    {
        this.kanata = kanata;
        this.Power = (int)Power;
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_p2":
                PlaySound(0);
                P2();
                damageEventCount++;
                break;
        }
    }

    private void PlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < audioClips.Length)
        {
            audioSource.PlayOneShot(audioClips[soundIndex]);
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "attack")
        {
            Destroy(gameObject);
        }
    }

    public void P2()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + 2.475f, transform.position.y + 0.8f);
        Vector2 boxSize = new Vector2(1.65f * 2f, 2.02f);
        Vector2 direction = Vector2.right;
        float raycastDistance = 0f;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, direction, raycastDistance, layerMask);

        Vector2 bottomLeft = raycastStart - new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 topLeft = raycastStart - new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(bottomLeft, bottomRight, Color.red, 2f);
        Debug.DrawLine(bottomRight, topRight, Color.red, 2f);
        Debug.DrawLine(topRight, topLeft, Color.red, 2f);
        Debug.DrawLine(topLeft, bottomLeft, Color.red, 2f);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.UpdateAttackingTower(kanata);
                    enemy.TakeDamage(Power);
                    kanata.Gorilla();
                }
            }
        }
    }
}
