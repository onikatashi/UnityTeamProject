using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SynergySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image synergyIcon;                       // 시너지 아이콘
    public TextMeshProUGUI synergyName;             // 시너지 이름
    public TextMeshProUGUI activeSynergyCount;      // 활성화된 시너지 수 / 최대 활성화 수

    public void SetUp(Sprite synergyIcon, string synergyName, int activeCount, int maxCount)
    {
        this.synergyIcon.sprite = synergyIcon;
        this.synergyName.text = synergyName;
        activeSynergyCount.text = activeCount.ToString() + " / " + maxCount.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
