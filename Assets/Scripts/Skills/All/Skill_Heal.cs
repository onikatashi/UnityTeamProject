using System.Collections;
using UnityEngine;

/// <summary>
/// 최대 체력 기준 회복 스킬
/// </summary>
[CreateAssetMenu(menuName = "Skills/Heal")]
public class Skill_Heal : SkillBase
{
    public float baseHeal = 5f;

    public override void Execute(Player player)
    {
        if (!CanUse()) return;

        lastUseTime = Time.realtimeSinceStartup;

        //만약 현재체력이 최대체력보다 적으면, 한대라도 맞았으면
        if(player.currentHp < player.finalStats.maxHp)
        {
            //최대체력 기준 몇퍼센트를 회복
            player.currentHp += player.finalStats.maxHp / baseHeal;

            //근데 만약 현재체력이 최대체력보다 커진다면
            if(player.currentHp >= player.finalStats.maxHp)
            {
                //최대체력으로 현재체력을 깎기
                player.currentHp = player.finalStats.maxHp;
            }
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();

        //baseHeal 감소, 체력량 증가
        baseHeal -= 0.5f;
    }
}
