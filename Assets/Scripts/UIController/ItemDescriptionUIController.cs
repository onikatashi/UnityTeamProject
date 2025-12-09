using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionUIController : MonoBehaviour
{
    public GameObject itemDescriptionPanel;             // 아이템 설명 패널 오브젝트
    public TextMeshProUGUI itemNameText;                // 아이템 이름 텍스트
    public TextMeshProUGUI itemStatDescriptionText;     // 아이템 스탯 설명 텍스트
    public TextMeshProUGUI itemDescriptionText;         // 아이템 설명 텍스트
    public Image itemIcon;                              // 아이템 아이콘 이미지
    public Button discardItemButton;                    // 아이템 버리기 버튼
    public int slotIndex;                               // 현재 선택된 슬롯 인덱스

    public Transform synergyPanel;                      // 시너지 아이콘 텍스트 프리팹의 부모 패널
    public DescriptionSynergyUI synergyUI;              // 시너지 아이콘과 텍스트 프리팹

    InventoryManager inventoryManager;
    SynergyManager synergyManager;
    PoolManager poolManager;
    UIManager uiManager;

    private void Awake()
    {
        inventoryManager = InventoryManager.Instance;
        synergyManager = SynergyManager.Instance;
        poolManager = PoolManager.Instance;
        uiManager = UIManager.Instance;

        // 설명창의 시너지 설명 오브젝트 풀 생성
        poolManager.CreatePool(Enums.PoolType.DescriptionSynergy, synergyUI, 2, synergyPanel);
    }
    private void Start()
    {
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


        for (int i = 0; i < itemData.iSynergy.Count; i++)
        {
            DescriptionSynergyUI icon = poolManager.Get<DescriptionSynergyUI>(Enums.PoolType.DescriptionSynergy);
            icon.SetUp(synergyManager.GetSynergyData(itemData.iSynergy[i]).synergyIcon,
                synergyManager.GetSynergyData(itemData.iSynergy[i]).synergyName);
        }
        
        itemStatDescriptionText.text = itemData.iStatDescription;
        itemDescriptionText.text = itemData.iDescription;
        itemIcon.sprite = itemData.iIcon;

    }

    // 아이템 설명 패널 비활성화
    public void HideItemDescription()
    {
        poolManager.Return(Enums.PoolType.DescriptionSynergy, synergyUI);
        itemDescriptionPanel.SetActive(false);
    }
}
