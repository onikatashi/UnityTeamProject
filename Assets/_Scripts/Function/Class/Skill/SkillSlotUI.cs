using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    public Image[] slotIcons;          // SkillSlot1~3 아이콘
    public TextMeshProUGUI[] slotLevelTexts; // SkillSlot1~3 레벨 텍스트

    private PlayerSkillController skillController;

    private void Awake()
    {
        skillController = PlayerSkillController.Instance;
        skillController.OnSkillChanged += Refresh;
    }

    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// 슬롯 UI 전체 갱신
    /// </summary>
    public void Refresh()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            var skillRuntime = skillController.GetSkillAtSlot(i);

            if (skillRuntime == null)
            {
                slotIcons[i].enabled = false;
                slotLevelTexts[i].gameObject.SetActive(false);
            }
            else
            {
                slotIcons[i].enabled = true;
                slotIcons[i].sprite = skillRuntime.skillBaseData.icon;

                slotLevelTexts[i].gameObject.SetActive(true);
                slotLevelTexts[i].text = $"Lv.{skillRuntime.currentLevel}";
            }
        }
    }

    private void OnDestroy()
    {
        skillController.OnSkillChanged -= Refresh;
    }
}
