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
    public int slotIndex;                               // 현재 선택된 슬롯 인덱스

    InventoryManager inventoryManager;
    UIManager uiManager;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        uiManager = UIManager.Instance;

        // 버리기 버튼 클릭 이벤트 등록
        discardItemButton.onClick.AddListener( () =>
        {
            inventoryManager.RemoveItemFromInventory(slotIndex);
            HideItemDescription();
        });
    }

    // 아이템 설명 패널 활성화
    public void ShowItemDescription(ItemData itemData)
    {
        if(itemData == null)
        {
            HideItemDescription();
            return;
        }

        itemDescriptionPanel.SetActive(true);
        itemNameText.text = itemData.iName;

        switch (itemData.iRank)
        {
            case Enums.ItemRank.Common:
                itemNameText.color = Color.white;
                break;
            case Enums.ItemRank.Rare:
                itemNameText.color = Color.blue;
                break;
            case Enums.ItemRank.Unique:
                itemNameText.color = new Color(128f / 255f, 0f, 128f / 255f, 1f);
                break;
            case Enums.ItemRank.Legendary:
                itemNameText.color = new Color(1f, 165f / 255f, 0f, 1f);
                break;
        }

        itemSynergyText.text = itemData.iSynergy.ToString();
        itemStatDescriptionText.text = itemData.iStatDescription;
        itemDescriptionText.text = itemData.iDescription;
        itemIcon.sprite = itemData.iIcon;
        // 시너지 아이콘 설정은 추후에 아이콘 리소스가 준비되면 구현
    }

    // 아이템 설명 패널 비활성화
    public void HideItemDescription()
    {
        itemDescriptionPanel.SetActive(false);
    }
}
