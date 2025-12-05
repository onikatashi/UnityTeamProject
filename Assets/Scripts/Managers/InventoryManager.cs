using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

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
    }

    public ItemData[] Inventory;                         // 인벤토리 아이템 배열
    int itemCount = 0;                                   // 인벤토리 아이템 수

    int currentIndex = 0;                                // 다음 아이템이 들어갈 위치 인덱스
    int lineSize = 3;                                    // 한 줄에 있는 슬롯 개수
    Dictionary<Enums.ItemSynergy, int> synergeCount;     // 시너지 효과 카운트
    Dictionary<int, int> reinforcedSlots;                // 강화된 슬롯 카운트 (키: 슬롯 인덱스, 값: 강화 레벨)
    List<int[]> lineCheck;                               // 줄 별 시너지 효과가 완성되었는지 확인하기 위한 2차원 배열

    void Start()
    {
        Inventory = new ItemData[9];
        synergeCount = new Dictionary<Enums.ItemSynergy, int>();
        InitReinforceSlots();
        GenerateLineCheck();

        reinforcedSlots[0] = 2;
    }


    void Update()
    {

    }

    void InitReinforceSlots()
    {
        reinforcedSlots = new Dictionary<int, int>();
        for (int i = 0; i< Inventory.Length; i++)
        {
            reinforcedSlots[i] = 0;
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

        Inventory[currentIndex] = newItem;
        currentIndex++;
        itemCount++;
        if (currentIndex >= Inventory.Length)
        {
            currentIndex--;
        }
        UIManager.Instance.inventoryUIController.UpdateItemIcon();
    }

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
        Debug.Log(totalStats.maxHp);
        return totalStats;
    }
}