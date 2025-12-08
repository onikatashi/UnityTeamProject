using System;
using UnityEngine;

public class TempManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        testGetItem();

        if(Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            int randIndex = UnityEngine.Random.Range(0, InventoryManager.Instance.Inventory.Length);
            InventoryManager.Instance.ReinforceInventorySlot(randIndex);
        }
    }

    void testGetItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(1));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(2));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(3));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(4));
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(6));
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(7));
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(8));
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(9));
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(10));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(11));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(12));
        }
    }
}
