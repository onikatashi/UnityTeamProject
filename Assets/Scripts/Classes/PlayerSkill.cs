using JetBrains.Annotations;
using Mono.Cecil.Cil;
using System.Collections;
using System.Threading;
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
    public LayerMask monsterLayer;

    float timer = 0f;

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
            Skill1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Skill2();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
    }


    /// <summary>
    /// 전방향 공격 3회
    /// </summary>
    public void Skill1()
    {
        Collider[] monsters = Physics.OverlapSphere(transform.position, Player.Instance.finalStats.attackRange, monsterLayer);
        float atkDmg = Player.Instance.finalStats.attackDamage;
        float atkRng = Player.Instance.finalStats.attackRange;
        foreach (Collider c in monsters)
        {

            Monster monster = c.GetComponent<Monster>();

            if (monster != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    monster.TakeDamage(atkDmg);
                    StartCoroutine(Skill1CoolTime());
                }
            }
        }
    }
    IEnumerator Skill1CoolTime()
    {
        yield return new WaitForSeconds(0.2f);
    }

    /// <summary>
    /// 공격력 2배 버프 ( 일단 5초 )
    /// </summary>
    void Skill2()
    {
        float currentAtkDmg = Player.Instance.finalStats.attackDamage;

        Player.Instance.finalStats.attackDamage *= 2f;

        timer += Time.deltaTime;
        if( timer >= 5f)
        {
            Player.Instance.finalStats.attackDamage /= 2f;
            timer = 0f;
        }
    }

}
