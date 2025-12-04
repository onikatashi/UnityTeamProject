using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ItemData[] Inventory;                        // 인벤토리 아이템 배열
    int itemCount = 0;                                  // 인벤토리 아이템 개수

    int currentIndex = 0;                               // 현재 아이템 들어갈 위치 인덱스
    int lineSize = 3;                                   // 한 줄에 있는 슬롯 개수 
    Dictionary<Enums.ItemSynergy, int> synergeCount;    // 시너지별 개수 딕셔너리
    Dictionary<int, int> reinforcedSlots;               // 강화된 슬롯 딕셔너리 (키: 슬롯 인덱스, 값: 강화 레벨)
    List<int[]> lineCheck;                              // 한 줄에 시너지 빙고가 되어있는지 확인하기 위한 2차원 배열
      

    void Start()
    {
        Inventory = new ItemData[9];
        synergeCount = new Dictionary<Enums.ItemSynergy, int>();
        reinforcedSlots = new Dictionary<int, int>();
        GenerateLineCheck();
    }


    void Update()
    {
        
    }

    // 빙고 체크 라인 생성
    void GenerateLineCheck()
    {
        lineCheck = new List<int[]>();

        // 가로 줄 체크라인 생성
        for (int i = 0; i < lineSize; i++)
        {
            int[] row = new int[lineSize];
            for( int j = 0; j < lineSize; j++)
            {
                row[j] = i * lineSize + j;
            }
            lineCheck.Add(row);
        }

        // 세로 줄 체크라인 생성
        for (int i = 0; i < lineSize; i++)
        {
            int[] column = new int[lineSize];
            for (int j = 0; j < lineSize; j++)
            {
                column[j] = j * lineSize + i;
            }
            lineCheck.Add(column);
        }

        // 좌상 -> 우하 대각선 라인 생성
        int[] diagonal1 = new int[lineSize];
        for(int i = 0; i < lineSize; i++)
        {
            diagonal1[i] = i * (lineSize + 1);
        }
        lineCheck.Add(diagonal1);

        // 우상 -> 좌하 대각선 라인 생성
        int[] diagonal2 = new int[lineSize];
        for(int i = 0; i < lineSize; i++)
        {
            diagonal2[i] = (i + 1) * (lineSize - 1);
        }
        lineCheck.Add(diagonal2);
    }

    // 인벤토리에 아이템 추가
    public void AddItemToInventory(ItemData newItem)
    {
        if(itemCount >= Inventory.Length)
        {
            Debug.Log("인벤토리 꽉참");
            return;
        }

        Inventory[currentIndex] = newItem;
        currentIndex++;
        itemCount++;
        if(currentIndex >= Inventory.Length)
        {
            currentIndex--; 
        }
        UIManager.Instance.inventoryUIController.UpdateItemIcon();
    }
}
