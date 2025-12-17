using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonsterBase
{
    public enum BossPattern
    {
        Laser, Marks, SplitShot, PullBurst,
        SlamStun, ExecuteCharge, SummonAdds, JumpTop,
        BulletHell,
    }

    [Header("Player Find")]
    public string playerLayerName = "Player";
    int playerLayer = -1;

    Player playerComp;

    [Header("Boss Core")]
    public Transform topCenterPoint;
    public float patternIdle = 0.6f;        
    public float executeChanceIfStunned = 0.8f;

    [Header("Laser")]
    public Transform firePoint2;
    [SerializeField] private MonsterLaser laser;
    public float laserWindup = 0.6f;
    public float laserDuration = 2.5f;
    public float laserTick = 0.12f;
    public float laserReverseDuration = 1.2f;

    public float laserTurnDegPerSec = 120f;     
    public float laserSpawnYOffset = 0.5f;        
    

    [Header("Marks")]
    public GroundTelegraph markPrefab;
    public int markCount = 3;
    public float markDelay = 1.0f;
    public float markRadius = 3.5f;
    public float markStun = 0.8f;
    public Vector2 markAreaMin = new Vector2(-10, -6);
    public Vector2 markAreaMax = new Vector2(10, 6);

    [Header("SplitShot")]
    public BossBullet bossBulletPrefab;
    public int splitChildCount = 10;
    public float splitDistance = 10f;
    public float bulletSpeed = 14f;

    [Header("PullBurst")]
    public float pullRange = 10f;
    public float pullDuration = 0.35f;
    public float pullStrength = 14f;
    public float pullExplosionRadius = 2.5f;
    public float pullStun = 0.6f;

    [Header("PullBurst Telegraph")]
    public GroundTelegraph pullTelegraphPrefab; 
    public float pullTelegraphYOffset = 0.05f; 
    public bool telegraphUseBossY = false;    


    [Header("SlamStun")]
    public float slamRange = 6f;
    public float slamWindup = 0.8f;
    public float slamRadius = 5f;
    public float slamStun = 1.2f;

    [Header("Execute")]
    public float executeCharge = 2.0f;
    public float executeRadius = 4.5f;

    [Header("Summon")]
    public GameObject[] randomMonsterPrefabs;
    public int summonMin = 2;
    public int summonMax = 4;
    public Vector2 summonAreaMin = new Vector2(-9, -5);
    public Vector2 summonAreaMax = new Vector2(9, 5);

    [Header("Summon FX")]
    public GameObject summonFxPrefab;
    public float summonFxLife = 2f;

    [Header("BulletHell")]
    public float hellDuration = 8f;
    public float hellFireInterval = 0.08f;
    public int hellRingBullets = 60;
    public int hellFanBullets = 21;
    public float hellFanAngle = 110f;
    public float hellSpiralSpeed = 420f;

    bool runningPattern = false;
    float lastUltimate = -999f;
    public float ultimateCd = 18f;

    protected override void Awake()
    {
        base.Awake();
        playerLayer = LayerMask.NameToLayer(playerLayerName);

        if (laser == null) laser = GetComponentInChildren<MonsterLaser>(true);
    }

    void Start()
    {
        FindPlayerByLayer();

        if (PoolManager.Instance != null && bossBulletPrefab != null)
        {
            PoolManager.Instance.CreatePool(Enums.PoolType.BossBullet, bossBulletPrefab, 200);
        }

        laser.EndFire();
    }

    void EnsurePlayer()
    {
        if (player != null && playerComp != null) return;

        if (player != null && playerComp == null)
            playerComp = player.GetComponent<Player>();

        if (player == null || playerComp == null)
            FindPlayerByLayer();
    }

    void FindPlayerByLayer()
    {
        Player p = Object.FindFirstObjectByType<Player>(FindObjectsInactive.Include);
        if (p != null && p.gameObject.layer == playerLayer)
        {
            playerComp = p;
            player = p.gameObject;
            return;
        }

        playerComp = null;
        player = null;
    }

    // md.attackSpeed를 다음 패턴까지 대기시간으로 사용
    float PatternInterval()
    {
        if (md != null && md.attackSpeed > 0f) return md.attackSpeed;
        return patternIdle;
    }

    protected override void Idle()
    {
        EnsurePlayer();
        if (player == null) return;

        state = Enums.MonsterState.Move;
    }

    protected override void Move()
    {
        EnsurePlayer();
        if (player == null || md == null) return;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(player.transform.position);
        }

        if (!runningPattern && Vector3.Distance(transform.position, player.transform.position) <= md.attackRange)
            state = Enums.MonsterState.Attack;
    }

    protected override void Attack()
    {
        EnsurePlayer();
        if (runningPattern || player == null || playerComp == null || md == null) return;

        StartCoroutine(CoPatternLoop());
    }

    IEnumerator CoPatternLoop()
    {
        runningPattern = true;

        while (state != Enums.MonsterState.Die)
        {
            EnsurePlayer();
            if (player == null || playerComp == null) yield break;

            List<BossPattern> conditional = new List<BossPattern>();
            if (Vector3.Distance(transform.position, player.transform.position) <= slamRange)
                conditional.Add(BossPattern.SlamStun);

            if (playerComp.IsStunned)
                conditional.Add(BossPattern.ExecuteCharge);

            if (randomMonsterPrefabs != null && randomMonsterPrefabs.Length > 0)
                conditional.Add(BossPattern.SummonAdds);

            if (topCenterPoint != null)
                conditional.Add(BossPattern.JumpTop);

            bool canUltimate = (Time.time >= lastUltimate + ultimateCd);

            if (canUltimate && Random.value < 0.12f)
            {
                lastUltimate = Time.time;
                yield return StartCoroutine(Pattern_BulletHell());
                yield return new WaitForSeconds(PatternInterval());
                continue;
            }

            if (playerComp.IsStunned && Random.value < executeChanceIfStunned)
            {
                yield return StartCoroutine(Pattern_ExecuteCharge());
                yield return new WaitForSeconds(PatternInterval());
                continue;
            }

            if (conditional.Count > 0 && Random.value < 0.45f)
            {
                BossPattern picked = conditional[Random.Range(0, conditional.Count)];
                yield return StartCoroutine(RunPattern(picked));
                yield return new WaitForSeconds(PatternInterval());
                continue;
            }

            int r = Random.Range(0, 4);
            BossPattern normal =
                (r == 0) ? BossPattern.Laser :
                (r == 1) ? BossPattern.Marks :
                (r == 2) ? BossPattern.SplitShot :
                           BossPattern.PullBurst;

            yield return StartCoroutine(RunPattern(normal));
            yield return new WaitForSeconds(PatternInterval());
        }

        runningPattern = false;
        state = Enums.MonsterState.Move;
    }

    IEnumerator RunPattern(BossPattern p)
    {
        switch (p)
        {
            case BossPattern.Laser: yield return StartCoroutine(Pattern_Laser()); break;
            case BossPattern.Marks: yield return StartCoroutine(Pattern_Marks()); break;
            case BossPattern.SplitShot: yield return StartCoroutine(Pattern_SplitShot()); break;
            case BossPattern.PullBurst: yield return StartCoroutine(Pattern_PullBurst()); break;
            case BossPattern.SlamStun: yield return StartCoroutine(Pattern_SlamStun()); break;
            case BossPattern.ExecuteCharge: yield return StartCoroutine(Pattern_ExecuteCharge()); break;
            case BossPattern.SummonAdds: yield return StartCoroutine(Pattern_SummonAdds()); break;
            case BossPattern.JumpTop: yield return StartCoroutine(Pattern_JumpTop()); break;
        }
    }

    // ====== 패턴 ======

    IEnumerator Pattern_Laser()
    {
        EnsurePlayer();
        if (player == null || playerComp == null || md == null || laser == null)
            yield break;

        agent.isStopped = true;
        agent.updateRotation = false;

        laser.BeginCharge();
        yield return new WaitForSeconds(laserWindup);

        laser.BeginFire();

        float t = 0f;
        float tick = 0f;

        while (t < laserDuration)
        {
            t += Time.deltaTime;
            tick += Time.deltaTime;

            // 회전 추적
            Vector3 aim = player.transform.position - transform.position;
            aim.y = 0f;

            if (aim.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(aim.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    target,
                    laserTurnDegPerSec * Time.deltaTime
                );
            }

            // firePoint 기준으로 레이저 갱신
            Vector3 origin = firePoint2 != null ? firePoint2.position : transform.position + Vector3.up * 0.5f;
            Vector3 forward = firePoint2 != null ? firePoint2.forward : transform.forward;

            laser.UpdateAim(origin, forward);

            if (tick >= laserTick)
            {
                laser.TryHit(md.attackDamage);
                tick = 0f;

                // 입력 반전
                if (Random.value < 0.25f) playerComp.ReverseInput(laserReverseDuration);
            }

            yield return null;
        }

        laser.EndFire();
        agent.isStopped = false;
    }



    IEnumerator Pattern_Marks()
    {
        EnsurePlayer();
        if (player == null || md == null || markPrefab == null) yield break;

        if (agent != null) agent.isStopped = false;

        for (int i = 0; i < markCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(markAreaMin.x, markAreaMax.x), 0f,
                Random.Range(markAreaMin.y, markAreaMax.y)
            );

            GroundTelegraph tg = Instantiate(markPrefab, pos, Quaternion.Euler(90f, 1f, 0f));
            tg.Setup(markRadius, markDelay);
            tg.StartCharge();

            StartCoroutine(CoMarkExplode(pos, markRadius, markDelay, markStun, tg));
            yield return new WaitForSeconds(0.18f);
        }

        yield return new WaitForSeconds(0.35f);
    }

    IEnumerator CoMarkExplode(Vector3 pos, float radius, float delay, float stun, GroundTelegraph tg)
    {
        yield return new WaitForSeconds(delay);

        Collider[] hits = Physics.OverlapSphere(pos, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer != playerLayer) continue;

            Player p = hits[i].GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(md.attackDamage);
                p.Stun(stun);
                break;
            }
        }

        if (tg != null) { tg.StopAndHide(); Destroy(tg.gameObject); }
    }

    IEnumerator Pattern_SplitShot()
    {
        EnsurePlayer();
        if (player == null || bossBulletPrefab == null) yield break;

        if (agent != null) agent.isStopped = true;

        Vector3 shootPos = firepoint != null ? firepoint.position : transform.position + transform.forward * 1.2f;
        Vector3 dir = (player.transform.position - shootPos);
        dir.y = 0f;
        dir = dir.normalized;

        BossBullet b = null;
        if (PoolManager.Instance != null)
            b = PoolManager.Instance.Get<BossBullet>(Enums.PoolType.BossBullet);

        if (b == null)
        {
            b = Instantiate(bossBulletPrefab, shootPos, Quaternion.LookRotation(dir));
        }
        else
        {
            b.transform.SetPositionAndRotation(shootPos, Quaternion.LookRotation(dir));
        }

        b.InitStraight(this, dir, bulletSpeed, splitDistance, splitChildCount);

        yield return new WaitForSeconds(0.4f);
        if (agent != null) agent.isStopped = false;
    }

    IEnumerator Pattern_PullBurst()
    {
        EnsurePlayer();
        if (player == null || playerComp == null || md == null) yield break;

        if (agent != null) agent.isStopped = true;

        if (Vector3.Distance(transform.position, player.transform.position) > pullRange)
        {
            if (agent != null) agent.isStopped = false;
            yield break;
        }

        float windup = 0.25f;

        yield return StartCoroutine(CoPullTelegraph(pullExplosionRadius, windup));

        yield return StartCoroutine(CoPull(player.transform, pullDuration, pullStrength));

        Collider[] hits = Physics.OverlapSphere(transform.position, pullExplosionRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer != playerLayer) continue;

            Player p = hits[i].GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(md.attackDamage);
                p.Stun(pullStun);
                break;
            }
        }

        if (agent != null) agent.isStopped = false;
    }


    IEnumerator CoPull(Transform target, float duration, float strength)
    {
        CharacterController cc = target != null ? target.GetComponent<CharacterController>() : null;

        float t = 0f;
        while (t < duration && target != null)
        {
            t += Time.deltaTime;

            Vector3 dir = (transform.position - target.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) break;
            dir.Normalize();

            Vector3 delta = dir * strength * Time.deltaTime;

            if (cc != null) cc.Move(delta);
            else target.position += delta;

            yield return null;
        }
    }

    IEnumerator Pattern_SlamStun()
    {
        EnsurePlayer();
        if (player == null || playerComp == null || markPrefab == null) yield break;

        if (agent != null) agent.isStopped = true;

        GroundTelegraph tg = Instantiate(markPrefab, transform.position, Quaternion.Euler(90f, 1f, 0f));
        tg.Setup(slamRadius, slamWindup);
        tg.StartCharge();

        yield return new WaitForSeconds(slamWindup);

        Collider[] hits = Physics.OverlapSphere(transform.position, slamRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer != playerLayer) continue;

            Player p = hits[i].GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(md.attackDamage);
                p.Stun(slamStun);
                break;
            }
        }

        if (tg != null) { tg.StopAndHide(); Destroy(tg.gameObject); }

        if (agent != null) agent.isStopped = false;
    }

    IEnumerator Pattern_ExecuteCharge()
    {
        EnsurePlayer();
        if (player == null || playerComp == null || markPrefab == null) yield break;

        if (!playerComp.IsStunned) yield break;

        if (agent != null) agent.isStopped = true;

        Vector3 pos = player.transform.position;
        GroundTelegraph tg = Instantiate(markPrefab, pos, Quaternion.Euler(90f, 1f, 0f));
        tg.Setup(executeRadius, executeCharge);
        tg.StartCharge();

        yield return new WaitForSeconds(executeCharge);

        float dis = Vector3.Distance(player.transform.position, pos);
        if (dis <= executeRadius)
            playerComp.TakeDamage(999999f);

        if (tg != null) { tg.StopAndHide(); Destroy(tg.gameObject); }

        if (agent != null) agent.isStopped = false;
    }

    IEnumerator Pattern_SummonAdds()
    {
        if (randomMonsterPrefabs == null || randomMonsterPrefabs.Length == 0) yield break;

        if (agent != null) agent.isStopped = true;

        int count = Random.Range(summonMin, summonMax + 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(summonAreaMin.x, summonAreaMax.x), 0f,
                Random.Range(summonAreaMin.y, summonAreaMax.y)
            );

            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2.5f, NavMesh.AllAreas))
                pos = hit.position;

            if (summonFxPrefab != null)
            {
                GameObject fx = Instantiate(summonFxPrefab, pos, Quaternion.identity);
                Destroy(fx, summonFxLife);
            }

            GameObject prefab = randomMonsterPrefabs[Random.Range(0, randomMonsterPrefabs.Length)];
            GameObject go = Instantiate(prefab, pos, Quaternion.identity);

            MonsterBase mb = go.GetComponent<MonsterBase>();
            if (mb != null) mb.player = player;

            NavMeshAgent a = go.GetComponent<NavMeshAgent>();
            if (a != null)
            {
                a.enabled = true;
                a.isStopped = false;
                a.Warp(pos);
            }

            yield return new WaitForSeconds(0.08f);
        }

        if (agent != null) agent.isStopped = false;
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator Pattern_JumpTop()
    {
        if (topCenterPoint == null) yield break;

        if (agent != null) agent.isStopped = true;
        transform.position = topCenterPoint.position;
        yield return new WaitForSeconds(0.35f);
        if (agent != null) agent.isStopped = false;
    }

    IEnumerator Pattern_BulletHell()
    {
        if (topCenterPoint != null)
        {
            if (agent != null) agent.isStopped = true;
            transform.position = topCenterPoint.position;
        }

        float t = 0f;
        float fire = 0f;
        float spiralAngle = 0f;

        while (t < hellDuration * 0.34f)
        {
            t += Time.deltaTime;
            fire += Time.deltaTime;

            spiralAngle += hellSpiralSpeed * Time.deltaTime;

            if (fire >= hellFireInterval)
            {
                fire = 0f;
                FireBulletAtAngle(spiralAngle);
                FireBulletAtAngle(spiralAngle + 180f);
                FireBulletAtAngle(spiralAngle + 90f);
                FireBulletAtAngle(spiralAngle + 270f);
            }
            yield return null;
        }

        FireRing(hellRingBullets);
        yield return new WaitForSeconds(0.55f);
        FireRing(hellRingBullets);
        yield return new WaitForSeconds(0.55f);

        EnsurePlayer();
        if (player != null)
        {
            Vector3 dir = (player.transform.position - transform.position);
            dir.y = 0f;
            float baseAng = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            FireFan(baseAng, hellFanAngle, hellFanBullets);
            yield return new WaitForSeconds(0.45f);
            FireFan(baseAng + 25f, hellFanAngle, hellFanBullets);
        }

        if (agent != null) agent.isStopped = false;
        yield return new WaitForSeconds(0.4f);
    }

    void FireBulletAtAngle(float angleDeg)
    {
        if (bossBulletPrefab == null) return;

        Vector3 dir = new Vector3(Mathf.Cos(angleDeg * Mathf.Deg2Rad), 0f, Mathf.Sin(angleDeg * Mathf.Deg2Rad));
        Vector3 pos = firepoint != null ? firepoint.position : transform.position + Vector3.up * 1.0f;

        BossBullet b = null;
        if (PoolManager.Instance != null)
            b = PoolManager.Instance.Get<BossBullet>(Enums.PoolType.BossBullet);

        if (b == null)
        {
            b = Instantiate(bossBulletPrefab, pos, Quaternion.LookRotation(dir));
        }
        else
        {
            b.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(dir));
        }

        b.InitSimple(this, dir, bulletSpeed);
    }

    void FireRing(int count)
    {
        float step = 360f / count;
        for (int i = 0; i < count; i++) FireBulletAtAngle(i * step);
    }

    void FireFan(float centerAngle, float fanAngle, int count)
    {
        if (count <= 1) { FireBulletAtAngle(centerAngle); return; }

        float start = centerAngle - fanAngle * 0.5f;
        float step = fanAngle / (count - 1);
        for (int i = 0; i < count; i++) FireBulletAtAngle(start + step * i);
    }

    IEnumerator CoPullTelegraph(float radius, float chargeTime)
    {
        if (pullTelegraphPrefab == null) yield break;

        Vector3 pos = transform.position;

        if (!telegraphUseBossY) pos.y = 0f;

        pos.y += pullTelegraphYOffset;

        GroundTelegraph tg = Instantiate(pullTelegraphPrefab, pos, Quaternion.Euler(90f, 0f, 0f));
        tg.Setup(radius, chargeTime);
        tg.StartCharge();

        yield return new WaitForSeconds(chargeTime);

        if (tg != null)
        {
            tg.StopAndHide();
            Destroy(tg.gameObject);
        }
    }

}
