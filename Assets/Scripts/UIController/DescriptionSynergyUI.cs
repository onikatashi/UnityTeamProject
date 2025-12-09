using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DescriptionSynergyUI : MonoBehaviour, IPointerClickHandler
{
    public Image synergyIcon;
    public TextMeshProUGUI synergyName;

    public void SetUp(Sprite synergyIcon, string synergyName)
    {
        this.synergyIcon.sprite = synergyIcon;
        this.synergyName.text = synergyName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
            // Å¬¸¯ ¾Æ´Ò ¶§ »ý°¢ÇØºÁ¶ó
    }
}
