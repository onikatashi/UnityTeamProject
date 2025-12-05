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
        }
        
        if(Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.ToggleInventory();
        }
    }
}
