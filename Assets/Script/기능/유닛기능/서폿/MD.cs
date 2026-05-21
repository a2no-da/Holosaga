using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class MD : MonoBehaviour
{
    public float Pull;

    private Enemy target;
    public Tower tower;
    private float maxX;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private List<KnockbackInfo> knockbackInfos = new List<KnockbackInfo>();
    HashSet<GameObject> knockedBackEnemies = new HashSet<GameObject>();
    public Force forceComponent;
    public SkeletonAnimation skeletonAnimation;
    private float timeSinceStart = 0f;
    private bool isGOActive = true;

    public void Initialize(float full, Tower tower)
    {
        this.Pull = full;
        this.tower = tower;
    }

    public class KnockbackInfo
    {
        public GameObject enemy;
        public float knockbackEndTime;

        public KnockbackInfo(GameObject enemy, float duration)
        {
            this.enemy = enemy;
            this.knockbackEndTime = Time.time + duration;
        }
    }

    void Start()
    {
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        if (forceComponent == null)
        {
            forceComponent = gameObject.AddComponent<Force>();
        }
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart >= 0.2f)
        {
            isGOActive = false;
            if (gameObject.tag != "Kabe")
            {
                gameObject.tag = "Kabe";
            }
        }

        if (isGOActive)
        {
            GO();
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    void GO()
    {
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
            Rigidbody2D enemyRb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
            if (enemyScript != null && !knockedBackEnemies.Contains(hit.collider.gameObject))
            {
                if (forceComponent != null)
                {
                    forceComponent.PublicApplyForce(enemyRb, new Vector2(Pull, 0));
                }
                knockedBackEnemies.Add(hit.collider.gameObject);
            }
        }
    }

    void OnDestroy()
    {
        foreach (GameObject enemyObj in knockedBackEnemies)
        {
            if (enemyObj != null)
            {
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                Rigidbody2D enemyRb = enemyObj.GetComponent<Rigidbody2D>();
                if (enemyRb != null && enemyScript != null)
                {
                    enemyRb.velocity = Vector2.zero;
                    enemyScript.isPushedOrPulled = false;
                }
            }
        }
    }
}
