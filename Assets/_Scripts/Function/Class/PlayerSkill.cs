using UnityEngine;
using UnityEngine.InputSystem.Layouts;

/// <summary>
/// 스킬 실행 스크립트
/// 나중에 패시브 느낌으로 사용되는 스킬 나오면 여기서 처리하기
/// </summary>
public class PlayerSkill : MonoBehaviour
{
    public Skill_Meteor meteor;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SkillSlotManager.Instance.UseSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SkillSlotManager.Instance.UseSkill(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SkillSlotManager.Instance.UseSkill(2);

        if (Input.GetKeyDown(KeyCode.Space)) SkillSlotManager.Instance.AddSkill(meteor);
    }
}
