using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Localization;

public class CollectBox : MonoBehaviour
{
    public int Four;
    public TextMeshProUGUI BoxCountText;
    public TextMeshProUGUI MoneyText;
    public Holomoney holomoney;
    private PlayerData data;
    public GameObject nobox;
    public GameObject boxGacha;
    public GameObject GachaE;
    public Dicton[] towerInfos;
    public Canvas canvas;
    public Artifactss artifactss;

    public GameObject ghache;
    public List<GameObject> rewardDisplayPositions;
    public List<TextMeshProUGUI> rewardTexts;
    public List<TextMeshProUGUI> rewardSubTexts;
    public List<GameObject> oldde;
    public GameObject Manimage;
    public GameObject Artimage;
    public GameObject Gacha1;
    public float ms;

    public List<Image> Ro; 
    public Sprite[] ros;

    private GameObject instantiatedBoxGacha;
    private List<GameObject> instantiatedRewards = new List<GameObject>();
    public ArtifactManager artifactManager;

    [System.Serializable]
    public class Artifactss
    {
        public Artifact[] nomal;
        public Artifact[] epic;
        public Artifact[] unique;
    }

    public float boxP;
    public float boxU;
    public float boxS;
    public float boxA_C;
    public float boxA_R;
    public float boxA_U;

    void Start()
    {
        data = holomoney.LoadPlayerData();
        MoneyText.text = "" + data.holomoney;
        artifactManager.LoadArtifact();
        UpdateBoxCountText();
    }

    public void OnOpenBoxButtonClicked()
    {
        if (data.BoxCount > 0)
        {
            DestroyAllInstantiatedRewards();

            foreach (var ro in Ro)
            {
                ro.gameObject.SetActive(false);
            }

            foreach (var old in oldde)
            {
                old.gameObject.SetActive(false);
            }

            foreach (var rewardtexts in rewardTexts)
            {
                rewardtexts.text = "";
            }

            foreach (var rewardsubTexts in rewardSubTexts)
            {
                rewardsubTexts.text = "";
            }

            instantiatedBoxGacha = Instantiate(boxGacha, transform.position, transform.rotation);
            instantiatedBoxGacha.transform.SetParent(ghache.transform, false);
            instantiatedBoxGacha.transform.localPosition = Vector3.zero; UISD uiSD = instantiatedBoxGacha.GetComponent<UISD>();
            if (uiSD != null)
            {
                uiSD.collectBox = this;
            }

            data.BoxCount--;
            (string reward, string subText) = GetRandomBoxReward(0);
            if (reward == "HoloMoney")
            {
                int holoMoneyAmount = int.Parse(subText);
                data.holomoney += holoMoneyAmount;
                DisplayReward(0, reward, subText, false, true);
            }
            UpdateBoxCountText();
            holomoney.SavePlayerData(data);
            MoneyText.text = "" + data.holomoney;
        }
        else
        {
            ShowInsufficientBoxPopup();
        }
    }

    public void DrawFiveTimes()
    {
        if (data.BoxCount > Four) // 4
        {
            DestroyAllInstantiatedRewards();
            data.BoxCount -= 5;
            UpdateBoxCountText();
            holomoney.SavePlayerData(data);

            foreach (var ro in Ro)
            {
                ro.gameObject.SetActive(false);
            }

            foreach (var old in oldde)
            {
                old.gameObject.SetActive(false);
            }


            foreach (var rewardtexts in rewardTexts)
            {
                rewardtexts.text = "";
            }

            foreach (var rewardsubTexts in rewardSubTexts)
            {
                rewardsubTexts.text = "";
            }

            instantiatedBoxGacha = Instantiate(boxGacha, transform.position, transform.rotation);
            instantiatedBoxGacha.transform.SetParent(ghache.transform, false);
            instantiatedBoxGacha.transform.localPosition = Vector3.zero; UISD uiSD = instantiatedBoxGacha.GetComponent<UISD>();
            if (uiSD != null)
            {
                uiSD.collectBox = this;
            }

            for (int i = 0; i < 5; i++)
            {
                (string reward, string subText) = GetRandomBoxReward(i);
                if (reward == "HoloMoney")
                {
                    int holoMoneyAmount = int.Parse(subText);
                    data.holomoney += holoMoneyAmount;
                    DisplayReward(i, reward, subText, false, true);
                }
                holomoney.SavePlayerData(data);
                MoneyText.text = "" + data.holomoney;
            }
        }
        else
        {
            ShowInsufficientBoxPopup();
        }
    }

