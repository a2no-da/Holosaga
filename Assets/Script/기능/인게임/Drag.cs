using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
 
public class Drag : Singleton<Drag>  
{
    private SpriteRenderer spriteRenderer;

    private Tower tower;

    float startX;
    float startY;

    private Tile lastTile;

    private Vector2 lastMousePosition;
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        startX = -9.8f;
        startY = 2;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();

        canvas.sortingOrder = 15;

        if (spriteRenderer.enabled && GameManager.Instance.ClickedBtn != null)
        {
            Vector2 mouseScreenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (lastMousePosition != mouseScreenPos)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                int mouseGridX = Mathf.FloorToInt((mouseWorldPos.x - startX) / LevelManager.Instance.TileSize.x);
                int mouseGridY = Mathf.FloorToInt((startY - mouseWorldPos.y) / LevelManager.Instance.TileSize.y);

                Tile currentTile = GetTileAtGridPosition(mouseGridX, mouseGridY);

                if (currentTile != null && !currentTile.Equals(lastTile))
                {
                    //ResetHighlight();
                    //HighlightRange(currentTile);
                    lastTile = currentTile;
                }

                lastMousePosition = mouseScreenPos;
            }
        }
    }

    private void FollowMouse()
    {
        if (spriteRenderer.enabled)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    public Tile GetTileAtGridPosition(int x, int y)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.GetComponent<Tile>() != null)
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile.GridPosition.X == x && tile.GridPosition.Y == y)
            {
                return tile;
            }
        }

        return null;
    }

    public void Activate(SkeletonDataAsset skeletonData)
    {
        Color newColor = spriteRenderer.color;
        newColor.a = 0.5f;
        spriteRenderer.color = newColor;

        Tower towerComponent = GameManager.Instance.ClickedBtn.TowerPrefab.GetComponent<Tower>();

        if (towerComponent != null && towerComponent.TowerSkeletonData != null)
        {
            SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();
            if (skeletonAnimation != null)
            {
                skeletonAnimation.skeletonDataAsset = towerComponent.TowerSkeletonData;
                skeletonAnimation.Initialize(true);
            }
        }

        spriteRenderer.enabled = true;
    }
}