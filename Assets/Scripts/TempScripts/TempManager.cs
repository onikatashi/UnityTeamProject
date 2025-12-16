using System;
using UnityEngine;

public class TempManager : MonoBehaviour
{
    public Stats testStat;
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

        if (Input.GetKeyDown(KeyCode.X))
        {
            ModeManager.Instance.SetMode(Enums.InventoryMode.None);
            Debug.Log("일반모드");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ModeManager.Instance.SetMode(Enums.InventoryMode.Swap);
            UIManager.Instance.ToggleInventory();
            UIManager.Instance.ToggleInventory();
            Debug.Log("스왑모드");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            ModeManager.Instance.SetMode(Enums.InventoryMode.RankUp);
            UIManager.Instance.ToggleInventory();
            UIManager.Instance.ToggleInventory();
            Debug.Log("강화모드");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ModeManager.Instance.SetMode(Enums.InventoryMode.RanKUpWithSynergy);
            UIManager.Instance.ToggleInventory();
            UIManager.Instance.ToggleInventory();
            Debug.Log("시너지 유지 강화모드");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            testStat = InventoryManager.Instance.GetInventoryTotalStats();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            SoundManager.Instance.PlayBGM("town");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SoundManager.Instance.Play("fire");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SoundManager.Instance.StopBGM();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneLoaderManager.Instance.LoadScene(SceneNames.Town);
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
