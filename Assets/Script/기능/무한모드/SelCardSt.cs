using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class SelCardSt : MonoBehaviour
{
    public GameObject ME;
    public GameObject btsPal;
    public GameObject[] backbts;
    public Button[] bts;
    public UpdatePanel updatePanel;
    private int Selin;
    public GameObject CardAPanel;
    public InfiniteStage infiniteStage;
    public SelAbilityCard abilityCard;
    private int ph, pp, pc, pcc;
    public TextMeshProUGUI[] uptex;
    public bool test = false;
    public SkeletonGraphic UpAnimation;

    public void Ran()
    {
        foreach (Button button in bts)
        {
            button.gameObject.SetActive(true);
        }

        foreach (GameObject bt in backbts)
        {
            bt.gameObject.SetActive(false);
        }

        UpAnimation.AnimationState.ClearTracks(); 
        UpAnimation.AnimationState.SetAnimation(0, "animation", false);
    }

    public void YesMBT(int i)
    {
        foreach (GameObject bt in backbts)
        {
            bt.gameObject.SetActive(false);
        }

        Selin = i;
        backbts[i].gameObject.SetActive(true);
    }

    private void ShuffleButtons()
    {
        List<Transform> buttonTransforms = new List<Transform>();
        foreach (Transform child in btsPal.transform)
        {
            buttonTransforms.Add(child);
        }

        for (int i = buttonTransforms.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Transform temp = buttonTransforms[i];
            buttonTransforms[i] = buttonTransforms[j];
            buttonTransforms[j] = temp;
        }

        for (int i = 0; i < buttonTransforms.Count; i++)
        {
            buttonTransforms[i].SetSiblingIndex(i);
        }
    }

    public void YEUP()
    {
        bool allInactive = true;
        foreach (GameObject bt in backbts)
        {
            if (bt.activeSelf)
            {
                allInactive = false;
                break;
            }
        }

        if (allInactive) { return; }

        switch (Selin)
        {
            case 0:
                PP();
                break;
            case 1:
                PS();
                break;
            case 2:
                PH();
                break;
            case 3:
                MC();
                break;
        }

        if(infiniteStage.mainStage == 1 || infiniteStage.mainStage == 5)
        {
            ME.gameObject.SetActive(false);
            abilityCard.Fir();
            CardAPanel.gameObject.SetActive(true);
            uptower();
            uptext();
        }
        else
        {
            Time.timeScale = GameManager.Instance.SPPD;
            ME.gameObject.SetActive(false);
            uptower();
            uptext();
            infiniteStage.coru();
        }
    }

    public void PH()
    {
        for (int i = 0; i < updatePanel.AllTowers.Count; i++)
        {
            if (updatePanel.AllTowers[i] != null)
            {
                updatePanel.AllTowers[i].inPlusStat.IpHp += 1;
            }
        }
        ph = ph + 30;
    }

    public void PP()
    {
        for (int i = 0; i < updatePanel.AllTowers.Count; i++)
        {
            if (updatePanel.AllTowers[i] != null)
            {
                updatePanel.AllTowers[i].inPlusStat.IpPower += 0.05f;
            }
        }
        pp = pp + 5;
    }

    public void PS()
    {
        for (int i = 0; i < updatePanel.AllTowers.Count; i++)
        {
            if (updatePanel.AllTowers[i] != null)
            {
                updatePanel.AllTowers[i].inPlusStat.IpSpeed += 0.04f;
            }
        }
        pc = pc + 4;
    }

    public void MC()
    {
        for (int i = 0; i < updatePanel.AllTowers.Count; i++)
        {
            if (updatePanel.AllTowers[i] != null)
            {
                updatePanel.AllTowers[i].inPlusStat.Imcool += 0.05f;
            }
        }
        pcc = pcc + 5;
    }

    private void uptext()
    {
        uptex[0].text = "+ " + pp + "%";
        uptex[1].text = "+ " + pc + "%";
        uptex[2].text = "+ " + ph;
        uptex[3].text = "- " + pcc + "%";
    }

    public void uptower()
    {
        for (int i = 0; i < updatePanel.AllTowers.Count; i++)
        {
            if (updatePanel.AllTowers[i] != null)
            {
                updatePanel.AllTowers[i].myStatInt(true);
            }
        }
    }
}
