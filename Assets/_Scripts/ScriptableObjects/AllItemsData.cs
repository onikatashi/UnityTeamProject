using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllItemsData", menuName = "Scriptable Objects/AllItemsData")]
public class AllItemsData : ScriptableObject
{
    public List<ItemData> allItems;

    [ContextMenu("ID 일괄 재배정")]
    public void ReassignAllIDs()
    {
        // 비어있는 리스트 삭제
        allItems.RemoveAll(item => item == null);

        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] != null)
            {
                allItems[i].iId = i + 1;
            }
        }
    }

}
