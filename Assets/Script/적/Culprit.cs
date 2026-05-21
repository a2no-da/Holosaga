using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culprit : Enemy
{
    public override void Start()
    {
        base.Start();

        endPointX = 10.8f;
    }

    public override void Update()
    {
        base.Update();
        MoveTowardsEndPoint();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower"))
        {
            targetEnemy = collider.transform.parent.gameObject;
            Tower tower = targetEnemy.GetComponent<Tower>();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower") && collider.transform.parent.gameObject == targetEnemy)
        {
            ising = false;
            targetEnemy = null;
            MoveTowardsEndPoint();
        }
    }

    private void HandleTowerMoved(Tower tower)
    {
        if (targetEnemy == tower.gameObject)
        {
            ising = false;
            targetEnemy = null;
        }
    }

    public override void Attack()
    {
    }

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled)
        {
            Vector3 direction = Vector3.right;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            SetAnimation(0, AnimClip[0], true);
        }
    }
}
