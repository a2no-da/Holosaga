using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;
using Spine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    private Tower tower;
    private float offset = 0.2f;
    private Color originalColor;

    public Point GridPosition { get; private set; }

    public bool IsTowerPlaced { get; set; }

    public bool _isEmpty;

    private IEnumerator blinkTileCoroutine;

    public SkeletonDataAsset spineData;

    public bool CheckIsEmpty()
    {
        return _isEmpty; 
    }

    public Tower Tower { get; private set; }

    private SpriteRenderer spriteRenderer;

    //private bool isHighlighted = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Btn btnScript = GetComponent<Btn>();
        if (btnScript != null)
        {
            GameObject towerPrefab = btnScript.TowerPrefab;
        }
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return this.spriteRenderer;
    }

    public void Setup(Point gridPos, Vector3 wolrdPos, Transform parent)
    {
        _isEmpty = true;
        this.GridPosition = gridPos;
        transform.position = wolrdPos;
        transform.SetParent(parent);

        x = gridPos.X;
        y = gridPos.Y;
    }

    private void OnMouseOver()
    {
        if (EventSystem.current != null && GameManager.Instance != null && GameManager.Instance.ClickedBtn != null)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceTower();
                }
            }
        }
    }

    public void PlaceTower()
    {
        if (_isEmpty && GameManager.Instance.ClickedBtn != null && GameManager.Instance.ClickedBtn.TowerPrefab != null)
        {
            _isEmpty = false;
            float offset = 0.2f; 
            Vector3 towerPosition = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
            GameObject towerObject = Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, towerPosition, Quaternion.identity);
            tower = towerObject.GetComponent<Tower>(); 
            GameManager.Instance.AddTower(tower);
            tower.CurrentTile = this;

            MeshRenderer meshRenderer = towerObject.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.sortingOrder = (GridPosition.Y - 1);

            tower.InitializeMeshRenderer(GridPosition.Y - 1);

            towerObject.transform.SetParent(transform);

            Tile[] allTiles = FindObjectsOfType<Tile>();
        }
    }

    public void PlaceTowerID(string unitId)
    {
        if (_isEmpty)
        {
            _isEmpty = false;

            GameObject towerPrefab = GameManager.Instance.GetTowerPrefabById(unitId);

            if (towerPrefab != null)
            {
                float offset = 0.2f; 
                Vector3 towerPosition = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
                GameObject newUnit = Instantiate(towerPrefab, towerPosition, Quaternion.identity);
                tower = newUnit.GetComponent<Tower>(); 
                GameManager.Instance.AddTower(tower);
                tower.CurrentTile = this;

                MeshRenderer meshRenderer = newUnit.GetComponentInChildren<MeshRenderer>();
                if (meshRenderer != null)
                    meshRenderer.sortingOrder = (GridPosition.Y * 2 - 3);

                tower.InitializeMeshRenderer(GridPosition.Y * 2 - 3);

                newUnit.transform.SetParent(transform);

                Tile[] allTiles = FindObjectsOfType<Tile>();
            }
        }
    }

    public void MoveTower(Tower towerToMove)
    {
        Tile towerToMoveOriginalTile = towerToMove.CurrentTile;
        Tower existingTower = this.tower;

        if (existingTower != null)
        {
            PlayAndDestroySpineAnimation(this.tower);
            Tower tempTower = this.tower;
            this.tower = null;
            this._isEmpty = true;

            MoveToThisTile(towerToMove, towerToMoveOriginalTile);

            this._isEmpty = false;
            this.tower = towerToMove;

            towerToMoveOriginalTile.MoveToThisTile(tempTower, towerToMoveOriginalTile);
            tempTower.CurrentTile = towerToMoveOriginalTile;
            tempTower.upd();
            towerToMoveOriginalTile._isEmpty = false;
            towerToMoveOriginalTile.tower = tempTower;
            PlayAndDestroySpineAnimation(towerToMove);
            return;
        }
        else
        {
            MoveToThisTile(towerToMove, towerToMoveOriginalTile);
            this._isEmpty = false;
            this.tower = towerToMove;
            PlayAndDestroySpineAnimation(towerToMove);
        }
    }

    private void MoveToThisTile(Tower tower, Tile originalTile = null)
    {
        Vector3 towerPosition = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        tower.transform.position = towerPosition;
        this.SetTower(tower);
        tower.CurrentTile = this;
        tower.transform.SetParent(transform);

        this._isEmpty = false;
        this.tower = tower;

        if (originalTile != null)
        {
            originalTile._isEmpty = true;
            originalTile.tower = null;
        }
    }

    public void RemoveTower()
    {
        if (Tower != null)
        {
            Destroy(Tower.gameObject);
            Tower = null;
            _isEmpty = true;
        }
    }

    public void SetTower(Tower tower)
    {
        /*if (tower == null)
        {
            return;
        }*/

        this.tower = tower;
        _isEmpty = (tower == null);
    }

    public Tower GetTower()
    {
        return this.tower;
    }

    private void PlayAndDestroySpineAnimation(Tower tower)
    {
        SkeletonAnimation skeletonAnimation = tower.gameObject.GetComponent<SkeletonAnimation>();

        if (skeletonAnimation == null)
        {
            skeletonAnimation = tower.gameObject.AddComponent<SkeletonAnimation>();
        }

        if (skeletonAnimation.AnimationState != null)
        {
            skeletonAnimation.AnimationState.ClearTracks();
        }

        skeletonAnimation.skeletonDataAsset = spineData;
        skeletonAnimation.AnimationState.SetAnimation(0, "animation", false);
        skeletonAnimation.GetComponent<Renderer>().sortingOrder = -5;

        skeletonAnimation.AnimationState.Complete += (trackEntry) => {
            //Destroy(skeletonAnimation); 
        };
    }
}

