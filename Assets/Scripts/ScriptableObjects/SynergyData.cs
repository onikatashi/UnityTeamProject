using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynergyData", menuName = "Scriptable Objects/SynergyData")]
public class SynergyData : ScriptableObject
{
    public Sprite synergyIcon;                  // 시너지 아이콘
    public Enums.ItemSynergy synergyType;       // 시너지 타입
    public string synergyName;                  // 시너지 명칭
    public List<SynergyTier> tiers;             // 시너지 활성화 개수에 따른 추가 능력치
}

[System.Serializable]
public class SynergyTier
{
    public int RequiredLines;

    public Stats BonusStats;
}