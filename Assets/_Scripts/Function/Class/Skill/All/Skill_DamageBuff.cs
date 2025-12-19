using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skills/DamageBuff")]
public class Skill_DamageBuff : SkillBase
{
    [Header("Damage Buff Settings")]
    [Tooltip("레벨 1 기준 공격력 배율 (예: 1.2 = 20% 증가)")]
    public float baseMultiplier = 1.2f;

    [Tooltip("레벨당 추가 배율 증가량")]
    public float multiplierPerLevel = 0.1f;

    [Tooltip("버프 지속 시간 (초)")]
    public float duration = 5f;

    public override void Execute(Player player, int skillLevel)
    {
        // 레벨 기반 최종 배율 계산
        float finalMultiplier = GetFinalMultiplier(skillLevel);

        // 곱연산 버프용 Stats 생성 (기본값 1)
        Stats mulStats = Stats.CreateMultiplierDefault();
        mulStats.attackDamage = finalMultiplier;

        // Player에게 곱연산 버프 적용 요청
        player.ApplyMultiplicativeBuff(mulStats, duration);
    }

    private float GetFinalMultiplier(int level)
    {
        return baseMultiplier + multiplierPerLevel * (level - 1);
    }
}
