using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AoAka : MonoBehaviour
{
    public Tower tower;
    public MyColor color;
    private bool isok = false;
    public SkeletonAnimation skeletonAnimation;

    public enum MyColor
    {
        빨강,
        파랑
    }

    public void Initialize(Tower tower)
    {
        this.tower = tower;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AoAka otherAoAka = other.GetComponent<AoAka>();
        if (otherAoAka != null && otherAoAka.tower != tower) 
        {
            if (color != otherAoAka.color)
            {
                isok = true;
                GameObject towerGameObject = tower.gameObject;
                BuffManager.Instance.ApplyBuff(towerGameObject, "��ī�ɵ�6", tower.myB[17].duration, tower.myB[17].powerUp, tower.myB[17].speedUp, tower.myB[17].HpUp);
                skeletonAnimation.AnimationState.SetAnimation(0, "a6_active", true);
            }
        }

        if (isok)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SlowMon(0.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        AoAka otherAoAka = other.GetComponent<AoAka>();
        if (otherAoAka != null && otherAoAka.tower != tower) 
        {
            isok = false;
            tower.RemoveBuff("��ī�ɵ�6");
            skeletonAnimation.AnimationState.SetAnimation(0, "a6", true);
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ResetedSpeed();
        }
    }
}
