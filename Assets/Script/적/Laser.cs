using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float damage;
    private List<Tower> damagedTowers = new List<Tower>();
    public LayerMask towerLayer;
    public Vector2 boxSize;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void Initialize()
    {
        damagedTowers.Clear();
    }

    void Update()
    {
        Vector2 startPosition = new Vector2(transform.position.x - 9.5f, transform.position.y + 0.2f);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(startPosition, boxSize, 0f, Vector2.zero, 0f, towerLayer);

        Vector3 boxCenter = startPosition;
        Vector3 topLeft = boxCenter + new Vector3(-boxSize.x / 2, boxSize.y / 2);
        Vector3 topRight = boxCenter + new Vector3(boxSize.x / 2, boxSize.y / 2);
        Vector3 bottomLeft = boxCenter + new Vector3(-boxSize.x / 2, -boxSize.y / 2);
        Vector3 bottomRight = boxCenter + new Vector3(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                Transform parentTransform = hit.collider.transform.parent;
                if (parentTransform == null) continue;

                Tower tower = parentTransform.GetComponent<Tower>();
                if (tower == null || damagedTowers.Contains(tower)) continue;

                ApplyDamageToTower(tower);
            }
        }
    }

    void OnEnable()
    {
        Initialize();
    }

    public void ApplyDamageToTower(Tower tower)
    {
        if (tower == null)
        {
            return;
        }
        Tile tile = tower.GetComponentInParent<Tile>();

        if (!damagedTowers.Contains(tower))
        {
            tower.TakeDamage(damage);
            damagedTowers.Add(tower);
        }
    }
}

