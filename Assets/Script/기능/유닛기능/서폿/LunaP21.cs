using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class LunaP21 : MonoBehaviour
{
    public GameObject lunaP2;
    public Luna luna;
    public int Power;
    public int H;
    public float speed;

    private SkeletonAnimation skeletonAnimation;
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;

    private float one;
    private float two;
    private float three;

    public void Initialize(Luna luna, float Power, float Speed, int Hit, float one, float two, float three)
    {
        this.luna = luna;
        this.Power = (int)Power;
        this.H = Hit;
        this.speed = Speed;
        this.one = one;
        this.two = two;
        this.three = three;
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        DestroyAfterSeconds(5f);
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        SP();
    }

    public void SP()
    {
        float localY = transform.localPosition.y;

        if (Mathf.Approximately(localY, 0.8600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -3;
        }
        else if (Mathf.Approximately(localY, -1.16f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -1;
        }
        else if (Mathf.Approximately(localY, -3.18f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 1;
        }
        else if (Mathf.Approximately(localY, -5.2f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 3;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (H <= 0)
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

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                if (H > 0)
                {
                    enemyScript.UpdateAttackingTower(luna);
                    enemyScript.TakeDamage(Power);

                    Vector3 newPosition = transform.position;
                    newPosition.x -= 0.1f;

                    GameObject lunaInstance = Instantiate(lunaP2, newPosition, Quaternion.identity);
                    lunaInstance.GetComponent<LunaP22>().Initialize(one, two, three, luna);
                    H--;
                }
            }
        }
    }

    void DestroyAfterSeconds(float seconds)
    {
        Destroy(gameObject, seconds);
    }
}
