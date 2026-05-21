using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS : MonoBehaviour
{
    public void Fast(int fast)
    {
        Time.timeScale = fast;
    }

    public void Slow(int slow)
    {
        Time.timeScale = slow;
    }
}
