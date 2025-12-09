using System.Collections;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    /*
     * 스킬 뭐만들지
     * 1전방향 공격?
     * 2검기 날리기?
     * 3대시처럼 이동기 쓰면서 공격?
     * 4기본공격보다 넓은 범위 finaldmg*2 공격?
     * 5아니면 걍 버프?
    */

    // 스킬 레벨업 시 수치 증가를 해야함 << 공식 적어서 하면 될 듯
    // 스킬 슬롯을 만들어야 될듯 , 스킬 얻으면 0번 부터 채워지게
    // 그다음 UseSkill 에 SkillSlot 1 번에 있는 스킬을 사용하게 만들어야 함

    //플레이어 스크립트 인스턴스용
    Player player;

    public LayerMask monsterLayer;

    //스킬 중복 사용 금지용 (쿨타임 포함)
    bool canUse1 = true;
    bool canUse2 = true;
    bool canUse3 = true;
    float skill1CoolTime = 7f;
    float skill2CoolTime = 5f;
    float skill3CoolTime = 20f;

    //코루틴 용 값 저장 변수들
    float currentAtkDmg;

    private void Awake()
    {
        player = Player.Instance;
    }

    private void Update()
    {
        UseSkill();
    }

    /// <summary>
    /// 스킬 1,2,3번으로 해당 번호에 있는 스킬 사용하기
    /// </summary>
    void UseSkill()
    {
        // 스킬 슬롯에 스킬이 있을 때 << 조건 추가해야 함.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //스킬 슬롯에 있는 스킬을 읽어서 사용
            //근데 일단은 그거 배제하고 적용해보자.
            if (canUse1) Skill1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (canUse2) Skill2();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Skill3();
        }
    }


    /// <summary>
    /// 전방향 공격 3회 코루틴 실행
    /// </summary>
    public void Skill1()
    {
        StartCoroutine(Skill1Coroutine());
    }

    /// <summary>
    /// 스킬1 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Skill1Coroutine()
    {
        //쿨타임용 (사용금지)
        canUse1 = false;
        float atkDmg = player.finalStats.attackDamage;
        float atkRng = player.finalStats.attackRange;

        for (int i = 0; i < 3; i++)
        {
            //시작, 0.4초마다 몬스터 범위안에 있는지 찾아주고
            Collider[] monsters = Physics.OverlapSphere(transform.position, player.finalStats.attackRange, monsterLayer);
            foreach (Collider c in monsters)
            {

                MonsterBase monster = c.GetComponent<MonsterBase>();

                if (monster != null)
                {
                    monster.TakeDamage(atkDmg);
                }
            }

            yield return new WaitForSeconds(0.4f);      //0.4초마다 몬스터 찾아서 TakeDamage하기.
        }

        yield return new WaitForSeconds(skill1CoolTime);
        canUse1 = true;
    }

    /// <summary>
    /// 공격력 2배 버프 ( 일단 5초 )
    /// </summary>
    void Skill2()
    {
        canUse2 = false;
        currentAtkDmg = player.finalStats.attackDamage;
        player.finalStats.attackDamage *= 2f;
        StartCoroutine(Skill2CoolTime());
    }
    IEnumerator Skill2CoolTime()
    {
        yield return new WaitForSeconds(skill2CoolTime);
        player.finalStats.attackDamage = currentAtkDmg;
        canUse2 = true;
    }

    /// <summary>
    /// 플레이어 최대체력 1/2 회복
    /// </summary>
    void Skill3()
    {
        if (player.currentHp < player.finalStats.maxHp)
        {
            canUse3 = false;
            player.Heal(player.finalStats.maxHp / 2);
            StartCoroutine(Skill3CoolTime());
        }
        else return;
    }

    IEnumerator Skill3CoolTime()
    {
        yield return new WaitForSeconds(skill3CoolTime);
        canUse3 = true;
    }
}
