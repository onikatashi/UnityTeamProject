using UnityEngine;

/// <summary>
/// 1. 스탯 기본 정보들
/// </summary>
[System.Serializable]
public class Stats
{
    public float maxHp;                // 최대 체력
    public float maxMp;                // 최대 마나
    public float mpRegen;              // 마나 재생 속도
    public float maxDashCount;         // 최대 대시 횟수
    public float dashRegen;            // 대시 재생 속도
    public float dashDistance;         // 대시 거리
    public float attackDamage;         // 공격력
    public float attackRange;          // 공격 범위
    public float attackSpeed;          // 공격 속도
    public float projectileCount;      // 투사체 개수
    public float projectileSpeed;      // 투사체 속도
    public float moveSpeed;            // 이동 속도
    public float criticalRate;         // 치명타 확률 (criChance)
    public float criticalDamage;       // 치명타 피해 (criDamage)
    public float shield;               // 보호막

    // 이건 미리 정해 놓은게 아니라 상의 해봐야함
    public float cooldownReduction;    // 쿨타임 감소
}
