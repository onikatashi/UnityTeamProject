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
public abstract class MonsterBase : MonoBehaviour
{
    [Header("공통 참조")]
    public GameObject player;
    public MonsterData md;
    
    protected Enums.MonsterState state;
    protected Animator anim;
    protected NavMeshAgent agent;

    [Header("공통 스탯")]
    public float currentHp = 100f;
    public float detectRange = 10f;

    public MonsterProjectile bulletPrefab;
    public Transform firepoint;

    PoolManager poolManager;
    public bool isDef = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        state = Enums.MonsterState.Idle;

        if(agent != null)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
        }

        player = Player.Instance.gameObject;
    }

    private void Start()
    {
        poolManager = PoolManager.Instance;
        if (bulletPrefab != null && firepoint != null)
        {
            poolManager.CreatePool<MonsterProjectile>(Enums.PoolType.MonsterProjectile, bulletPrefab, 5, null);
        }
        
    }

    protected virtual void Update()
    {
        if (isDef) return;

        switch (state)
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

    protected abstract void Idle();
    protected abstract void Move();
    protected abstract void Attack();
    protected virtual void Die()
    {
        //agent.isStopped = true;
        agent.enabled = false;
        Destroy(gameObject, 4f);
        //anim.SetTrigger("Die");
    }

    public virtual void TakeDamage(float dmg)
    {
        BuffReceiver buff = GetComponent<BuffReceiver>();
        if (buff != null)
        {
            dmg *= buff.DamageTakenMultiplier;
        }
        currentHp -= dmg;
        if (currentHp <= 0 && state != Enums.MonsterState.Die)
        {
            state = Enums.MonsterState.Die;
            Die();
        }
    }

    public virtual MonsterProjectile GetMonsterProjectile()
    {
        return poolManager.Get<MonsterProjectile>(Enums.PoolType.MonsterProjectile);
    }

    public virtual void ReturnMonsterProjectile(MonsterProjectile mp)
    {
        poolManager.Return(Enums.PoolType.MonsterProjectile, mp);
    }

    protected virtual void OnEnable()
    {
        if(agent != null) agent.isStopped = false;
    }
}
