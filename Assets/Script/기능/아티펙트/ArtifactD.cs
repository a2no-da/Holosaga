using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.EventSystems;

public class ArtifactD : MonoBehaviour
{
    public Artifactss artifactss;
    public Buttons artifactButtons;
    public TextM TMP;
    public Mae mae;
    public BBack backk;
    public Haeche haeche;
    public Images images;
    public ArtifactManager artifactManager;
    public bool first;
    public Artpiece artpiece;
    public Holomoney holomoney;
    public GameObject NonePal;
    public GameObject SurePal;
    public GameObject EndPal;

    private int normalCount = 0;
    private int epicCount = 0;
    private int uniqueCount = 0;

    public TextMeshProUGUI atiName;
    public GameObject[] aS;
    public TextMeshProUGUI[] atiSul;
    public TextMeshProUGUI atiMainSul;

    [System.Serializable]
    public class Artifactss
    {
        public Artifact[] nomal;
        public Artifact[] epic;
        public Artifact[] unique;
    }

    [System.Serializable]
    public class TextM
    {
        public TextMeshProUGUI[] nomal;
        public TextMeshProUGUI[] epic;
        public TextMeshProUGUI[] unique;
    }

    [System.Serializable]
    public class Buttons
    {
        public Button[] nomal;
        public Button[] epic;
        public Button[] unique;
    }

    [System.Serializable]
    public class Mae
    {
        public GameObject[] nomal;
        public GameObject[] epic;
        public GameObject[] unique;
    }

    [System.Serializable]
    public class BBack
    {
        public GameObject[] nomal;
        public GameObject[] epic;
        public GameObject[] unique;
    }

    [System.Serializable]
    public class Images
    {
        public Sprite[] nomal;
        public Sprite[] epic;
        public Sprite[] unique;
    }

    [System.Serializable]
    public class Haeche
    {
        public GameObject[] haesss;
        public int[] mynum;
        public TextMeshProUGUI[] num;
    }

    void Start()
    {
        artifactManager.LoadArtifact();
        UpdateArtifactButtons();

        SetupButtons(artifactButtons.nomal, artifactss.nomal);
        SetupButtons(artifactButtons.epic, artifactss.epic);
        SetupButtons(artifactButtons.unique, artifactss.unique);
    }

    public void UpdateArtifactButtons()
    {
        UpdateButtons(artifactss.nomal, artifactButtons.nomal, TMP.nomal, mae.nomal, backk.nomal);
        UpdateButtons(artifactss.epic, artifactButtons.epic, TMP.epic, mae.epic, backk.epic);
        UpdateButtons(artifactss.unique, artifactButtons.unique, TMP.unique, mae.unique, backk.unique);
    }

    void UpdateButtons(Artifact[] artifacts, Button[] buttons, TextMeshProUGUI[] Tmp, GameObject[] gameObject1, GameObject[] gameObject2)
    {
        for (int i = 0; i < artifacts.Length; i++)
        {
            if (artifacts[i].Quantity <= 0)
            {
                Color buttonColor = buttons[i].image.color;
                buttonColor.a = 0.5f;
                buttons[i].image.color = buttonColor;

                Tmp[i].gameObject.SetActive(false);
                
                if(first)
                {
                    gameObject1[i].SetActive(false);
                    gameObject2[i].SetActive(false);

                    normalCount = 0;
                    artpiece.normal.text = normalCount.ToString();
                    epicCount = 0;
                    artpiece.epic.text = epicCount.ToString();
                    uniqueCount = 0;
                    artpiece.unique.text = uniqueCount.ToString();

                    RestText();
                }
            }
            else
            {
                Color buttonColor = buttons[i].image.color;
                buttonColor.a = 1f;
                buttons[i].image.color = buttonColor;

                buttons[i].interactable = true;
                Tmp[i].gameObject.SetActive(true);
                Tmp[i].text = artifacts[i].Quantity.ToString();

                if (first)
                {
                    gameObject1[i].SetActive(false);
                    gameObject2[i].SetActive(false);

                    normalCount = 0;
                    artpiece.normal.text = normalCount.ToString();
                    epicCount = 0;
                    artpiece.epic.text = epicCount.ToString();
                    uniqueCount = 0;
                    artpiece.unique.text = uniqueCount.ToString();
                    RestText();
                }
            }
        }
        first = false;
    }

