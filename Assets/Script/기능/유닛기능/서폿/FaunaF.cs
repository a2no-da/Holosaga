using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FaunaF : MonoBehaviour
{
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public Vector2 raycastStartOffset2 = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    public Vector2 boxSize2 = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    public GameObject prefab;
    public SkeletonAnimation skeletonAnimation;
    public Fauna fauna;
    public float damage;
    public string Na;
    public AudioSource a1;
    public AudioSource a2;

    public void Start()
    {
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        int towerLayer = LayerMask.NameToLayer("Tower");
        layerMask = 1 << towerLayer;
        if (Na == "healField")
        {
            healF();
            Pa();
        }
        else
        {
            if (fauna.LevelS > 1)
            {
                FF();
            }
        }
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "fauna_VFX_pattern1_heal":
                a1.Play();
                break;
            case "fauna_VFX_pattern2_buff":
                a2.Play();
                break;
        }
    }

    public void PlayAni(string Name)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, Name, false);
    }

    public void Initialize(float damage, Fauna fauna, string NAM)
    {
        this.damage = damage;
        this.fauna = fauna;
        this.Na = NAM;
    }

    public void healF()
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
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Tower"))
            {
                GameObject parentObject = singleHit.collider.gameObject.transform.parent.gameObject;
                Tower tower = parentObject.GetComponent<Tower>();

                if (tower != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                {
                    if(tower != fauna)
                    {
                        if (!fauna.Towers.Contains(tower)) 
                        {
                            if (fauna.LevelS > 1)
                            {
                                fauna.Towers.Add(tower);
                            }
                        }
                    }
                    PlayAnimationAndSound(tower);
                    tower.Heal(damage, fauna);
                }
            }
        }
    }

    public void Pa()
    {
        if (fauna.LevelS > 1)
        {
            if (fauna.Towers != null)
            {
                fauna.PaJong();
            }
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    private void PlayAnimationAndSound(Tower tower)
    {
        Vector3 position = tower.transform.position;
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity, tower.transform);
    }

    public void FF()
    {
        DamageEnemiess.Clear();
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset2.x, transform.position.y + raycastStartOffset2.y);

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize2, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize2.x / 2, boxSize2.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize2.x / 2, boxSize2.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize2.x / 2, -boxSize2.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize2.x / 2, -boxSize2.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (RaycastHit2D singleHit in hitResults)
        {
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Tower"))
            {
                GameObject parentObject = singleHit.collider.gameObject.transform.parent.gameObject;
                Tower tower = parentObject.GetComponent<Tower>();

                if (tower != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                {
                    GameObject towerGameObject = tower.gameObject;
                    if (tower != fauna)
                    {
                        switch (Na)
                        {
                            case "powerField":
                                BuffManager.Instance.ApplyBuff(towerGameObject, "파종파워", tower.myB[25].duration, tower.myB[25].powerUp, tower.myB[25].speedUp, tower.myB[25].HpUp);
                                break;
                            case "speedField":
                                BuffManager.Instance.ApplyBuff(towerGameObject, "파종공속", tower.myB[26].duration, tower.myB[26].powerUp, tower.myB[26].speedUp, tower.myB[26].HpUp);
                                break;
                            case "hpupField":
                                BuffManager.Instance.ApplyBuff(towerGameObject, "파종체력업", tower.myB[27].duration, tower.myB[27].powerUp, tower.myB[27].speedUp, tower.myB[27].HpUp);
                                break;
                        }
                    }
                }
            }
        }
    }
}
