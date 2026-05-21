using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;

public class CategoryButtonManager : MonoBehaviour
{
    public List<Button> buttons; 
    public List<GameObject> windows;

    public UnlockM unlockM;
    public Collection collection;

    private Dictionary<Button, GameObject> buttonToWindowMap;
    private Button currentButton;

    public SkeletonGraphic skeletonGraphic;

    public GameObject wd1;
    public GameObject wd2;

    void Start()
    {
        buttonToWindowMap = new Dictionary<Button, GameObject>();

        for (int i = 0; i < buttons.Count; i++)
        {
            Button button = buttons[i];
            GameObject window = windows[i];

            buttonToWindowMap[button] = window;

            int index = i;
            button.onClick.AddListener(() => OnButtonClickedWrapper(button, index));

            if (i == 0)
            {
                unlockM.UpdateButtonT();
            }
            else if(i == 1)
            {
                unlockM.UpdateButtonM();
            }
        }

        if (buttons.Count > 0)
        {
            OnButtonClicked(buttons[0], 0);
        }
    }

    void OnButtonClickedWrapper(Button button, int index)
    {
        OnButtonClicked(button, index);
    }

    void OnButtonClicked(Button clickedButton, int index)
    {
        if (clickedButton == currentButton)
        {
            return;
        }

        if (currentButton != null)
        {
            currentButton.interactable = true;
        }

        currentButton = clickedButton;
        currentButton.interactable = false;

        foreach (Button button in buttons)
        {
            buttonToWindowMap[button].SetActive(false);
            button.GetComponent<Image>().color = Color.gray;
        }

        GameObject windowToActivate = buttonToWindowMap[clickedButton];
        windowToActivate.SetActive(true);
        clickedButton.GetComponent<Image>().color = Color.white;

        collection.ResetInfo();
        if (index == 0)
        {
            collection.ResetPage(false);
        }
        else
        {
            collection.ResetPage(true);
        }
        collection.Col();

        if (index == 2)
        {
            wd1.SetActive(false);
            wd2.SetActive(false);
            skeletonGraphic.AnimationState.ClearTracks();
            skeletonGraphic.AnimationState.SetAnimation(0, "wide", false);
        }
        else
        {
            wd1.SetActive(true);
            wd2.SetActive(true);
        }
    }
}