    private void DestroyAllInstantiatedRewards()
    {
        foreach (GameObject reward in instantiatedRewards)
        {
            if (reward != null)
            {
                Destroy(reward);
            }
        }
        instantiatedRewards.Clear();
    }

    private (string, string) GetRandomBoxReward(int index)
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);

        if (randomValue <= boxP) //0.7f
        {
            float holoMoneyRandomValue = UnityEngine.Random.Range(0f, 1f);
            if (holoMoneyRandomValue <= 0.70f)
            {
                SpawnGachaE(index, 0);
                ShowRewardPrefab(Manimage, index);
                return ("HoloMoney", "25");
            }
            else if (holoMoneyRandomValue <= 0.85f)
            {
                SpawnGachaE(index, 0);
                ShowRewardPrefab(Manimage, index);
                return ("HoloMoney", "50");
            }
            else if (holoMoneyRandomValue <= 0.95f)
            {
                SpawnGachaE(index, 0);
                ShowRewardPrefab(Manimage, index);
                return ("HoloMoney", "100");
            }
            else if (holoMoneyRandomValue <= 0.99f)
            {
                SpawnGachaE(index, 0);
                ShowRewardPrefab(Manimage, index);
                return ("HoloMoney", "150");
            }
            else
            {
                SpawnGachaE(index, 1);
                ShowRewardPrefab(Manimage, index);
                return ("HoloMoney", "400");
            }
        }
        else if (randomValue <= boxU + boxP) // 0.06f 
        {
            SpawnGachaE(index, 1);
            return UnlockRandomTower(index);
        }
        else if (randomValue <= boxS + boxU + boxP) // 0.01
        {
            SpawnGachaE(index, 1);
            return UnlockRandomSkin(index);
        }
        else if (randomValue <= boxA_C + boxS + boxU + boxP) // 0.15
        {
            SpawnGachaE(index, 0);
            return UnlockRandomArti(index, 0);
        }
        else if (randomValue <= boxA_R + boxA_C + boxS + boxU + boxP) // 0.075
        {
            SpawnGachaE(index, 1);
            return UnlockRandomArti(index, 1);
        }
        /*else // 0.005 
        {
            SpawnGachaE(index, 2);
            return UnlockRandomArti(index, 2);
        }*/
        else if (randomValue <= boxA_U + boxA_R + boxA_C + boxS + boxU + boxP) // 0.005 
        {
            SpawnGachaE(index, 2);
            return UnlockRandomArti(index, 2);
        }
        else
        {
            SpawnGachaE(index, 2);
            return UnlockUniqueRandomTower(index);
        }
    }

    private (string, string) UnlockRandomTower(int index)
    {
        int randomIndex = UnityEngine.Random.Range(5, towerInfos.Length - 2);
        Dicton selectedTower = towerInfos[randomIndex];
        
        Ro[index].gameObject.SetActive(true);
        Ro[index].sprite = ros[0];

        if (selectedTower.isPurchasable)
        {
            selectedTower.isPurchasable = false;
            ShowRewardPrefab(selectedTower.originprefab, index);
            DisplayReward(index, selectedTower.descriptionKey, selectedTower.rolePlay, false, false);
            selectedTower.SaveAll();
            return (selectedTower.name, selectedTower.rolePlay);
        }
        else
        {
            oldde[index].gameObject.SetActive(true);
            ShowRewardPrefab(selectedTower.originprefab, index);
            DisplayReward(index, "HoloMoney", (selectedTower.MyPrice * ms).ToString(), false, true);
            return ("HoloMoney", (selectedTower.MyPrice * ms).ToString());
        }
    }

    private (string, string) UnlockUniqueRandomTower(int index)
    {
        int randomIndex = UnityEngine.Random.Range(towerInfos.Length - 2, towerInfos.Length);
        Dicton selectedTower = towerInfos[randomIndex];

        Ro[index].gameObject.SetActive(true);
        Ro[index].sprite = ros[0];

        if (selectedTower.isPurchasable)
        {
            selectedTower.isPurchasable = false;
            ShowRewardPrefab(selectedTower.originprefab, index);
            DisplayReward(index, selectedTower.descriptionKey, selectedTower.rolePlay, false, false);
            selectedTower.SaveAll();
            return (selectedTower.name, selectedTower.rolePlay);
        }
        else
        {
            oldde[index].gameObject.SetActive(true);
            ShowRewardPrefab(selectedTower.originprefab, index);
            DisplayReward(index, "HoloMoney", (selectedTower.MyPrice * ms).ToString(), false, true);
            return ("HoloMoney", (selectedTower.MyPrice * ms).ToString());
        }
    }

    private (string, string) UnlockRandomSkin(int index)
    {
        var validTowers = towerInfos.Where(tower => tower.SkinList.Length > 1).ToArray();

        int randomTowerIndex = UnityEngine.Random.Range(0, validTowers.Length);
        Dicton selectedTower = validTowers[randomTowerIndex];

        int randomSkinIndex = UnityEngine.Random.Range(1, selectedTower.SkinList.Length);
        Dicton.SkinData selectedSkin = selectedTower.SkinList[randomSkinIndex];

        Ro[index].gameObject.SetActive(true);
        Ro[index].sprite = ros[1];

        if (selectedSkin.isUnlocked)
        {
            selectedSkin.isUnlocked = false;
            ShowRewardPrefab(selectedSkin.skinPrefab, index);
            selectedTower.SaveAll();
            DisplayReward(index, selectedSkin.skinKey, selectedTower.descriptionKey, true, false);
            return (selectedSkin.skinName, selectedTower.name);
        }
        else
        {
            oldde[index].gameObject.SetActive(true);
            ShowRewardPrefab(selectedSkin.skinPrefab, index);
            DisplayReward(index, "HoloMoney", (selectedSkin.price * ms).ToString(), false, true);
            return ("HoloMoney", (selectedSkin.price * ms).ToString());
        }
    }

    private (string, string) UnlockRandomArti(int index, int rado)
    {
        int randomIndex = 0;
        string meg = null;
        string megg = null;
        string dol = null;

        Artifact selectedArti = null;

        switch (rado)
        {
            case 0:
                randomIndex = UnityEngine.Random.Range(0, artifactss.nomal.Length);
                selectedArti = artifactss.nomal[randomIndex];
                meg = "c";
                megg = "common";
                break;
            case 1:
                randomIndex = UnityEngine.Random.Range(0, artifactss.epic.Length);
                selectedArti = artifactss.epic[randomIndex];
                meg = "r";
                megg = "rare";
                break;
            case 2:
                randomIndex = UnityEngine.Random.Range(0, artifactss.unique.Length);
                selectedArti = artifactss.unique[randomIndex];
                meg = "u";
                megg = "unique";
                break;
        }

        var localizedItemPN = new LocalizedString { TableReference = "UI", TableEntryReference = "Artifact-" + megg };

        localizedItemPN.StringChanged += (string localizedText) =>
        {
            dol = localizedText;
        };

        var localizedString = new LocalizedString { TableReference = "Item_artifact", TableEntryReference = "name_" + meg + "_p" + (randomIndex + 1) };

        localizedString.StringChanged += (string localizedText) =>
        {
            string name = localizedText;
            DisplayReward(index, name, dol, false, true);
            ShowRewardAtiPrefab(Artimage, index, selectedArti.sprit);
        };
        selectedArti.Quantity += 1;
        artifactManager.SaveArtifact();
        return (name, dol);
    }

    private void ShowRewardPrefab(GameObject prefab, int index)
    {
        if (prefab != null && index < rewardDisplayPositions.Count)
        {
            GameObject instantiatedReward = Instantiate(prefab, rewardDisplayPositions[index].transform.position, Quaternion.identity, rewardDisplayPositions[index].transform);
            if(index == 0)
            {
                instantiatedReward.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            }
            else
            {
                instantiatedReward.transform.localScale = new Vector3(1, 1, 1);
            }
            instantiatedRewards.Add(instantiatedReward);
        }
    }

    private void ShowRewardAtiPrefab(GameObject prefab, int index, Sprite sprite)
    {
        if (prefab != null && index < rewardDisplayPositions.Count)
        {
            Vector3 positionWithOffset = rewardDisplayPositions[index].transform.position;
            positionWithOffset.y += 1.8f;

            GameObject instantiatedReward = Instantiate(prefab, positionWithOffset, Quaternion.identity, rewardDisplayPositions[index].transform);

            Image imageComponent = instantiatedReward.GetComponent<Image>();
            if (imageComponent != null && sprite != null)
            {
                imageComponent.sprite = sprite;
            }

            if (index == 0)
            {
                instantiatedReward.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            }
            else
            {
                instantiatedReward.transform.localScale = new Vector3(1, 1, 1);
            }
            instantiatedRewards.Add(instantiatedReward);
        }
    }

    private void SpawnGachaE(int index, int Jrado)
    {
        if (index < rewardDisplayPositions.Count)
        {
            GameObject gachaEInstance = Instantiate(GachaE, rewardDisplayPositions[index].transform.position, Quaternion.identity, rewardDisplayPositions[index].transform);

            var skeletonGraphic = gachaEInstance.GetComponent<Spine.Unity.SkeletonGraphic>();

            if (skeletonGraphic != null)
            {
                var animationState = skeletonGraphic.AnimationState;

                switch (Jrado)
                {
                    case 0:
                        animationState.SetAnimation(0, "light_normal", true);
                        break;
                    case 1:
                        animationState.SetAnimation(0, "light_rare", true);
                        break;
                    case 2:
                        animationState.SetAnimation(0, "light_unique", true);
                        break;
                }
            }

            instantiatedRewards.Add(gachaEInstance);
        }
    }

    public void Shimaee()
    {
        Gacha1.SetActive(false);

        if (instantiatedBoxGacha != null)
        {
            Destroy(instantiatedBoxGacha);
            instantiatedBoxGacha = null;
        }
    }

    private void DisplayReward(int index, string key, string subKey, bool isSkin, bool isM)
    {
        if (index < rewardTexts.Count && index < rewardSubTexts.Count)
        {
            if (isM)
            {
                rewardTexts[index].text = key;
                rewardSubTexts[index].text = subKey;
            }
            else
            {
                if (isSkin)
                {
                    var localizedSString = new LocalizedString { TableReference = "Item_skin_name", TableEntryReference = key };
                    localizedSString.StringChanged += (string localizedSTexts) =>
                    {
                        rewardTexts[index].text = localizedSTexts;
                    };

                    var localizedString = new LocalizedString { TableReference = "Item_unit_name", TableEntryReference = subKey };
                    localizedString.StringChanged += (string localizedTextss) =>
                    {
                        rewardSubTexts[index].text = localizedTextss;
                    };
                }
                else
                {
                    var localizedString = new LocalizedString { TableReference = "Item_unit_name", TableEntryReference = key };
                    localizedString.StringChanged += (string localizedTextt) =>
                    {
                        rewardTexts[index].text = localizedTextt;
                    };

                    string skey;

                    switch (subKey)
                    {
                        case "어택커":
                            skey = "Unit-Role2";
                            break;
                        case "투사체":
                            skey = "Unit-Role1";
                            break;
                        case "서포터":
                            skey = "Unit-Role3";
                            break;
                        default:
                            skey = null;
                            break;
                    }

                    var localizedRString = new LocalizedString { TableReference = "UI", TableEntryReference = skey };
                    localizedRString.StringChanged += (string localizedTexttt) =>
                    {
                        rewardSubTexts[index].text = localizedTexttt;
                    };
                }
            }
        }
    }

    private void UpdateBoxCountText()
    {
        BoxCountText.text = data.BoxCount.ToString();
    }

    private void ShowInsufficientBoxPopup()
    {
        nobox.SetActive(true);
    }
}
