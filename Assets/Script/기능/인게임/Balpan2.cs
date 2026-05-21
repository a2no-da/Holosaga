using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Balpan2 : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public MeshRenderer towerRenderer;
    private MeshRenderer myRenderer;
    
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        myRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (towerRenderer != null)
        {
            myRenderer.sortingOrder = towerRenderer.sortingOrder + 1;
        }
    }
}
