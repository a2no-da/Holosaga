using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public int Difficulty { get; set; }
    public DifficultyButton.DifficultyMode DifficultyMode { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetDifficulty()
    {
        Difficulty = 0;
        DifficultyMode = DifficultyButton.DifficultyMode.Basics;
    }
}
