using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionUIController : MonoBehaviour
{
    public GameObject itemDescriptionPanel;             // 아이템 설명 패널 오브젝트
    public TextMeshProUGUI itemNameText;                // 아이템 이름 텍스트
    public TextMeshProUGUI itemSynergyText;             // 아이템 시너지 텍스트
    public TextMeshProUGUI itemStatDescriptionText;     // 아이템 스탯 설명 텍스트
    public TextMeshProUGUI itemDescriptionText;         // 아이템 설명 텍스트
    public Image itemIcon;                              // 아이템 아이콘 이미지
    public Image synergeIcon;                           // 시너지 아이콘 이미지
    public Button discardItemButton;                    // 아이템 버리기 버튼

    InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        discardItemButton.onClick.AddListener(DiscardItem);
    }

    public void ShowItemDescription(ItemData itemData)
    {
        if(itemData == null)
        {
            HideItemDescription();
            return;
        }

        itemDescriptionPanel.SetActive(true);
        itemNameText.text = itemData.iName;
        itemSynergyText.text = itemData.iSynergy.ToString();
        itemStatDescriptionText.text = itemData.iStatDescription;
        itemDescriptionText.text = itemData.iDescription;
        itemIcon.sprite = itemData.iIcon;
        // 시너지 아이콘 설정은 추후에 아이콘 리소스가 준비되면 구현
    }

    public void HideItemDescription()
    {
        itemDescriptionPanel.SetActive(false);
    }

    public void DiscardItem()
    {
        // 인벤토리에서 remove 생각이 아니라 이걸 버튼에 연결하면 될듯
    }
}
