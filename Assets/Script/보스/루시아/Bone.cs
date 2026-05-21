using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    public int hit_Limit;
    public float stunDuration;
    public Iusia lusia;
    private float maxX;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;

    public void Initialize(float damage, int hit_Limit, float stunDuration)
    {
        this.damage = damage;
        this.hit_Limit = hit_Limit;
        this.stunDuration = stunDuration;
    }

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(5f));
        maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        layerMask = 1 << LayerMask.NameToLayer("Tower");
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x > maxX || hit_Limit <= 0)
        {
            Destroy(gameObject);
        }

        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))
        {
            GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
            Tower tower = parentObject.GetComponent<Tower>();
            Tile tile = parentObject.GetComponentInParent<Tile>();
            if (tower != null)
            {
                if (hit_Limit > 0)
                {
                    tower.TakeDamage(damage * 2);
                    tower.Stun(stunDuration);
                    hit_Limit--;
                }
            }
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
