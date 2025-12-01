using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 1. 아이템 데이터를 저장하는 SO
/// 2. 아이템 이름, 설명, 효과, 능력치 등을 포함
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public int iId;                         // 아이템 고유 ID
    public string iName;                    // 아이템 이름
    public string iDescription;             // 아이템 설명
    public Enums.ItemRank iRank;            // 아이템 등급
    public Enums.ItemSynergy iSynergy;      // 아이템 시너지
    public Enums.ItemEffect iEffect;        // 아이템 추가 효과
    public float iPrice;                    // 아이템 가격(골드)

    // 아이템 능력치
    public Stats iBaseStat;                 // 아이템 능력치

    // 아이템 강화 보너스 스탯
    // 원래는 extra였지만 bonus로 바꿈. 이거 얘기 후에 결정
    public float iBonusAttackDamage;        // 강화 보너스 공격력
    public float iBonusRange;               // 공격 추적 범위 ?
    public float iBonusAttackRange;         // 강화 보너스 공격 범위
    public float iBonusAttackSpeed;         // 강화 보너스 공격 속도
    public float iBonusProjectileSpeed;     // 강화 보너스 투사체 속도

}