    public void ABA(Artifact artifact)
    {
        BackReset();

        GameObject[] targetMae = null;
        GameObject[] targetBackk = null;

        switch (artifact.grade)
        {
            case Grade.Normal:
                targetMae = mae.nomal;
                targetBackk = backk.nomal;
                break;
            case Grade.Epic:
                targetMae = mae.epic;
                targetBackk = backk.epic;
                break;
            case Grade.Unique:
                targetMae = mae.unique;
                targetBackk = backk.unique;
                break;
        }

        bool hasActiveArtifact = false;

        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            if (haeche.haesss[i].activeSelf)
            {
                switch (artifact.grade)
                {
                    case Grade.Normal:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.nomal[artifact.index - 1])
                        {
                            if (haeche.mynum[i] > 0)
                            {
                                hasActiveArtifact = true;
                            }
                        }
                        break;
                    case Grade.Epic:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.epic[artifact.index - 1])
                        {
                            if (haeche.mynum[i] > 0)
                            {
                                hasActiveArtifact = true;
                            }
                        }
                        break;
                    case Grade.Unique:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.unique[artifact.index - 1])
                        {
                            if (haeche.mynum[i] > 0)
                            {
                                hasActiveArtifact = true;
                            }
                        }
                        break;
                }
            }
        }

        if (!hasActiveArtifact && artifact.Quantity <= 0)
        {
            return;
        }

        RestText();
        ChanText(artifact);

        if (hasActiveArtifact)
        {
            for (int i = 0; i < targetMae.Length; i++)
            {
                targetMae[i].SetActive(false);
                targetBackk[i].SetActive(false);
            }

            if (artifact.index >= 0 && artifact.index < targetMae.Length)
            {
                targetMae[artifact.index - 1].SetActive(true);
                targetBackk[artifact.index - 1].SetActive(true);
            }
        }
        else
        {
            if (artifact.index >= 0 && artifact.index < targetMae.Length)
            {
                if (targetMae[artifact.index - 1].activeSelf)
                {
                    targetMae[artifact.index - 1].SetActive(false);
                    targetBackk[artifact.index - 1].SetActive(false);
                }
                else
                {
                    for (int i = 0; i < targetMae.Length; i++)
                    {
                        targetMae[i].SetActive(false);
                        targetBackk[i].SetActive(false);
                    }

                    targetMae[artifact.index - 1].SetActive(true);
                    targetBackk[artifact.index - 1].SetActive(true);
                }
            }
        }
    }

    public void P(Artifact artifact)
    {
        if (artifact.Quantity <= 0)
            return;
        
        bool found = false;
        bool mus = false;

        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            if (haeche.haesss[i].activeSelf)
            {
                switch (artifact.grade)
                {
                    case Grade.Normal:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.nomal[artifact.index - 1])
                        {
                            if (haeche.mynum[i] < 20) 
                            {
                                haeche.mynum[i]++;
                                haeche.num[i].text = haeche.mynum[i].ToString();
                                normalCount++;
                                artpiece.normal.text = normalCount.ToString();
                                mus = true;
                            }
                            found = true;
                        }
                        break;
                    case Grade.Epic:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.epic[artifact.index - 1])
                        {
                            if (haeche.mynum[i] < 20)
                            {
                                haeche.mynum[i]++;
                                haeche.num[i].text = haeche.mynum[i].ToString();
                                epicCount++;
                                artpiece.epic.text = epicCount.ToString(); 
                                mus = true;
                            }
                            found = true;
                        }
                        break;
                    case Grade.Unique:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.unique[artifact.index - 1])
                        {
                            if (haeche.mynum[i] < 20) 
                            {
                                haeche.mynum[i]++;
                                haeche.num[i].text = haeche.mynum[i].ToString();
                                uniqueCount++;
                                artpiece.unique.text = uniqueCount.ToString();
                                mus = true;
                            }
                            found = true;
                        }
                        break;
                }

                if (mus)
                {
                    artifact.Quantity--;
                    UpdateArtifactButtons();
                }

                if (found) return;
            }

            if (!haeche.haesss[i].activeSelf)
            {
                switch (artifact.grade)
                {
                    case Grade.Normal:
                        if (CountArtifactsByGrade(Grade.Normal, artifact) < 12) 
                        {
                            haeche.haesss[i].GetComponent<Image>().sprite = images.nomal[artifact.index - 1];
                            haeche.mynum[i]++;
                            haeche.num[i].text = haeche.mynum[i].ToString();
                            haeche.haesss[i].SetActive(true);
                            normalCount++;
                            artpiece.normal.text = normalCount.ToString(); 
                            mus = true;
                        }
                        break;
                    case Grade.Epic:
                        if (CountArtifactsByGrade(Grade.Epic, artifact) < 12)
                        {
                            haeche.haesss[i].GetComponent<Image>().sprite = images.epic[artifact.index - 1];
                            haeche.mynum[i]++;
                            haeche.num[i].text = haeche.mynum[i].ToString();
                            haeche.haesss[i].SetActive(true);
                            epicCount++;
                            artpiece.epic.text = epicCount.ToString();
                            mus = true;
                        }
                        break;
                    case Grade.Unique:
                        if (CountArtifactsByGrade(Grade.Unique, artifact) < 12)
                        {
                            haeche.haesss[i].GetComponent<Image>().sprite = images.unique[artifact.index - 1];
                            haeche.mynum[i]++;
                            haeche.num[i].text = haeche.mynum[i].ToString();
                            haeche.haesss[i].SetActive(true);
                            uniqueCount++;
                            artpiece.unique.text = uniqueCount.ToString(); 
                            mus = true;
                        }
                        break;
                }
                break;
            }         
        }

        if (mus)
        {
            artifact.Quantity--;
            UpdateArtifactButtons();
        }
    }

    public void M(Artifact artifact)
    {
        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            if (haeche.haesss[i].activeSelf)
            {
                switch (artifact.grade)
                {
                    case Grade.Normal:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.nomal[artifact.index - 1])
                        {
                            if (haeche.mynum[i] > 0) 
                            {
                                haeche.mynum[i]--;
                                artifact.Quantity++;
                                haeche.num[i].text = haeche.mynum[i].ToString();

                                normalCount--;
                                artpiece.normal.text = normalCount.ToString();

                                if (haeche.mynum[i] == 0) 
                                {
                                    haeche.haesss[i].SetActive(false);
                                }
                            }
                        }
                        break;
                    case Grade.Epic:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.epic[artifact.index - 1])
                        {
                            if (haeche.mynum[i] > 0)
                            {
                                haeche.mynum[i]--;
                                artifact.Quantity++;
                                haeche.num[i].text = haeche.mynum[i].ToString();

                                epicCount--;
                                artpiece.epic.text = epicCount.ToString();

                                if (haeche.mynum[i] == 0)
                                {
                                    haeche.haesss[i].SetActive(false);
                                }
                            }
                        }
                        break;
                    case Grade.Unique:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.unique[artifact.index - 1])
                        {
                            if (haeche.mynum[i] > 0)
                            {
                                haeche.mynum[i]--;
                                artifact.Quantity++;
                                haeche.num[i].text = haeche.mynum[i].ToString();

                                uniqueCount--;
                                artpiece.unique.text = uniqueCount.ToString();

                                if (haeche.mynum[i] == 0)
                                {
                                    haeche.haesss[i].SetActive(false);
                                }
                            }
                        }
                        break;
                }
            }
        }

        UpdateArtifactButtons(); 
    }

    private int CountArtifactsByGrade(Grade grade, Artifact artifact)
    {
        int count = 0;
        for (int i = 0; i < haeche.mynum.Length; i++)
        {
            if (haeche.haesss[i].activeSelf)
            {
                switch (grade)
                {
                    case Grade.Normal:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.nomal[artifact.index - 1])
                            count++;
                        break;
                    case Grade.Epic:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.epic[artifact.index - 1])
                            count++;
                        break;
                    case Grade.Unique:
                        if (haeche.haesss[i].GetComponent<Image>().sprite == images.unique[artifact.index - 1])
                            count++;
                        break;
                }
            }
        }
        return count;
    }

    private void SetupButtons(Button[] buttons, Artifact[] artifacts)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            EventTrigger eventTrigger = buttons[i].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnterEntry.callback.AddListener((data) => { OnPointerEnter(artifacts[index]); });
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExitEntry.callback.AddListener((data) => { OnPointerExit(); });
            eventTrigger.triggers.Add(pointerExitEntry);
        }
    }

    private void OnPointerEnter(Artifact artifact)
    {
        ChanText(artifact);
    }

    private void OnPointerExit()
    {
        RestText();
    }

    public void ChanText(Artifact artifact)
    {
        string meg;
        switch (artifact.grade)
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
            atiName.text = localizedText;
        };

        var localizedItemPN = new LocalizedString { TableReference = "Item_artifact", TableEntryReference = "desc_" + meg + "_p" + artifact.index };

        localizedItemPN.StringChanged += (string localizedText) =>
        {
            atiMainSul.text = localizedText;
        };

        if (artifact.hpIncrease > 0)
        {
            aS[0].SetActive(true);
            atiSul[0].text = "HP +" + artifact.hpIncrease + "%";
        }
        else if (artifact.hpIncrease < 0)
        {
            aS[0].SetActive(true);
            atiSul[0].text = "HP " + artifact.hpIncrease + "%";
        }

        if (artifact.SpeedIncrease > 0)
        {
            aS[2].SetActive(true);
            atiSul[2].text = "SPEED +" + artifact.SpeedIncrease + "%";
        }
        else if (artifact.SpeedIncrease < 0)
        {
            aS[2].SetActive(true);
            atiSul[2].text = "SPEED " + artifact.SpeedIncrease + "%";
        }

        if (artifact.PowerIncrease > 0)
        {
            aS[1].SetActive(true);
            atiSul[1].text = "POWER +" + artifact.PowerIncrease + "%";
        }
        else if (artifact.PowerIncrease < 0)
        {
            aS[1].SetActive(true);
            atiSul[1].text = "POWER " + artifact.PowerIncrease + "%";
        }

        if (artifact.hpplus > 0)
        {
            aS[0].SetActive(true);
            atiSul[0].text = "HP +" + artifact.hpplus;
        }
        else if (artifact.hpplus < 0)
        {
            aS[0].SetActive(true);
            atiSul[0].text = "HP " + artifact.hpplus;
        }

        if (artifact.Powerplus > 0)
        {
            aS[1].SetActive(true);
            atiSul[1].text = "POWER +" + artifact.Powerplus;
        }
        else if (artifact.Powerplus < 0)
        {
            aS[1].SetActive(true);
            atiSul[1].text = "POWER " + artifact.Powerplus;
        }

        if (artifact.Speedplus > 0)
        {
            aS[2].SetActive(true);
            atiSul[2].text = "SPEED +" + artifact.Speedplus;
        }
        else if (artifact.Speedplus < 0)
        {
            aS[2].SetActive(true);
            atiSul[2].text = "SPEED " + artifact.Speedplus;
        }

        if (artifact.ASpeedplus > 0)
        {
            aS[3].SetActive(true);
            atiSul[3].text = "COOLTIME -" + artifact.ASpeedplus;
        }
        else if (artifact.ASpeedplus < 0)
        {
            aS[3].SetActive(true);
            atiSul[3].text = "COOLTIME +" + artifact.ASpeedplus * 1f;
        }

        if (artifact.ASpeedIncrease > 0)
        {
            aS[3].SetActive(true);
            atiSul[3].text = "COOLTIME -" + artifact.ASpeedIncrease + "%";
        }
        else if (artifact.ASpeedIncrease < 0)
        {
            aS[3].SetActive(true);
            atiSul[3].text = "COOLTIME +" + artifact.ASpeedIncrease * -1f + "%";
        }
    }

    public void RestText()
    {
        atiName.text = "";
        atiMainSul.text = "";

        foreach (var text in atiSul)
        {
            text.text = "";
        }

        foreach (var obj in aS)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void Reset()
    {
        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            if (haeche.haesss[i].activeSelf && haeche.mynum[i] > 0)
            {
                Artifact currentArtifact = GetArtifactByIndex(i);

                if (currentArtifact != null)
                {
                    currentArtifact.Quantity += haeche.mynum[i];
                }
            }
        }

        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            haeche.haesss[i].SetActive(false);
        }

        for (int i = 0; i < haeche.mynum.Length; i++)
        {
            haeche.mynum[i] = 0;
        }
    }

    public void BackReset()
    {
        if (mae != null)
        {
            foreach (var obj in mae.nomal)
            {
                if (obj != null) obj.SetActive(false);
            }
            foreach (var obj in mae.epic)
            {
                if (obj != null) obj.SetActive(false);
            }
            foreach (var obj in mae.unique)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        if (backk != null)
        {
            foreach (var obj in backk.nomal)
            {
                if (obj != null) obj.SetActive(false);
            }
            foreach (var obj in backk.epic)
            {
                if (obj != null) obj.SetActive(false);
            }
            foreach (var obj in backk.unique)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        RestText();
    }

    private Artifact GetArtifactByIndex(int index)
    {
        for (int j = 0; j < artifactss.nomal.Length; j++)
        {
            if (haeche.haesss[index].GetComponent<Image>().sprite == images.nomal[j])
            {
                return artifactss.nomal[j];
            }
        }

        for (int j = 0; j < artifactss.epic.Length; j++)
        {
            if (haeche.haesss[index].GetComponent<Image>().sprite == images.epic[j])
            {
                return artifactss.epic[j];
            }
        }

        for (int j = 0; j < artifactss.unique.Length; j++)
        {
            if (haeche.haesss[index].GetComponent<Image>().sprite == images.unique[j])
            {
                return artifactss.unique[j];
            }
        }

        return null; 
    }

    public void SureDecomposition()
    {
        bool hasArtifactsToDecompose = false;

        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            if (haeche.haesss[i].activeSelf && haeche.mynum[i] > 0)
            {
                hasArtifactsToDecompose = true;

                Artifact currentArtifact = GetArtifactByIndex(i);
            }
        }

        if (!hasArtifactsToDecompose)
        {
            NonePal.SetActive(true);
            SurePal.SetActive(false);
        }
        else
        {
            NonePal.SetActive(false);
            SurePal.SetActive(true);
        }
    }

    public void Decomposition()
    {
        SurePal.SetActive(false);
        EndPal.SetActive(true);

        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            if (haeche.haesss[i].activeSelf && haeche.mynum[i] > 0)
            {
                Artifact currentArtifact = GetArtifactByIndex(i);

                if (currentArtifact != null)
                {
                    if (currentArtifact.Quantity < 0)
                    {
                        currentArtifact.Quantity = 0;
                    }
                }
            }
        }

        artpiece.Pnormal.text = normalCount.ToString();
        artpiece.Pepic.text = epicCount.ToString();
        artpiece.Punique.text = uniqueCount.ToString();

        PlayerData playerData = holomoney.LoadPlayerData();
        playerData.ANormal += normalCount;
        playerData.AEpic += epicCount;
        playerData.AUnique += uniqueCount;
        holomoney.SavePlayerData(playerData);
        artifactManager.SaveArtifact();

        for (int i = 0; i < haeche.haesss.Length; i++)
        {
            haeche.haesss[i].SetActive(false);
        }

        for (int i = 0; i < haeche.mynum.Length; i++)
        {
            haeche.mynum[i] = 0;
        }

        first = true;
        artpiece.UPTTT();
        BackReset();
        UpdateArtifactButtons();
    }
}
