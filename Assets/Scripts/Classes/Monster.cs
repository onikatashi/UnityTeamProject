using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 1. 몬스터 스탯 (체력, 공격력, 방어력 등)
/// 2. 몬스터 행동 (이동, 공격 등)
/// 3. 몬스터 애니메이션 제어
/// 4. 몬스터 드롭 경험치 및 골드
/// 5. 몬스터 행동 패턴 (후순위)
/// </summary>
public class Monster : MonoBehaviour
{
    public GameObject player;
    MonsterData md;
    Enums.MonsterState state;
    Animator anim;
    CharacterController cc;

    public float currentHp = 100f;
    public float detectRange = 10f;
    float timer = 0f;

    NavMeshAgent agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.updateRotation = true;
        state = Enums.MonsterState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case Enums.MonsterState.Idle:
                Idle();
                break;
            case Enums.MonsterState.Move:
                Move();
                break;
            case Enums.MonsterState.Attack:
                Attack();
                break;
            case Enums.MonsterState.Die:
                Die();
                break;
        }
    }

    private void Idle()
    {
        
    }

    private void Move()
    {
        
    }

    private void Attack()
    {

    }
    private void Die()
    {

    }
}
