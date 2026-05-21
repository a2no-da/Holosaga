using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllKill : MonoBehaviour
{
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    public bool boos;

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");

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

                switch (enemyScript.type)
                {
                    case Enemy.EnemyType.Normal:
                    case Enemy.EnemyType.Special:
                        if (enemyScript != null)
                        {
                            enemyScript.TakeDamage(9999);
                        }
                        break;
                    case Enemy.EnemyType.Boss:
                        if(boos)
                        {
                            if (enemyScript != null)
                            {
                                enemyScript.TakeDamage(99999);
                            }
                        }
                        break;
                }
            }
        }

        Destroy(gameObject);
    }
}
