using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Artpiece : MonoBehaviour
{
    public Holomoney holomoney;
    public TextMeshProUGUI normal;
    public TextMeshProUGUI epic;
    public TextMeshProUGUI unique;

    public TextMeshProUGUI Pnormal;
    public TextMeshProUGUI Pepic;
    public TextMeshProUGUI Punique;

    public TextMeshProUGUI PPnormal;
    public TextMeshProUGUI PPepic;
    public TextMeshProUGUI PPunique;

    void Start()
    {
        UPTTT();
    }

    public void UPTTT()
    {
        PlayerData playerData = holomoney.LoadPlayerData();
        PPnormal.text = playerData.ANormal.ToString();
        PPepic.text = playerData.AEpic.ToString();
        PPunique.text = playerData.AUnique.ToString();
    }
}
