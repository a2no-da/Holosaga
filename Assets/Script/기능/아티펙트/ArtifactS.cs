using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Localization;

public class ArtifactS : MonoBehaviour
{
    public Upgrade upgrade;
    public Button selectArtifactButton;
    public Button selectbackButton;
    public TextM TMP;
    public Buttons artifactButtons;
    public Artifactss artifactss;
    public Images images;
    public BBack backk;
    public GameObject gpanal;
    public Dicton dicton;
    public ArtifactManager artifactManager;
    public TextMeshProUGUI bt;

    public TextMeshProUGUI atiName;
    public GameObject[] aS;
    public TextMeshProUGUI[] atiSul;
    public TextMeshProUGUI atiMainSul;

    public TextMeshProUGUI MEatiName;
    public GameObject[] MEaS;
    public TextMeshProUGUI[] MEatiSul;
    public TextMeshProUGUI MEatiMainSul;

    public GameObject noati;
    public GameObject yesati;
    private bool yes;

    public TextMeshProUGUI hpp;
    public TextMeshProUGUI powerp;
    public Unit SelTower;

    [System.Serializable]
    public class Artifactss
    {
        public Artifact[] nomal;
        public Artifact[] epic;
        public Artifact[] unique;
    }

    [System.Serializable]
    public class Buttons
    {
        public Button[] nomal;
        public Button[] epic;
        public Button[] unique;
    }

    [System.Serializable]
    public class BBack
    {
        public GameObject[] nomal;
        public GameObject[] epic;
        public GameObject[] unique;
    }

    [System.Serializable]
    public class TextM
    {
        public TextMeshProUGUI[] nomal;
        public TextMeshProUGUI[] epic;
        public TextMeshProUGUI[] unique;
    }

    [System.Serializable]
    public class Images
    {
        public Sprite[] nomal;
        public Sprite[] epic;
        public Sprite[] unique;
        public Sprite reset;
    }

    void Start()
    {
        artifactManager.LoadArtifact();
        UpdateArtifactButtons();
        selectbackButton.gameObject.SetActive(false);

        SetupButtons(artifactButtons.nomal, artifactss.nomal);
        SetupButtons(artifactButtons.epic, artifactss.epic);
        SetupButtons(artifactButtons.unique, artifactss.unique);
    }

    public void selectati(Artifact artifact)
    {
        if (artifact == null)
        {
            selectArtifactButton.image.sprite = images.reset;
            bt.gameObject.SetActive(false);
        }
        else
        {
            if (artifact.Quantity <= 0) { return; }

            ChanText(artifact);

            int ind = artifact.index - 1;

            bool isActivated = false;

            if (artifact.grade == Grade.Normal)
            {
                isActivated = ActivateBBack(ind, Grade.Normal);
            }
            else if (artifact.grade == Grade.Epic)
            {
                isActivated = ActivateBBack(ind, Grade.Epic);
            }
            else if (artifact.grade == Grade.Unique)
            {
                isActivated = ActivateBBack(ind, Grade.Unique);
            }

            if (!isActivated) return;

            DeactivateAllBackk();
            Sprite newSprite = images.reset;

            if (artifact.grade == Grade.Normal)
            {
                newSprite = images.nomal[artifact.index - 1];
            }
            else if (artifact.grade == Grade.Epic)
            {
                newSprite = images.epic[artifact.index - 1];
            }
            else if (artifact.grade == Grade.Unique)
            {
                newSprite = images.unique[artifact.index - 1];
            }

            selectArtifactButton.image.sprite = newSprite;

            if (dicton.myartifact != artifact)
            {
                artifact.Quantity -= 1;
                if (dicton.myartifact != null)
                {
                    dicton.myartifact.Quantity += 1;
                }
            }

            dicton.myartifact = artifact;
            dicton.myArtifactId = artifact.artifactId;

            dicton.SaveAll();
            artifactManager.SaveArtifact();

            noati.SetActive(false);
            yesati.SetActive(true);

            UpdateArtifactButtons();
            selectbackButton.gameObject.SetActive(false);
            bt.gameObject.SetActive(false);
            RestText();
            MChanText(artifact);
            gpanal.SetActive(true);
        }
    }

    private bool ActivateBBack(int index, Grade grade)
    {
        if (backk != null)
        {
            switch (grade)
            {
                case Grade.Normal:
                    if (index < backk.nomal.Length)
                    {
                        if (!backk.nomal[index].activeSelf)
                        {
                            backk.nomal[index].SetActive(true);
                            DeactivateAllBackkk(backk.nomal[index]);
                            return false; 
                        }
                        return true; 
                    }
                    break;

                case Grade.Epic:
                    if (index < backk.epic.Length)
                    {
                        if (!backk.epic[index].activeSelf)
                        {
                            backk.epic[index].SetActive(true);
                            DeactivateAllBackkk(backk.epic[index]);
                            return false; 
                        }
                        return true; 
                    }
                    break;

                case Grade.Unique:
                    if (index < backk.unique.Length)
                    {
                        if (!backk.unique[index].activeSelf)
                        {
                            backk.unique[index].SetActive(true);
                            DeactivateAllBackkk(backk.unique[index]);
                            return false;
                        }
                        return true; 
                    }
                    break;
            }
        }
        return false; 
    }

