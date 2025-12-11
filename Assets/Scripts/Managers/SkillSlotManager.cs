using UnityEngine;

/// <summary>
/// 슬롯 (1-3) 키를 눌렀을 때 어떤 스킬을 실행할 지 관리하는 매니저
/// "어떤 스킬인지"는 SkillBase / ScriptableObject가 들고있고,
/// "그 스킬을 언제/어떤 슬롯에서 실행할지"를 관리하는 매니저
/// </summary>
public class SkillSlotManager : MonoBehaviour
{
    //싱글톤
    public static SkillSlotManager Instance;

    //3개의 스킬 슬롯
    public SkillBase[] skillSlots = new SkillBase[3];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    ///// <summary>
    /////>>>>>>>>>>>>>>>>>> 나중에 스킬UI 스크립트에 넣을 함수 <<<<<<<<<<<<<<<<<<<<
    ///// </summary>
    ///// <param name="selectedSkill"></param>
    //public void PlayerChoseSkill(SkillBase selectedSkill)
    //{
    //    AddSkill(selectedSkill);
    //}

    public void AddSkill(SkillBase selectedSkill)
    {
       
        for (int i = 0; i < skillSlots.Length; i++)
        {
            //빈 슬롯 찾기
            if (skillSlots[i] == null)
            {
                //selectedSkill은 asset 원본이므로 복사본 생성 << 이래야 스킬 초기화 간편해짐
                SkillBase instance = Instantiate(selectedSkill);

                //초기 레벨 설정
                instance.level = 1;

                //슬롯에 넣어주기
                skillSlots[i] = instance;
                skillSlots[i].ResetRuntime();

                Debug.Log($"스킬 {instance.skillName} 이(가) {i}번 슬롯에 장착됨");
                return;
            }

        }
        // 2) 꽉 찼으면 → 기존 스킬 중 하나 업그레이드 UI로 이동
        //OpenLevelUpUI(selectedSkill);
    }

    /// <summary>
    /// 특정 슬롯에 있는 스킬 사용
    /// </summary>
    /// <param name="slotIndex"></param>
    public void UseSkill(int slotIndex)
    {
        //인덱스 범위 벗어나면 리턴
        if (slotIndex < 0 || slotIndex >= skillSlots.Length) return;

        SkillBase skill = skillSlots[slotIndex];

        //해당 슬롯에 스킬 없으면 리턴
        if (skill == null) return;

        //스킬 실행
        skill.Execute(Player.Instance);
    }

    /// <summary>
    /// 던전이 끝나고 마을로 돌아올 때
    /// 배운 스킬 전부 초기화 해주는 함수
    /// </summary>
    public void ResetSkills()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i] = null;
        }
    }
}
