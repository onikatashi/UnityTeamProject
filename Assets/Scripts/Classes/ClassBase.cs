using UnityEngine;

public abstract class ClassBase : ScriptableObject
{
    [Header("직업 이름")]
    public string className;

    [Header("해당 직업이 가지고 있는 스킬 목록")]
    public SkillBase[] classSkills;

    /// <summary>
    /// 직업마다 기본 공격이 다르므로 override 필요
    /// </summary>
    /// <param name="player"></param>
    public abstract void BasicAttack(Player player,LayerMask monsterLayer);
}
