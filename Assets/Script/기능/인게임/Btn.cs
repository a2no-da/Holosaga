using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class Btn : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    public GameObject TowerPrefab { get { return towerPrefab; } set { towerPrefab = value; } }

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } }

    public SkeletonDataAsset SkeletonData { get; set; }

    public bool towerInstalled = false;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ClickButton);
        }
    }

    private void ClickButton()
    {
        if (!towerInstalled)
        {
            towerInstalled = true;
            button.interactable = false;
        }
    }

    public void ResetTowerInstallation()
    {
        towerInstalled = false;
        button.interactable = true;
    }
}
