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
        if(Input.GetKeyDown(KeyCode.Q))
        {
            InventoryManager.Instance.AddItemToInventory(ItemManager.Instance.CreateItemInstance(1));
            InventoryManager.Instance.GetInventoryTotalStats();
        }
        
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
}
