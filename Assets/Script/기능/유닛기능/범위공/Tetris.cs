using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Tetris : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Tower tower;
    public float speed;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();

    public AudioSource A1;
    public AudioSource A2;
    public GameObject Vf3die;

    public void Initialize(Tower tower)
    {
        this.tower = tower;
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        skeletonAnimation.timeScale = speed;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                A2.Play();
                Damage();
                break;
            case "suisei_VFX_pattern3_1":
                A1.Play();
                break;
        }
    }

    void Damage()
    {
        DamageEnemiess.Clear();
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (RaycastHit2D singleHit in hitResults)
        {
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                {
                    enemyScript.UpdateAttackingTower(tower);
                    if ((enemyScript.Health - (tower.Power * tower.myP[3].dmgCoe)) <= 0)
                    {
                        tower.attackCount3 += 1;
                        if (tower.attackCount3 >= 50)
                        {
                            tower.attackCount3 = 50;
                        }
                        Instantiate(Vf3die, enemyScript.transform.position, enemyScript.transform.rotation);
                        tower.P3T.text = tower.attackCount3.ToString();
                    }
                    enemyScript.TakeDamage(tower.Power * tower.myP[3].dmgCoe);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
