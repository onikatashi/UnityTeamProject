using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

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

    [Header("Dissolve")]
    public string dissolveProp = "_SplitValue";
    public float dissolveDuration = 1f;
    public float destroyDelay = 1f;

    Renderer rend;
    MaterialPropertyBlock mpb;
    int dissolveID;
    Coroutine dissolve;
    

    public MonsterProjectile bulletPrefab;
    public Transform firepoint;
    PoolManager poolManager;
    public bool isDef = false;
    bool isDie;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        state = Enums.MonsterState.Idle;

        rend = GetComponentInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
        dissolveID = Shader.PropertyToID(dissolveProp);

        SetDissolve(1f);


        if (agent != null)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
        }

        if (Player.Instance != null) player = Player.Instance.gameObject;
    }

    protected virtual void Start()
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
        SetDie(1f);
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
        isDie = false;

        if(agent != null) agent.isStopped = false;

        SetDissolve(1f);
    }
    void SetDie(float duration)
    {
        if (isDie) return;
        isDie = true;
        agent.enabled = false;

        // 플레이어에게 경험치 주기
        GiveExpToPlayer();
        if (dissolve != null) StopCoroutine(dissolve);

        dissolve = StartCoroutine(DissolveRoutine(1f, 0f, dissolveDuration));

        Destroy(gameObject, destroyDelay);
        
        
    }
    IEnumerator DissolveRoutine(float start, float end, float duration)
    {
        SetDissolve(start);
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float v = Mathf.Lerp(start, end, t);
            SetDissolve(v);
            yield return null;
        }

        SetDissolve(end);
    }

    void SetDissolve(float v)
    {
        if (rend == null) return;
        rend.GetPropertyBlock(mpb);
        mpb.SetFloat(dissolveID, v);
        rend.SetPropertyBlock(mpb);
    }

    /// <summary>
    /// 몬스터 사망 시 플레이어에게 경험치 지급해주는 함수
    /// 몬스터는 raw exp만 전달하고
    /// 실제 계산은 PlayerExperience에서 처리해서 적용할 예정
    /// </summary>
    protected virtual void GiveExpToPlayer()
    {
        if(Player.Instance == null) return;

        PlayerExperience exp = Player.Instance.GetComponent<PlayerExperience>();
        if(exp == null)
        {
            Debug.LogWarning("PlayerExperience 컴포넌트 없음, 추가 바랍니다");
            return;
        }

        exp.ReceiveRawExp(md.dropExp);
    }
}
