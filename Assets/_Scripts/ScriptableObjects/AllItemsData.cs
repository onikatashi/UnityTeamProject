using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllItemsData", menuName = "Scriptable Objects/AllItemsData")]
public class AllItemsData : ScriptableObject
{
    public List<ItemData> allItems;
}
