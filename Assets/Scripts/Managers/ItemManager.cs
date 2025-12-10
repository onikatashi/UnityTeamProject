using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 1. 모든 아이템의 정보를 가지고 있는 매니저
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    // 모든 아이템 정보를 가지고 있는 SO를 이용
    [SerializeField]
    AllItemsData allItemDatas;

    // 아이템 데이터를 ID로 편하게 찾기위한 Dictionary
    Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();
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

    void Start()
    {
        // 아이템 딕셔너리  채워주기
        itemDictionary = allItemDatas.allItems.ToDictionary(x => x.iId, x => x);
    }

    // ItemId를 통한 ItemData 반환
    public ItemData GetItemData(int itemId)
    {
        if( !itemDictionary.TryGetValue(itemId, out var data))
        {
            Debug.LogError("ItemData를 찾을 수 없음");
            return null;
        }
        return data;
    }
}
