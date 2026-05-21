using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyButton : MonoBehaviour
{
    public GameObject nomineral;

    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard,
        Extreme
    }

    public enum DifficultyMode
    {
        Basics,
        Infinite,
        Sandbag
    }

    public DifficultyLevel difficulty;
    public DifficultyMode difficultyMode;
    public Holomoney holomoney;

    public void OnCBasics()
    {
        DifficultyManager.Instance.DifficultyMode = DifficultyMode.Basics;
        SceneLoader.Instance.LoadScene("UnitSelect");
    }

    public void OnCInfinite()
    {
        DifficultyManager.Instance.DifficultyMode = DifficultyMode.Infinite;
        SceneLoader.Instance.LoadScene("UnitSelect");
    }

    public void OnCSandbag()
    {
        PlayerData playerData = holomoney.LoadPlayerData();
        DifficultyManager.Instance.DifficultyMode = DifficultyMode.Sandbag;
        SceneLoader.Instance.LoadScene("UnitSelect");
    }

    void Start()
    {
        PlayerData playerData = holomoney.LoadPlayerData();
        DifficultyManager.Instance.Difficulty = playerData.SellastNan;
        holomoney.SavePlayerData(playerData);
    }
}
