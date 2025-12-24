using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("Materials")]
    public Material normalMat; 
    public Material dissolveMat;  

    protected SpriteRenderer sr;
    protected Material runtimeDissolveMat;     // 개체별 인스턴스
    Coroutine dissolveCo;

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

        sr = GetComponentInChildren<SpriteRenderer>(true);
        if (sr == null)
        {
            //Debug.LogError($"{name} : SpriteRenderer 를 찾을수 없음");
        }
        else
        {
            // 기본 머티리얼 적용
            if (normalMat != null) sr.sharedMaterial = normalMat;

            // 디졸브 머티리얼은 개체별 인스턴스 생성
            if (dissolveMat != null) runtimeDissolveMat = new Material(dissolveMat);
        }

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
            poolManager.CreatePool<MonsterProjectile>(Enums.PoolType.MonsterProjectile, bulletPrefab, 20, null);
        }
    }

    protected virtual void Update()
    {
        if (isDef) return;

        switch (state)
        {
            case Enums.MonsterState.Idle: Idle(); break;
            case Enums.MonsterState.Move: Move(); break;
            case Enums.MonsterState.Attack: Attack(); break;
            case Enums.MonsterState.Die: Die(); break;
        }
    }

    protected abstract void Idle();
    protected abstract void Move();
    protected abstract void Attack();

    protected virtual void Die()
    {
        SetDie();
    }

    public virtual void TakeDamage(float dmg)
    {
        BuffReceiver buff = GetComponent<BuffReceiver>();
        if (buff != null) dmg *= buff.DamageTakenMultiplier;

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

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        //풀에서 재사용될 때 기본 머티리얼로 복구
        UseNormalMat();
        SetDissolveValue(1f);
    }

    void SetDie()
    {
        if (isDie) return;
        isDie = true;

        if (agent != null) agent.enabled = false;

        // 플레이어에게 경험치 + 골드 주기
        GiveExpToPlayer();
        GiveGoldToPlayer();

        Debug.Log($"플레이어에게 경험치 주기 성공{md.dropExp}");

        //죽을 때만 디졸브 머티리얼 적용 + 디졸브 실행
        if (dissolveCo != null) StopCoroutine(dissolveCo);
        dissolveCo = StartCoroutine(CoDieDissolveAndDestroy());
    }

    IEnumerator CoDieDissolveAndDestroy()
    {
        UseDissolveMat();

        yield return CoDissolve(1f, 0f, dissolveDuration);

        Destroy(gameObject, destroyDelay);
    }

    IEnumerator CoDissolve(float start, float end, float duration)
    {
        SetDissolveValue(start);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float v = Mathf.Lerp(start, end, t);
            SetDissolveValue(v);
            yield return null;
        }

        SetDissolveValue(end);
    }

    // 디졸브 값 세팅 (runtimeDissolveMat에만)
    void SetDissolveValue(float v)
    {
        if (runtimeDissolveMat == null) return;
        runtimeDissolveMat.SetFloat(dissolveProp, v);
    }

    //머티리얼 전환
    protected void UseDissolveMat()
    {
        if (sr == null || runtimeDissolveMat == null) return;
        sr.material = runtimeDissolveMat; // 인스턴스 머티리얼 적용
    }

    protected void UseNormalMat()
    {
        if (sr == null || normalMat == null) return;
        sr.sharedMaterial = normalMat;
    }

    protected virtual void GiveExpToPlayer()
    {
        if (Player.Instance == null) return;

        PlayerExperience exp = Player.Instance.GetComponent<PlayerExperience>();
        if (exp == null) return;

        exp.ReceiveRawExp(md.dropExp);
    }

    protected virtual void GiveGoldToPlayer()
    {
        if (Player.Instance == null) return;

        PlayerGold gold = Player.Instance.GetComponent<PlayerGold>();

        gold.ReceiveRawGold(md.dropGold);
    }
}
