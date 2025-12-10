using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public ItemData[] Inventory;                            // 인벤토리 아이템 배열
    int itemCount = 0;                                      // 인벤토리 아이템 수

    int currentIndex = 0;                                   // 다음 아이템이 들어갈 위치 인덱스
    int lineSize = 3;                                       // 한 줄에 있는 슬롯 개수
    public Dictionary<Enums.ItemSynergy, int> synergyCount;        // 시너지 효과 카운트 (키: 시너지 종류, 값: 시너지 개수)
    public Dictionary<Enums.ItemSynergy, int> synergyActiveCount;  // 시너지 활성화 개수 (키: 시너지 종류, 값: 활성화된 시너지 개수)
    public Dictionary<int, int> reinforcedSlots;            // 강화된 슬롯 카운트 (키: 슬롯 인덱스, 값: 강화 레벨)
    Dictionary<int, int> indexByItemId;                     // 아이템 ID로 슬롯 인덱스 찾기 위한 딕셔너리
    List<int[]> lineCheck;                                  // 줄 별 시너지 효과가 완성되었는지 확인하기 위한 2차원 배열

    UIManager uiManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Inventory = new ItemData[9];
        synergyCount = new Dictionary<Enums.ItemSynergy, int>();
        synergyActiveCount = new Dictionary<Enums.ItemSynergy, int>();
        indexByItemId = new Dictionary<int, int>();
        GenerateLineCheck();
        InitReinforceSlots();
        InitSynergyActiveCount();

        reinforcedSlots[0] = 2;
    }

    void Start()
    {
        uiManager = UIManager.Instance;
    }

    // 강화슬롯 초기화
    void InitReinforceSlots()
    {
        reinforcedSlots = new Dictionary<int, int>();
        for (int i = 0; i< Inventory.Length; i++)
        {
            reinforcedSlots[i] = 0;
        }
    }

    // 시너지 활성화 딕셔너리 초기화
    void InitSynergyActiveCount()
    {
        // 미리 모든 아이템 시너지를 Key로 만들어둠
        // 아이템 효과 중 무작위 시너지 효과 레벨 +1 등이 생길 때 사용하기 위해서
        foreach (Enums.ItemSynergy type in Enum.GetValues(typeof(Enums.ItemSynergy)))
        {
            synergyActiveCount[type] = 0;
        }

    }

    // 라인 체크 배열 생성
    void GenerateLineCheck()
    {
        lineCheck = new List<int[]>();

        // 가로 줄 체크 배열 생성
        for (int i = 0; i < lineSize; i++)
        {
            int[] row = new int[lineSize];
            for (int j = 0; j < lineSize; j++)
            {
                row[j] = i * lineSize + j;
            }
            lineCheck.Add(row);
        }

        // 세로 줄 체크 배열 생성
        for (int i = 0; i < lineSize; i++)
        {
            int[] column = new int[lineSize];
            for (int j = 0; j < lineSize; j++)
            {
                column[j] = j * lineSize + i;
            }
            lineCheck.Add(column);
        }

        // 좌상 -> 우하 대각선 배열 생성
        int[] diagonal1 = new int[lineSize];
        for (int i = 0; i < lineSize; i++)
        {
            diagonal1[i] = i * (lineSize + 1);
        }
        lineCheck.Add(diagonal1);

        // 우상 -> 좌하 대각선 배열 생성
        int[] diagonal2 = new int[lineSize];
        for (int i = 0; i < lineSize; i++)
        {
            diagonal2[i] = (i + 1) * (lineSize - 1);
        }
        lineCheck.Add(diagonal2);
    }

    // 인벤토리에 아이템 추가
    public void AddItemToInventory(ItemData newItem)
    {
        if (itemCount >= Inventory.Length)
        {
            Debug.Log("인벤토리 가득 참");
            return;
        }

        if(CheckDuplicateItems(newItem.iId))
        {
            Debug.Log("이미 인벤토리에 있는 아이템");
            return;
        }

        Inventory[currentIndex] = newItem;
        indexByItemId[newItem.iId] = currentIndex;

        // 아이템 추가할 때, 해당 아이템의 시너지를 저장
        for (int i = 0; i < newItem.iSynergy.Count; i++)
        {
            if (newItem.iSynergy[i] != Enums.ItemSynergy.None)
            {
                if (!synergyCount.ContainsKey(newItem.iSynergy[i]))
                {
                    synergyCount[newItem.iSynergy[i]] = 0;
                }
                synergyCount[newItem.iSynergy[i]]++;
            }
        }

        uiManager.synergyEffectUIController.ReturnSynergySlot();
        uiManager.synergyEffectUIController.ShowSynergyEffect();

        // currentIndex 가 Inventory.Length 를 넘지 않도록 처리
        // Remove 했을 때, 중간 아이템이 비는 슬롯에 아이템 추가하도록 처리
        while (true)
        {
            if(currentIndex + 1 <= Inventory.Length -1 && Inventory[currentIndex + 1] != null)
            {
                currentIndex++;
            }
            else
            {
                currentIndex++;
                break;
            }
        }
        
        // 아이템 개수 증가
        itemCount++;

        if (currentIndex >= Inventory.Length)
        {
            currentIndex--;
        }
        uiManager.inventoryUIController.UpdateItemIcon();
    }

    // 인벤토리 아이템 최종 스탯 반환
    public Stats GetInventoryTotalStats()
    {
        Stats totalStats = new Stats();
        for(int i = 0; i< Inventory.Length; i++)
        {
            var itemData = Inventory[i];
            if(itemData != null)
            {
                if (reinforcedSlots.ContainsKey(i) && Inventory[i] != null)
                {
                    totalStats += itemData.iBaseStat + itemData.iBonusStat * reinforcedSlots[i];
                }
            }
        }
        return totalStats;
    }

    // 인벤토리 슬롯 강화
    public void ReinforceInventorySlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= Inventory.Length)
        {
            Debug.LogError("인벤토리 슬롯 범위를 벗어남");
            return;
        }

        reinforcedSlots[slotIndex]++;
        uiManager.inventoryUIController.UpdateItemIcon();
    }

    // 인벤토리 아이템 제거 (버리기 버튼을 통해 호출)
    public void RemoveItemFromInventory(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= Inventory.Length)
        {
            Debug.LogError("인벤토리 슬롯 범위를 벗어남");
            return;
        }
        if (Inventory[slotIndex] == null)
        {
            Debug.LogWarning("해당 슬롯에 아이템이 없음");
            return;
        }

        // 시너지 개수 카운트 줄여주고 0이면 제거
        for (int i = 0; i < Inventory[slotIndex].iSynergy.Count; i++)
        {
            if (Inventory[slotIndex].iSynergy[i] != Enums.ItemSynergy.None)
            {
                if (synergyCount.ContainsKey(Inventory[slotIndex].iSynergy[i]))
                {
                    synergyCount[Inventory[slotIndex].iSynergy[i]]--;
                }
                if (synergyCount[Inventory[slotIndex].iSynergy[i]] == 0)
                {
                    synergyCount.Remove(Inventory[slotIndex].iSynergy[i]);
                }
            }
        }

        uiManager.synergyEffectUIController.ReturnSynergySlot();
        uiManager.synergyEffectUIController.ShowSynergyEffect();

        indexByItemId.Remove(Inventory[slotIndex].iId);
        Inventory[slotIndex] = null;
        itemCount--;

        if(currentIndex >= slotIndex)
        {
            currentIndex = slotIndex;
        }
        uiManager.inventoryUIController.UpdateItemIcon();
    }

    // 아이템 중복 체크
    public bool CheckDuplicateItems(int itemId)
    {
        if (indexByItemId.ContainsKey(itemId)){
            return true;
        }
        else
        {
            return false;
        }
    }
}