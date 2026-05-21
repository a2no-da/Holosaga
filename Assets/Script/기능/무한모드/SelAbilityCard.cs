using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using Spine.Unity;

public class SelAbilityCard : MonoBehaviour
{
    public GameObject ME;
    public AbilityCard[] abilityCard;
    public Image[] cardIm;
    public Cards[] cards;
    public GameObject[] backbts;
    private int Selin;
    public UpdatePanel updatePanel;
    private List<AbilityCard> selectedAbilityCards = new List<AbilityCard>();
    public Image[] cardImage;
    public SelCardSt selCardSt;
    public InfiniteStage infiniteStage;
    private HashSet<int> excludedCardIndices = new HashSet<int>();
    public SkeletonGraphic UpAnimation;

    [System.Serializable]
    public class Cards
    {
        public Image WaGGu;
        public TextMeshProUGUI name;
        public TextMeshProUGUI Sul;
    }

    public void Fir()
    {
        foreach (GameObject bt in backbts)
        {
            bt.gameObject.SetActive(false);
        }

        selectedAbilityCards.Clear();

        List<AbilityCard> selectedCards = new List<AbilityCard>();
        HashSet<int> selectedIndices = new HashSet<int>();

        while (selectedCards.Count < 3 && selectedCards.Count < abilityCard.Length)
        {
            int randomIndex = Random.Range(0, abilityCard.Length);
            if (!selectedIndices.Contains(randomIndex) && !excludedCardIndices.Contains(abilityCard[randomIndex].index))
            {
                selectedIndices.Add(randomIndex);
                selectedCards.Add(abilityCard[randomIndex]);
                selectedAbilityCards.Add(abilityCard[randomIndex]);
            }
        }

        for (int i = 0; i < selectedCards.Count; i++)
        {
            if (i < cards.Length)
            {
                var localizedString = new LocalizedString { TableReference = "Item_Card", TableEntryReference = "name_ak_p" + selectedCards[i].index };
                localizedString.StringChanged += (string localizedText) =>
                {
                    cards[i].name.text = localizedText;
                };


                var localizedItemPN = new LocalizedString { TableReference = "Item_Card", TableEntryReference = "desc_ak_p" + selectedCards[i].index };

                localizedItemPN.StringChanged += (string localizedText) =>
                {
                    cards[i].Sul.text = localizedText;
                };


                cardIm[i].GetComponent<Image>().sprite = selectedCards[i].sprit;
            }
        }
        UpAnimation.AnimationState.ClearTracks(); 
        UpAnimation.AnimationState.SetAnimation(0, "animation", false);
    }

    public void YesABT(int i)
    {
        foreach (GameObject bt in backbts)
        {
            bt.gameObject.SetActive(false);
        }

        Selin = i;
        backbts[i].gameObject.SetActive(true);
    }

    public void YEUP()
    {
        bool allInactive = true;
        bool allneedrole = true;
        foreach (GameObject bt in backbts)
        {
            if (bt.activeSelf)
            {
                allInactive = false;
                break;
            }
        }

        if (!allInactive)
        {
            int activeTowersCount = 0;

            foreach (var tower in updatePanel.AllTowers)
            {
                if (!tower.isEx)
                {
                    activeTowersCount++;
                }
            }
            if ((selectedAbilityCards[Selin].needtower == 1 && activeTowersCount != 1) ||
                (selectedAbilityCards[Selin].needtower == 2 && activeTowersCount < 1))
            {
                allInactive = true;
            }

            switch (selectedAbilityCards[Selin].needtype)
            {
                case NeedType.None:
                    allneedrole = false;
                    break;
                case NeedType.범위공격:
                    foreach (var tower in updatePanel.AllTowers)
                    {
                        if (tower != null && !tower.isEx)
                        {
                            var role = tower.GetComponent<Tower>().dicton.role;
                            if (role == DictonRole.Attacker)
                            {
                                allneedrole = false;
                                break;
                            }
                        }
                    }
                    break;
                case NeedType.투사체:
                    foreach (var tower in updatePanel.AllTowers)
                    {
                        if (tower != null && !tower.isEx)
                        {
                            var role = tower.GetComponent<Tower>().dicton.role;
                            if (role == DictonRole.Projectiler)
                            {
                                allneedrole = false;
                                break;
                            }
                        }
                    }
                    break;
                case NeedType.서포터:
                    foreach (var tower in updatePanel.AllTowers)
                    {
                        if (tower != null && !tower.isEx)
                        {
                            var role = tower.GetComponent<Tower>().dicton.role;
                            if (role == DictonRole.Supporter)
                            {
                                allneedrole = false;
                                break;
                            }
                        }
                    }

                    break;
            }

            //if (allneedrole) { return; }

            if (!allneedrole && !allInactive)
            {
                for (int i = 0; i < updatePanel.AllTowers.Count; i++)
                {
                    if (updatePanel.AllTowers[i] != null)
                    {
                        if (!updatePanel.AllTowers[i].isEx) 
                        {
                            if (selectedAbilityCards[Selin].index == 5)
                            {
                                var role = updatePanel.AllTowers[i].GetComponent<Tower>().dicton.role;
                                if (role == DictonRole.Attacker)
                                {
                                    selectedAbilityCards[Selin].abilityFunction.FunctionCard(updatePanel.AllTowers[i]);
                                }
                            }
                            else
                            {
                                selectedAbilityCards[Selin].abilityFunction.FunctionCard(updatePanel.AllTowers[i]);
                            }

                            if (updatePanel.AllTowers[i].abilityCards[0] == null)
                            {
                                updatePanel.AllTowers[i].abilityCards[0] = selectedAbilityCards[Selin];
                            }
                            else
                            {
                                updatePanel.AllTowers[i].abilityCards[1] = selectedAbilityCards[Selin];
                            }
                        }
                    }
                }

                selectedAbilityCards[Selin].abilityFunction.FunctionWorldCard();

                int activeTowerCount = 0;

                foreach (var tower in updatePanel.AllTowers)
                {
                    if (tower != null && !tower.isEx)
                    {
                        activeTowerCount++;
                    }
                }

                if (selectedAbilityCards[Selin].needtower <= activeTowerCount)
                {
                    int count = 0; 

                    for (int i = 0; i < updatePanel.AllTowers.Count; i++)
                    {
                        if (updatePanel.AllTowers[i] != null && !updatePanel.AllTowers[i].isEx)
                        {
                            selectedAbilityCards[Selin].abilityFunction.FunctionDesCard(updatePanel.AllTowers[i]);
                            count++;

                            if (count >= selectedAbilityCards[Selin].needtower)
                            {
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < cardImage.Length; i++)
                {
                    if (!cardImage[i].gameObject.activeSelf)
                    {
                        cardImage[i].gameObject.SetActive(true);
                        cardImage[i].GetComponent<Image>().sprite = selectedAbilityCards[Selin].sprit;
                        break; 
                    }
                }

                excludedCardIndices.Add(selectedAbilityCards[Selin].index);
                Time.timeScale = GameManager.Instance.SPPD;
                ME.gameObject.SetActive(false);
                infiniteStage.coru();
            }
            else
            {
                excludedCardIndices.Add(selectedAbilityCards[Selin].index);
                Time.timeScale = GameManager.Instance.SPPD;
                ME.gameObject.SetActive(false);
                infiniteStage.coru();
            }
        }
    }
}
