using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeohapBaegi : MonoBehaviour
{
    public float damage;
    public float stunDuration;
    private List<Tower> damagedTowers = new List<Tower>();

    void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.CompareTag("Tower"))
        {
            Transform parentTransform = collider.transform.parent;
            if (parentTransform == null) return;

            Tower tower = parentTransform.GetComponent<Tower>();
            if (tower == null || damagedTowers.Contains(tower)) return;

            ApplyDamageToTower(tower);
        }
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
            tower.Stun(stunDuration);
            damagedTowers.Add(tower);
        }
    }
}