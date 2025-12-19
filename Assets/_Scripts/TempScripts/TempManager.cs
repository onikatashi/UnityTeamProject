using System.Collections.Generic;
using UnityEngine;

public class TempManager : MonoBehaviour
{
    public Stats testStat;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveLoadManager.Instance.userData.level++;
            UIManager.Instance.playerStatUIController.UpdatePlayerStatUI();
            SaveLoadManager.Instance.SaveUserData();  
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DungeonManager.Instance.ResetDungeonData();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            int randIndex = UnityEngine.Random.Range(0, InventoryManager.Instance.inventory.Length);
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
}
