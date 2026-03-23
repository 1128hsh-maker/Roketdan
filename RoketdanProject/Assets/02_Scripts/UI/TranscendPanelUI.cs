using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranscendPanelUI : MonoBehaviour
{
    [SerializeField] private BoardInteractionController controller;
    [SerializeField] private GameObject root;

    [Header("Buttons")]
    [SerializeField] private Button optionAButton;
    [SerializeField] private Button optionBButton;
    [SerializeField] private Button optionCButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text optionAText;
    [SerializeField] private TMP_Text optionBText;
    [SerializeField] private TMP_Text optionCText;
    [SerializeField] private TMP_Text titleText;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (optionAButton != null)
            optionAButton.onClick.AddListener(controller.OnClickTranscendOptionA);

        if (optionBButton != null)
            optionBButton.onClick.AddListener(controller.OnClickTranscendOptionB);

        if (optionCButton != null)
            optionCButton.onClick.AddListener(controller.OnClickTranscendOptionC);

        Hide();
    }

    public void Show(HeroInstance selectedHero, HeroData optionA, HeroData optionB, HeroData optionC, int transcendCost)
    {
        if (root != null)
            root.SetActive(true);

        if (titleText != null)
        {
            string heroName = selectedHero != null && selectedHero.Data != null ? selectedHero.Data.heroId : "Hero";
            titleText.text = $"{heroName} Transcend ({transcendCost} Mineral)";
        }

        SetOption(optionAButton, optionAText, optionA, transcendCost);
        SetOption(optionBButton, optionBText, optionB, transcendCost);
        SetOption(optionCButton, optionCText, optionC, transcendCost);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void SetOption(Button button, TMP_Text text, HeroData data, int cost)
    {
        bool active = data != null;

        if (button != null)
            button.gameObject.SetActive(active);

        if (text != null)
        {
            text.text = active ? $"{data.heroId}\nG{data.grade}\n({cost} Mineral)" : string.Empty;
        }
    }
}
