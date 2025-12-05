using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
<<<<<<< Updated upstream
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
=======
    public GameObject inventoryPanel;                           // 인벤토리 패널 오브젝트 (인벤토리 부모 오브젝트)
    public GameObject slotPrefab;                               // 인벤토리 슬롯 프리팹
    public ItemDescriptionUIController descriptionUIController; // 아이템 설명 UI 컨트롤러

    public List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();


    InventoryManager inventoryManager;
    UIManager uiManager;

    // InventoryUIController 초기화
    public void InitInventoryUIController()
    {
        inventoryManager = InventoryManager.Instance;
        uiManager = UIManager.Instance;

        ClearAndCreateSlots();
        UpdateItemIcon();
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        
    }
<<<<<<< Updated upstream
}
=======

    // 슬롯 생성
    void ClearAndCreateSlots()
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        Debug.Log(inventoryManager);
        Debug.Log(inventoryManager.Inventory.Length);
        for (int i = 0; i < inventoryManager.Inventory.Length; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, inventoryPanel.transform);
            newSlot.name = $"Slot_{i}";

            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                slotUI.slotIndex = i;
                slotUI.SetUp(i, OnSlotClicked);
                inventorySlots.Add(slotUI);
            }
            else
            {
                Debug.LogError("InventoryUIController: 슬롯 프리팹에 InventorySlotUI 컴포넌트가 없습니다.");
            }
        }
    }

    public void OnSlotClicked(int slotIndex)
    {
        ItemData itemData = inventoryManager.Inventory[slotIndex];

        ItemDescriptionUIController descriptionUI = uiManager.itemDescriptionUIController;

        if(itemData != null)
        {
            descriptionUI.ShowItemDescription(itemData);
        }
        else
        {
            descriptionUI.HideItemDescription();
        }
    }


    // 인벤토리 슬롯 아이콘 업데이트
    public void UpdateItemIcon()
    {
        for (int i = 0; i < inventoryManager.Inventory.Length; i++)
        {
            if (inventoryManager.Inventory[i] != null)
            {
                inventorySlots[i].itemIcon.sprite = inventoryManager.Inventory[i].iIcon;
                inventorySlots[i].reinforceLevel.text = "+" + inventoryManager.reinforcedSlots[i].ToString();
            }
            else
            {
                inventorySlots[i].itemIcon.sprite = null;
                inventorySlots[i].reinforceLevel.text = null;
            }
        }
    }
}
>>>>>>> Stashed changes
