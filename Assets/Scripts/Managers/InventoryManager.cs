using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

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
        Inventory = new ItemData[9];
    }

<<<<<<< Updated upstream
    public List<Item> Inventory;

    void Start()
    {
        
=======
    public ItemData[] Inventory;                         // 인벤토리 아이템 배열
    int itemCount = 0;                                   // 인벤토리 아이템 수

    int currentIndex = 0;                                // 다음 아이템이 들어갈 위치 인덱스
    int lineSize = 3;                                    // 한 줄에 있는 슬롯 개수
    Dictionary<Enums.ItemSynergy, int> synergeCount;     // 시너지 효과 카운트
    public Dictionary<int, int> reinforcedSlots;         // 강화된 슬롯 카운트 (키: 슬롯 인덱스, 값: 강화 레벨)
    List<int[]> lineCheck;                               // 줄 별 시너지 효과가 완성되었는지 확인하기 위한 2차원 배열

    void Start()
    {
        synergeCount = new Dictionary<Enums.ItemSynergy, int>();
        InitReinforceSlots();
        GenerateLineCheck();

        reinforcedSlots[0] = 2;
>>>>>>> Stashed changes
    }


    void Update()
    {
        
    }
}
