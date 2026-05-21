using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Console : MonoBehaviour
{
    public TMP_InputField inputField; 
    public TMP_Text chatText;
    public RectTransform content;
    public GameObject panel;
    public GameObject prefabToSpawn;
    public GameObject Lui;
    public Vector3 spawnPosition;
    public Holomoney holomoney;
    public GameObject GetsugaTenshou;
    public SpawnM spawnM;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                string[] splitInput = inputField.text.Split(' ');

                switch (splitInput[0])
                {
                    case "/ts":
                        panel.gameObject.SetActive(true);
                        break;

                    case "/End":
                        panel.gameObject.SetActive(false);
                        break;

                    case "/up":
                        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                        break;

                    case "/lui":
                        Instantiate(Lui, spawnPosition, Quaternion.identity);
                        break;

                    case "/greed":
                        PlayerData data = holomoney.LoadPlayerData();
                        data.holomoney += 999999;
                        holomoney.SavePlayerData(data);
                        break;

                    case "/gmh9999":
                        GameManager.instance.mainHealth = 999999;
                        break;

                    case "/latest":
                        spawnM.LATest();
                        Tower.experience += 99999999;
                        break;

                    case "/월아":
                        Instantiate(GetsugaTenshou, new Vector3(0, 0, 0), Quaternion.identity);
                        break;
                }

                chatText.text += inputField.text + "\n"; 
                inputField.text = "";

                float textHeight = chatText.preferredHeight;

                content.sizeDelta = new Vector2(content.sizeDelta.x, textHeight);
            }
        }
    }
}
