using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public ItemData[] Inventory;                            // 인벤토리 아이템 배열
    int itemCount = 0;                                      // 인벤토리 아이템 수

    int currentIndex = 0;                                   // 다음 아이템이 들어갈 위치 인덱스
    int lineSize = 3;                                       // 한 줄에 있는 슬롯 개수
    public SortedDictionary<Enums.ItemSynergy, int> synergyCount;        // 시너지 효과 카운트 (키: 시너지 종류, 값: 시너지 개수)
    public SortedDictionary<Enums.ItemSynergy, int> synergyActiveCount;  // 시너지 활성화 개수 (키: 시너지 종류, 값: 활성화된 시너지 개수)
    public Dictionary<int, int> reinforcedSlots;            // 강화된 슬롯 카운트 (키: 슬롯 인덱스, 값: 강화 레벨)
    Dictionary<int, int> indexByItemId;                     // 아이템 ID로 슬롯 인덱스 찾기 위한 딕셔너리
    List<int[]> lineCheck;                                  // 줄 별 시너지 효과가 완성되었는지 확인하기 위한 2차원 배열

    UIManager uiManager;
    ItemManager itemManager;

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
        synergyCount = new SortedDictionary<Enums.ItemSynergy, int>();
        synergyActiveCount = new SortedDictionary<Enums.ItemSynergy, int>();

        indexByItemId = new Dictionary<int, int>();
        GenerateLineCheck();
        InitReinforceSlots();
        InitSynergyActiveCount();

        reinforcedSlots[0] = 2;
    }

    void Start()
    {
        uiManager = UIManager.Instance;
        itemManager = ItemManager.Instance;
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

        // 아이템 시너지 카운트 업데이트
        CheckActiveSynergy();

        // 아이템 획득 시 시너지 효과 업데이트
        uiManager.synergyEffectUIController.ReturnSynergySlot();
        uiManager.synergyEffectUIController.ShowSynergyEffect();

        // currentIndex 다시 설정
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                currentIndex = i;
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

    // 인벤토리에 아이템 추가 인덱스 기반
    public void AddItemToInventoryByIndex(int index, ItemData newItem)
    {
        if (itemCount >= Inventory.Length)
        {
            Debug.Log("인벤토리 가득 참");
            return;
        }

        Inventory[index] = newItem;
        if (newItem != null)
        {
            indexByItemId[newItem.iId] = index;

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
        }

        // 아이템 시너지 카운트 업데이트
        CheckActiveSynergy();

        // 아이템 획득 시 시너지 효과 업데이트
        uiManager.synergyEffectUIController.ReturnSynergySlot();
        uiManager.synergyEffectUIController.ShowSynergyEffect();

        // 아이템 개수 증가
        itemCount++;

        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                currentIndex = i;
                break;
            }
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

        // 강화 수치도 랜덤하게 올라가고 싶으면 여기 수정
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

        indexByItemId.Remove(Inventory[slotIndex].iId);
        Inventory[slotIndex] = null;

        // 아이템 시너지 카운트 업데이트
        CheckActiveSynergy();

        // 아이템 제거 시, 시너지 효과 패널 업데이트
        uiManager.synergyEffectUIController.ReturnSynergySlot();
        uiManager.synergyEffectUIController.ShowSynergyEffect();

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

    // 시너지 빙고 확인
    public void CheckActiveSynergy()
    {
        // 재계산 전에 모든 시너지 카운트를 0으로 초기화
        var synergyTypes = synergyActiveCount.Keys.ToList();
        foreach (var type in synergyTypes)
        {
            synergyActiveCount[type] = 0;
        }

        foreach(int[] line in lineCheck)
        {
            // 첫 번째 슬롯에 아이템이 없거나, 첫 번째 아이템에 시너지가 없으면 다음 라인 확인
            if (Inventory[line[0]] == null 
                || Inventory[line[0]].iSynergy == null 
                || Inventory[line[0]].iSynergy.Count == 0)
            {
                continue;
            }

            // 첫 번째 슬롯에 있는 아이템의 모든 시너지를 HashSet으로 설정
            HashSet<Enums.ItemSynergy> checkSynergy = Inventory[line[0]].iSynergy
                .Where(x => x != Enums.ItemSynergy.None)
                .ToHashSet();

            // 시너지가 없다면 다음 라인 확인
            if (checkSynergy.Count == 0)
            {
                continue;
            }

            // 나머지 슬롯을 순회하며 교집합 업데이트
            bool isLineEmpty = false;
            for (int i = 1; i < line.Length; i++)
            {
                int index = line[i];

                // 아이템이 비어있거나 시너지가 비어있으면 실패
                if (Inventory[index] == null
                || Inventory[index].iSynergy == null
                || Inventory[index].iSynergy.Count == 0)
                {
                    isLineEmpty = true;
                    break;
                }

                // 현재 슬롯에 있는 아이템의 모든 시너지를 HashSet을 생성
                HashSet<Enums.ItemSynergy> currentItemSynergy = Inventory[index].iSynergy
                    .Where(x => x != Enums.ItemSynergy.None)
                    .ToHashSet();

                // 첫 번째 슬롯의 아이템 시너지와 교집합 연산
                checkSynergy.IntersectWith(currentItemSynergy);

                // 교집합 결과가 비어있다면, 공통 시너지 존재하지 않음
                if (checkSynergy.Count == 0)
                {
                    break;
                }
            }

            // 아이템이 비어있으면 다음 라인으로
            if (isLineEmpty) continue;

            // 남은 공통 시너지 활성화
            foreach (var synergy in checkSynergy)
            {
                synergyActiveCount[synergy]++;
            }

        }
    }

    // 인벤토리 아이템 위치 변환
    public void SwapItem(int index1, int index2)
    {
        ItemData temp = Inventory[index1];
        ItemData temp2 = Inventory[index2];

        RemoveItemFromInventory(index1);
        RemoveItemFromInventory(index2);

        AddItemToInventoryByIndex(index2, temp);
        AddItemToInventoryByIndex(index1, temp2);

        // 스왑 후 시너지 빙고 확인
        CheckActiveSynergy();

        // 인벤토리 슬롯 이미지 업데이트
        uiManager.inventoryUIController.UpdateItemIcon();
    }

    // 아이템 등급 업 => 랜덤 아이템으로
    public void RankUpItem(int index)
    {
        if (Inventory[index] == null)
        {
            Debug.Log("아무것도 없다");
            return;
        }
        if (Inventory[index].iRank == Enums.ItemRank.Legendary)
        {
            Debug.Log("이미 최고 등급");
            return;
        }

        int currentRankValue = (int)Inventory[index].iRank;
        int nextRankValue = currentRankValue + 1;
        Type rankEnumType = typeof(Enums.ItemRank);

        if (Enum.IsDefined(rankEnumType, nextRankValue))
        {
            Enums.ItemRank nextRank = (Enums.ItemRank)nextRankValue;

            if (itemManager.itemDictionaryForRank.ContainsKey(nextRank)) {

                // 현재 등급 다음 단계의 아이템 리스트를 가져옴
                List<ItemData> nextRankItemList = itemManager.itemDictionaryForRank[nextRank];

                // 해당 등급 중 랜덤한 아이템 가져옴
                int randomIndex = UnityEngine.Random.Range(0, nextRankItemList.Count);

                // 중복 아이템이 나오지 않을때까지 반복
                while (CheckDuplicateItems(nextRankItemList[randomIndex].iId))
                {
                    randomIndex = UnityEngine.Random.Range(0, nextRankItemList.Count);
                }

                RemoveItemFromInventory(index);
                AddItemToInventoryByIndex(index, nextRankItemList[randomIndex]);

                // 아이템 등급 업 후 시너지 체크
                CheckActiveSynergy();

                // 인벤토리 슬롯 이미지 업데이트
                uiManager.inventoryUIController.UpdateItemIcon();
            }
        }
    }

    // 아이템 등급 업 => 시너지 효과 유지한 채로 (시너지가 2개 이상이면 둘 중 하나라도 유지)
    public void RankUpItemWithSynergy(int index)
    {
        if (Inventory[index] == null)
        {
            Debug.Log("아무것도 없다");
            return;
        }
        if (Inventory[index].iRank == Enums.ItemRank.Legendary)
        {
            Debug.Log("이미 최고 등급");
            return;
        }

        int currentRankValue = (int)Inventory[index].iRank;
        int nextRankValue = currentRankValue + 1;
        Type rankEnumType = typeof(Enums.ItemRank);

        if (Enum.IsDefined(rankEnumType, nextRankValue))
        {
            Enums.ItemRank nextRank = (Enums.ItemRank)nextRankValue;

            if (itemManager.itemDictionaryForRank.ContainsKey(nextRank)) {

                // 현재 등급 다음 단계의 아이템 리스트를 가져옴
                List<ItemData> rankUpItemList = itemManager.itemDictionaryForRank[nextRank];

                List<HashSet<ItemData>> synergyItemList = new List<HashSet<ItemData>>();

                // 현재 인덱스의 있는 시너지를 가진 아이템 리스트를 HashSet으로 가져옴
                foreach (Enums.ItemSynergy synergy in Inventory[index].iSynergy)
                {
                    synergyItemList.Add(itemManager.itemDictionaryForSynergy[synergy].ToHashSet());
                }

                // 다음 등급의 아이템 리스트와 현재 시너지의 아이템 리스트의 교집합을 구함
                foreach (var itemList in synergyItemList)
                {
                    itemList.IntersectWith(rankUpItemList);
                }

                HashSet<ItemData> finalItemList = new HashSet<ItemData>();

                // 해당 교집합을 합하면 다음 등급의 아이템과 현재 아이템의 시너지를 이어받는 아이템 리스트가 나옴
                foreach (var itemList in synergyItemList)
                {
                    finalItemList.UnionWith(itemList);
                }

                // 최종 리스트에서 랜덤한 아이템 가져옴
                int randomIndex = UnityEngine.Random.Range(0, finalItemList.Count);

                // 중복 아이템이 나오지 않을때까지 반복
                while (CheckDuplicateItems(finalItemList.ToList()[randomIndex].iId))
                {
                    randomIndex = UnityEngine.Random.Range(0, finalItemList.Count);
                }

                RemoveItemFromInventory(index);
                AddItemToInventoryByIndex(index, finalItemList.ToList()[randomIndex]);

                // 아이템 등급 업 후 시너지 체크
                CheckActiveSynergy();

                // 인벤토리 슬롯 이미지 업데이트
                uiManager.inventoryUIController.UpdateItemIcon();
            }
        }
    }
}