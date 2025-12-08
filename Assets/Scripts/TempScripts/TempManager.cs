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
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.GetItemData(i + 1));
            }
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
