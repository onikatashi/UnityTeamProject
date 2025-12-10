using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public GameObject InventoryPanel;               // 인벤토리 패널 오브젝트
    public GameObject slotPrefab;                   // 슬롯 프리팹
    public List<InventorySlotUI> inventorySlots;    // 인벤토리 슬롯 UI 리스트

    InventoryManager inventoryManager;
    UIManager uiManager;

    // InventoryUIController 초기화
    public void InitInventoryUIController()
    {
        inventoryManager = InventoryManager.Instance;
        uiManager = UIManager.Instance;

        ClearAndCreateSlots();
        UpdateItemIcon();
    }

    // 슬롯 생성 및 초기화 (콜백 함수 연결)
    void ClearAndCreateSlots()
    {
        foreach(Transform child in InventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        // 슬롯 생성
        for (int i = 0; i < inventoryManager.Inventory.Length; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, InventoryPanel.transform);
            newSlot.name = $"Slot_{i}";

            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();

            slotUI.SetUp(i);
            inventorySlots.Add(slotUI);
        }
    }

    // 슬롯 클릭 이벤트
    //public void OnSlotClicked(int slotIndex)
    //{

    //}

    // 인벤토리 슬롯 아이콘 업데이트
    public void UpdateItemIcon()
    {
        for (int i = 0; i < inventoryManager.Inventory.Length; i++)
        {
            // 나중에 reinforcedSlots을 얻는 함수로 바꿔도 될듯
            inventorySlots[i].reinforceLevel.text = "+" + inventoryManager.reinforcedSlots[i].ToString();
            if (inventoryManager.Inventory[i] != null)
            {
                inventorySlots[i].itemIcon.sprite = inventoryManager.Inventory[i].iIcon;
            }
            else
            {
                inventorySlots[i].itemIcon.sprite = null;
            }
        }
    }
}