    private void DeactivateAllBackkk(GameObject activeObj)
    {
        if (backk != null)
        {
            foreach (var obj in backk.nomal)
            {
                if (obj != null && obj != activeObj)
                {
                    obj.SetActive(false);
                }
            }
            foreach (var obj in backk.epic)
            {
                if (obj != null && obj != activeObj) 
                {
                    obj.SetActive(false);
                }
            }
            foreach (var obj in backk.unique)
            {
                if (obj != null && obj != activeObj)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    private void DeactivateAllBackk()
    {
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

        foreach (var obj in aS)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void ChanText(Artifact artifact)
    {
        RestText();

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
            atiSul[3].text = "COOLTIME " + artifact.ASpeedplus;
        }

        if (artifact.ASpeedIncrease > 0)
        {
            aS[3].SetActive(true);
            atiSul[3].text = "COOLTIME -" + artifact.ASpeedIncrease + "%";
        }
        else if (artifact.ASpeedIncrease < 0)
        {
            aS[3].SetActive(true);
            atiSul[3].text = "COOLTIME " + artifact.ASpeedIncrease + "%";
        }
    }

    public void MChanText(Artifact artifact)
    {
        RestMeText();

        float currentHP, currentPower, newHP = 0, newPower = 0;

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

        if (SelTower != null)
        {
            currentHP = SelTower.MaxHealth + (SelTower.MaxHealth * 0.2f * SelTower.dicton.HpUp);
            //float currentSpeed = SelTower.CurrentSpeed; 
            currentPower = SelTower.Power + (SelTower.Power * 0.2f * SelTower.dicton.PowerUp);
            newHP = currentHP * (artifact.hpIncrease / 100f);
            //float newSpeed = currentSpeed * (1 + artifact.SpeedIncrease / 100f);
            newPower = currentPower * (artifact.PowerIncrease / 100f);
        }

        var localizedString = new LocalizedString { TableReference = "Item_artifact", TableEntryReference = "name_" + meg + "_p" + artifact.index };

        localizedString.StringChanged += (string localizedText) =>
        {
            MEatiName.text = localizedText;
        };

        var localizedItemPN = new LocalizedString { TableReference = "Item_artifact", TableEntryReference = "desc_" + meg + "_p" + artifact.index };

        localizedItemPN.StringChanged += (string localizedText) =>
        {
            MEatiMainSul.text = localizedText;
        };

        if (artifact.hpIncrease > 0)
        {
            MEaS[0].SetActive(true);
            MEatiSul[0].text = "HP +" + artifact.hpIncrease + "%";
            hpp.text = "( +" + newHP + ")";
        }
        else if (artifact.hpIncrease < 0)
        {
            MEaS[0].SetActive(true);
            MEatiSul[0].text = "HP " + artifact.hpIncrease + "%";
            hpp.text = "( " + newHP + ")";
        }

        if (artifact.SpeedIncrease > 0)
        {
            MEaS[2].SetActive(true);
            MEatiSul[2].text = "SPEED +" + artifact.SpeedIncrease + "%";
        }
        else if (artifact.SpeedIncrease < 0)
        {
            MEaS[2].SetActive(true);
            MEatiSul[2].text = "SPEED " + artifact.SpeedIncrease + "%";
        }

        if (artifact.PowerIncrease > 0)
        {
            MEaS[1].SetActive(true);
            MEatiSul[1].text = "POWER +" + artifact.PowerIncrease + "%";
            powerp.text = "( +" + newPower + ")";
        }
        else if (artifact.PowerIncrease < 0)
        {
            MEaS[1].SetActive(true);
            MEatiSul[1].text = "POWER " + artifact.PowerIncrease + "%";
            powerp.text = "( " + newPower + ")";
        }

        if (artifact.hpplus > 0)
        {
            MEaS[0].SetActive(true);
            MEatiSul[0].text = "HP +" + artifact.hpplus;
            hpp.text = "( +" + artifact.hpplus + ")";
        }
        else if (artifact.hpplus < 0)
        {
            MEaS[0].SetActive(true);
            MEatiSul[0].text = "HP " + artifact.hpplus;
            hpp.text = "( " + artifact.hpplus + ")";
        }

        if (artifact.Powerplus > 0)
        {
            MEaS[1].SetActive(true);
            MEatiSul[1].text = "POWER +" + artifact.Powerplus;
            powerp.text = "( +" + artifact.Powerplus + ")";
        }
        else if (artifact.Powerplus < 0)
        {
            MEaS[1].SetActive(true);
            MEatiSul[1].text = "POWER " + artifact.Powerplus;
            powerp.text = "( " + artifact.Powerplus + ")";
        }

        if (artifact.Speedplus > 0)
        {
            MEaS[2].SetActive(true);
            MEatiSul[2].text = "SPEED +" + artifact.Speedplus;
        }
        else if (artifact.Speedplus < 0)
        {
            MEaS[2].SetActive(true);
            MEatiSul[2].text = "SPEED " + artifact.Speedplus;
        }

        if (artifact.ASpeedplus > 0)
        {
            MEaS[3].SetActive(true);
            MEatiSul[3].text = "COOLTIME -" + artifact.ASpeedplus;
        }
        else if (artifact.ASpeedplus < 0)
        {
            MEaS[3].SetActive(true);
            MEatiSul[3].text = "COOLTIME +" + artifact.ASpeedplus * -1f;
        }

        if (artifact.ASpeedIncrease > 0)
        {
            MEaS[3].SetActive(true);
            MEatiSul[3].text = "COOLTIME -" + artifact.ASpeedIncrease + "%";
        }
        else if (artifact.ASpeedIncrease < 0)
        {
            MEaS[3].SetActive(true);
            MEatiSul[3].text = "COOLTIME +" + artifact.ASpeedIncrease * -1f + "%";
        }

        upgrade.PlText();
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

    public void first(Artifact artifact)
    {
        if (artifact == null)
        {
            selectArtifactButton.image.sprite = images.reset;
            selectbackButton.gameObject.SetActive(false);
            bt.gameObject.SetActive(false);
            noati.SetActive(true);
            yesati.SetActive(false);
            RestText();
            RestMeText();
        }
        else
        {
            dicton.myartifact = artifact;

            Sprite newSprite = images.reset;

            if (artifact.grade == Grade.Normal)
            {
                newSprite = images.nomal[artifact.index - 1];
            }
            else if (artifact.grade == Grade.Epic)
            {
                newSprite = images.epic[artifact.index - 1];
            }
            else if (artifact.grade == Grade.Unique)
            {
                newSprite = images.unique[artifact.index - 1];
            }

            noati.SetActive(false);
            yesati.SetActive(true);

            MChanText(artifact);
            selectArtifactButton.image.sprite = newSprite;
            bt.gameObject.SetActive(false);
            selectbackButton.gameObject.SetActive(false);
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
    }

    public void Reset()
    {
        selectArtifactButton.image.sprite = images.reset;
        selectbackButton.gameObject.SetActive(false);
        bt.gameObject.SetActive(false);
        RestText();
        RestMeText();

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

    public void RestMeText()
    {
        MEatiName.text = "";
        MEatiMainSul.text = "";
        powerp.text = "";
        hpp.text = "";

        foreach (var text in MEatiSul)
        {
            text.text = "";
        }

        foreach (var obj in MEaS)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void BackReset(bool re)
    {
        if(re)
        {
            selectArtifactButton.image.sprite = images.reset;
        }
        selectbackButton.gameObject.SetActive(false);
        gpanal.SetActive(true);
        bt.gameObject.SetActive(false);
        RestText();

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
    }

    public void ResetBT()
    {
        if (!gpanal.activeSelf)
        {
           // if(!selectbackButton.gameObject.activeSelf)
            selectArtifactButton.image.sprite = images.reset;

            if (dicton.myartifact != null)
            {
                upgrade.MuText();
                dicton.myartifact.Quantity += 1;
                dicton.myartifact = null;
                dicton.myArtifactId = null;
                dicton.SaveAll();
            }
            artifactManager.SaveArtifact();
            UpdateArtifactButtons();
            selectbackButton.gameObject.SetActive(false);
            gpanal.SetActive(true);
            bt.gameObject.SetActive(false);
            RestText();
            RestMeText();

            noati.SetActive(true);
            yesati.SetActive(false);

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
        }
        else
        {
            gpanal.SetActive(false);
            selectbackButton.gameObject.SetActive(true);
        }
    }

    public void UpdateArtifactButtons()
    {
        UpdateButtons(artifactss.nomal, artifactButtons.nomal, TMP.nomal, backk.nomal);
        UpdateButtons(artifactss.epic, artifactButtons.epic, TMP.epic, backk.epic);
        UpdateButtons(artifactss.unique, artifactButtons.unique, TMP.unique, backk.unique);
    }

    void UpdateButtons(Artifact[] artifacts, Button[] buttons, TextMeshProUGUI[] Tmp, GameObject[] gameObject)
    {
        for (int i = 0; i < artifacts.Length; i++)
        {
            if (artifacts[i].Quantity <= 0)
            {
                Color buttonColor = buttons[i].image.color;
                buttonColor.a = 0.5f; 
                buttons[i].image.color = buttonColor;

                buttons[i].interactable = false;
                Tmp[i].gameObject.SetActive(false);
                gameObject[i].SetActive(false);
            }
            else
            {
                Color buttonColor = buttons[i].image.color;
                buttonColor.a = 1f; 
                buttons[i].image.color = buttonColor;

                buttons[i].interactable = true;
                Tmp[i].gameObject.SetActive(true);
                Tmp[i].text = artifacts[i].Quantity.ToString();
                gameObject[i].SetActive(false);
            }
        }
        bt.gameObject.SetActive(false);
        RestText();

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
    }
}