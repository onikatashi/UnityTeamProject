using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 카드 하나 UI
/// </summary>
public class SkillCardUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Button button;

    private SkillCardData cardData;
    private Action<SkillCardData> onClickAction;

    public void SetUp(SkillCardData data, Action<SkillCardData> onClick)
    {
        cardData = data;
        onClickAction = onClick;

        iconImage.sprite = data.icon;
        nameText.text = data.nameText;
        descriptionText.text = data.descriptionText;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        onClickAction?.Invoke(cardData);
    }
}
