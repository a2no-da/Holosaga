using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class Arti : MonoBehaviour
{
    public Holomoney holomoney;
    public ArtifactManager artifactManager;
    public Artifactss artifactss;
    public GameObject buyA;
    public GameObject ArtiDisplayPrefab;
    public Transform ArtiDisplayParent;
    public GameObject NoPiece;
    public int index;
    private List<GameObject> artiDisplayPool = new List<GameObject>();
    public int poolSize = 10;

    public Image MIG;
    public TMP_Text Nm;
    public TMP_Text[] SulS;
    public GameObject[] SulSO;
    public TMP_Text SulM;

    public GameObject NoneP;
    public GameObject YesB;

    public Artpiece artpiece;
    public Artifact selAti;
    public ArtiProfile artiProfile;

    [System.Serializable]
    public class Artifactss
    {
        public Artifact[] nomal;
        public Artifact[] epic;
        public Artifact[] unique;
    }

    void Start()
    {
        artifactManager.LoadArtifact();
        InitializePool();
        DisplayAll();
    }

    void InitializePool()
    {
        int index = 0;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject display = Instantiate(ArtiDisplayPrefab, ArtiDisplayParent);
            ArtiProfile artiProfile = display.GetComponent<ArtiProfile>();
            artiProfile.i = index++;
            artiProfile.Set(this);
            display.SetActive(false);
            artiDisplayPool.Add(display);
        }
    }

    void ResetPool()
    {
        foreach (GameObject display in artiDisplayPool)
        {
            display.SetActive(false);
        }
    }

    public void DisplayAll()
    {
        ResetPool();
        selAti = null;

        int displayIndex = 0;

        List<Artifact> allArtifacts = new List<Artifact>();
        allArtifacts.AddRange(artifactss.nomal);
        allArtifacts.AddRange(artifactss.epic);
        allArtifacts.AddRange(artifactss.unique);

        foreach (var artifact in allArtifacts)
        {
            if (displayIndex >= artiDisplayPool.Count) break;

            GameObject display = artiDisplayPool[displayIndex];
            display.SetActive(true);

            ArtiProfile artiProfile = display.GetComponent<ArtiProfile>();
            if (artiProfile != null)
            {
                artiProfile.SetArtifact(artifact);
            }
            displayIndex++;
        }
    }

    public void Buyy()
    {
        Artifact artifact = selAti;

        int requiredPieces = artifact.Price;
        PlayerData playerData = holomoney.LoadPlayerData();

        switch (artifact.grade)
        {
            case Grade.Normal:
                if (playerData.ANormal >= requiredPieces)
                {
                    YesB.SetActive(true); 
                    NoneP.SetActive(false);
                }
                else
                {
                    NoneP.SetActive(true);
                    YesB.SetActive(false); 
                }
                break;

            case Grade.Epic:
                if (playerData.AEpic >= requiredPieces)
                {
                    YesB.SetActive(true);
                    NoneP.SetActive(false);
                }
                else
                {
                    NoneP.SetActive(true);
                    YesB.SetActive(false);
                }
                break;

            case Grade.Unique:
                if (playerData.AUnique >= requiredPieces)
                {
                    YesB.SetActive(true);
                    NoneP.SetActive(false);
                }
                else
                {
                    NoneP.SetActive(true);
                    YesB.SetActive(false);
                }
                break;

            default:
                Debug.LogError("없음");
                break;
        }
    }

    public void hontoBuyy()
    {
        artifactManager.LoadArtifact();
        Artifact artifact = selAti;

        int requiredPieces = artifact.Price;
        PlayerData playerData = holomoney.LoadPlayerData();

        switch (artifact.grade)
        {
            case Grade.Normal:
                if (playerData.ANormal >= requiredPieces)
                {
                    YesB.SetActive(false);
                    NoneP.SetActive(false);
                    playerData.ANormal = playerData.ANormal - requiredPieces;
                    artifact.Quantity++; 
                }
                break;

            case Grade.Epic:
                if (playerData.AEpic >= requiredPieces)
                {
                    YesB.SetActive(false);
                    NoneP.SetActive(false);
                    playerData.AEpic = playerData.AEpic - requiredPieces;
                    artifact.Quantity++;
                }
                break;

            case Grade.Unique:
                if (playerData.AUnique >= requiredPieces)
                {
                    YesB.SetActive(false);
                    NoneP.SetActive(false);
                    playerData.AUnique = playerData.AUnique - requiredPieces;
                    artifact.Quantity++;
                }
                break;

            default:
                Debug.LogError("없음");
                break;
        }

        holomoney.SavePlayerData(playerData);
        artifactManager.SaveArtifact();
        artiProfile.quna();
        artpiece.UPTTT();
    }

    public void UPText(int I, Grade grade, Sprite sprite, Artifact artifact)
    {
        NoneText();
        MIG.gameObject.SetActive(true);
        MIG.GetComponent<Image>().sprite = sprite;

        string meg;
        switch (grade)
        {
            case Grade.Normal:
                meg = "c";
                break;
            case Grade.Epic:
                meg = "r";
                break;
            case Grade.Unique:
                meg = "u";
                break;
            default:
                meg = null;
                break;
        }

        var localizedString = new LocalizedString { TableReference = "Item_artifact", TableEntryReference = "name_" + meg + "_p" + artifact.index };

        localizedString.StringChanged += (string localizedText) =>
        {
            Nm.text = localizedText;
        };

        var localizedItemPN = new LocalizedString { TableReference = "Item_artifact", TableEntryReference = "desc_" + meg + "_p" + artifact.index };

        localizedItemPN.StringChanged += (string localizedText) =>
        {
            SulM.text = localizedText;
        };

        if (artifact.hpIncrease > 0)
        {
            SulSO[0].SetActive(true);
            SulS[0].gameObject.SetActive(true);
            SulS[0].text = "HP +" + artifact.hpIncrease + "%";
        }
        else if(artifact.hpIncrease < 0)
        {
            SulSO[0].SetActive(true);
            SulS[0].gameObject.SetActive(true);
            SulS[0].text = "HP " + artifact.hpIncrease + "%";
        }

        if (artifact.SpeedIncrease > 0)
        {
            SulSO[1].SetActive(true);
            SulS[1].gameObject.SetActive(true);
            SulS[1].text = "SPEED +" + artifact.SpeedIncrease + "%";
        }
        else if (artifact.SpeedIncrease < 0)
        {
            SulSO[1].SetActive(true);
            SulS[1].gameObject.SetActive(true);
            SulS[1].text = "SPEED " + artifact.SpeedIncrease + "%";
        }

        if (artifact.PowerIncrease > 0)
        {
            SulSO[2].SetActive(true);
            SulS[2].gameObject.SetActive(true);
            SulS[2].text = "POWER +" + artifact.PowerIncrease + "%";
        }
        else if (artifact.PowerIncrease < 0)
        {
            SulSO[2].SetActive(true);
            SulS[2].gameObject.SetActive(true);
            SulS[2].text = "POWER " + artifact.PowerIncrease + "%";
        }

        if (artifact.hpplus > 0)
        {
            SulSO[0].SetActive(true);
            SulS[0].gameObject.SetActive(true);
            SulS[0].text = "HP +" + artifact.hpplus;
        }
        else if (artifact.hpplus < 0)
        {
            SulSO[0].SetActive(true);
            SulS[0].gameObject.SetActive(true);
            SulS[0].text = "HP " + artifact.hpplus;
        }

        if (artifact.Powerplus > 0)
        {
            SulSO[2].SetActive(true);
            SulS[2].gameObject.SetActive(true);
            SulS[2].text = "POWER +" + artifact.Powerplus;
        }
        else if (artifact.Powerplus < 0)
        {
            SulSO[2].SetActive(true);
            SulS[2].gameObject.SetActive(true);
            SulS[2].text = "POWER " + artifact.Powerplus;
        }

        if (artifact.Speedplus > 0)
        {
            SulSO[1].SetActive(true);
            SulS[1].gameObject.SetActive(true);
            SulS[1].text = "SPEED +" + artifact.Speedplus;
        }
        else if (artifact.Speedplus < 0) 
        {
            SulSO[1].SetActive(true);
            SulS[1].gameObject.SetActive(true);
            SulS[1].text = "SPEED " + artifact.Speedplus;
        }

        if (artifact.ASpeedplus > 0)
        {
            SulSO[3].SetActive(true);
            SulS[3].gameObject.SetActive(true);
            SulS[3].text = "COOLTIME -" + artifact.ASpeedplus;
        }
        else if (artifact.ASpeedplus < 0) 
        {
            SulSO[3].SetActive(true);
            SulS[3].gameObject.SetActive(true);
            SulS[3].text = "COOLTIME +" + artifact.ASpeedplus;
        }


        if (artifact.ASpeedIncrease > 0)
        {
            SulSO[3].SetActive(true);
            SulS[3].gameObject.SetActive(true);
            SulS[3].text = "COOLTIME -" + artifact.ASpeedIncrease + "%";
        }
        else if (artifact.ASpeedIncrease < 0)
        {
            SulSO[3].SetActive(true);
            SulS[3].gameObject.SetActive(true);
            SulS[3].text = "COOLTIME +" + artifact.ASpeedIncrease + "%";
        }
    }

    public void NoneText()
    {
        MIG.gameObject.SetActive(false);

        Nm.text = "";
        SulM.text = "";

        foreach (var text in SulS)
        {
            text.text = "";
            text.gameObject.SetActive(false);
        }

        foreach (var obj in SulSO)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}
