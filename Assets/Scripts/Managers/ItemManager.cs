using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. 모든 아이템의 정보를 가지고 있는 매니저
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
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

    // 모든 아이템 데이터 (직접 넣어줌)
    [SerializeField]
    List<ItemData> allItemDatas;
    
    // 아이템 데이터를 ID로 편하게 찾기위한 Dictionary
    Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 아이템 딕셔너리  채워주기
        foreach (ItemData itemdata in allItemDatas)
        {
            if(itemdata == null)
            {
                continue;
            }
            itemDictionary[itemdata.iId] = itemdata;
        }
    }

    // Item 객체 생성
    public ItemData CreateItemInstance(int itemId)
    {
        if( !itemDictionary.TryGetValue(itemId, out var data))
        {
            Debug.LogError("ItemData를 찾을 수 없음");
            return null;
        }
        return data;
    }
}
