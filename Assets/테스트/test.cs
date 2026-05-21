using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Tower tower;
    public int fast = 50;

    public void A()
    {
        tower.ApplyBuff("루나진저");
    }

    public void S()
    {
        tower.ApplyBuff("황금당근");
    }

    public void P()
    {
        tower.ApplyBuff("루나타임");
    }

    public void Fast()
    {
        Time.timeScale = fast;
    }

    public void NFast()
    {
        Time.timeScale = 1f;
    }

    public void Stop()
    {
        Time.timeScale = 0f;
    }
}
