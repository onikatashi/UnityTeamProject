using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public MonsterLaser laser;
    public float laserWindup = 0.6f;
    public float laserDuration = 2.5f;
    public float laserTick = 0.12f;
    public float laserReverseDuration = 1.2f; 

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

    [Header("BulletHell (HARD)")]
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
    }

    void Start()
    {
        // 씬 시작 시 한 번 찾기
        FindPlayerByLayer();
    }

    void EnsurePlayer()
    {
        if (player != null && playerComp != null) return;

        if (player != null && playerComp == null) playerComp = player.GetComponent<Player>();

        if (player == null || playerComp == null) FindPlayerByLayer();
    }

    void FindPlayerByLayer()
    {
        // 씬에 있는 모든 Player 컴포넌트 중 레이어가 맞는 것 찾기
        Player p = Object.FindFirstObjectByType<Player>(FindObjectsInactive.Include);
        if (p != null && p.gameObject.layer == playerLayer)
        {
            playerComp = p;
            player = p.gameObject;
        }
        
        // 못 찾으면 null 유지
        playerComp = null;
        player = null;
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

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.SetDestination(player.transform.position);

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

            // 후보 수집(조건부)
            List<BossPattern> conditional = new List<BossPattern>();
            if (Vector3.Distance(transform.position, player.transform.position) <= slamRange) conditional.Add(BossPattern.SlamStun);

            // playerComp 기반으로 스턴 여부 체크
            if (playerComp.IsStunned) conditional.Add(BossPattern.ExecuteCharge);

            if (randomMonsterPrefabs != null && randomMonsterPrefabs.Length > 0) conditional.Add(BossPattern.SummonAdds);
            if (topCenterPoint != null) conditional.Add(BossPattern.JumpTop);

            bool canUltimate = (Time.time >= lastUltimate + ultimateCd);

            // 필살기
            if (canUltimate && Random.value < 0.12f)
            {
                lastUltimate = Time.time;
                yield return StartCoroutine(Pattern_BulletHell());
                yield return new WaitForSeconds(patternIdle);
                continue;
            }

            // 스턴 중 즉사기 우선
            if (playerComp.IsStunned && Random.value < executeChanceIfStunned)
            {
                yield return StartCoroutine(Pattern_ExecuteCharge());
                yield return new WaitForSeconds(patternIdle);
                continue;
            }

            // 조건부 패턴 확률
            if (conditional.Count > 0 && Random.value < 0.45f)
            {
                BossPattern picked = conditional[Random.Range(0, conditional.Count)];
                yield return StartCoroutine(RunPattern(picked));
                yield return new WaitForSeconds(patternIdle);
                continue;
            }

            // 5) 일반패턴 4개(25%씩)
            int r = Random.Range(0, 4);
            BossPattern normal =
                (r == 0) ? BossPattern.Laser :
                (r == 1) ? BossPattern.Marks :
                (r == 2) ? BossPattern.SplitShot :
                              BossPattern.PullBurst;

            yield return StartCoroutine(RunPattern(normal));
            yield return new WaitForSeconds(patternIdle);
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

    // ====== 패턴들 ======

    IEnumerator Pattern_Laser()
    {
        EnsurePlayer();
        if (player == null || playerComp == null || laser == null) yield break;

        agent.isStopped = true;
        agent.updateRotation = false;

        laser.BeginWindup(laserWindup);
        yield return new WaitForSeconds(laserWindup);

        float t = 0f;
        float tick = 0f;
        laser.BeginFire();

        bool reversedOnce = false;

        while (t < laserDuration)
        {
            EnsurePlayer();
            if (player == null || playerComp == null) break;

            t += Time.deltaTime;
            tick += Time.deltaTime;

            Vector3 aim = player.transform.position - transform.position;
            aim.y = 0f;
            if (aim.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(aim.normalized);

            laser.UpdateAim(transform.position, transform.forward);

            if (tick >= laserTick)
            {
                laser.TryHit(playerComp, md.attackDamage); // 틱 데미지
                tick = 0f;

                if (!reversedOnce && Random.value < 0.35f)
                {
                    playerComp.ReverseInput(laserReverseDuration);
                    reversedOnce = true;
                }
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

        agent.isStopped = false;

        for (int i = 0; i < markCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(markAreaMin.x, markAreaMax.x),
                0f,
                Random.Range(markAreaMin.y, markAreaMax.y)
            );

            GroundTelegraph tg = Instantiate(markPrefab, pos, Quaternion.Euler(90f, 0f, 0f));
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

        agent.isStopped = true;

        Vector3 shootPos = firepoint != null ? firepoint.position : transform.position + transform.forward * 1.2f;
        Vector3 dir = (player.transform.position - shootPos);
        dir.y = 0f;
        dir = dir.normalized;

        BossBullet b = Instantiate(bossBulletPrefab, shootPos, Quaternion.LookRotation(dir));
        b.InitStraight(this, dir, bulletSpeed, splitDistance, splitChildCount);

        yield return new WaitForSeconds(0.4f);
        agent.isStopped = false;
    }

    IEnumerator Pattern_PullBurst()
    {
        EnsurePlayer();
        if (player == null || playerComp == null) yield break;

        agent.isStopped = true;

        // pullRange 밖이면 사용하지 않게(조건부 느낌)
        if (Vector3.Distance(transform.position, player.transform.position) > pullRange)
        {
            agent.isStopped = false;
            yield break;
        }

        yield return new WaitForSeconds(0.25f);
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

        agent.isStopped = false;
    }

    IEnumerator CoPull(Transform target, float duration, float strength)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        float t = 0f;

        while (t < duration && target != null)
        {
            t += Time.deltaTime;

            Vector3 dir = (transform.position - target.position);
            dir.y = 0f;
            dir = dir.normalized;

            if (rb != null)
            {
                Vector3 v = dir * strength;
                rb.linearVelocity = new Vector3(v.x, rb.linearVelocity.y, v.z); // 3D Rigidbody는 velocity
            }
            else
            {
                target.position += dir * strength * Time.deltaTime;
            }
            yield return null;
        }

        if (rb != null)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    IEnumerator Pattern_SlamStun()
    {
        EnsurePlayer();
        if (player == null || playerComp == null || markPrefab == null) yield break;

        agent.isStopped = true;

        GroundTelegraph tg = Instantiate(markPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
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

        agent.isStopped = false;
    }

    IEnumerator Pattern_ExecuteCharge()
    {
        EnsurePlayer();
        if (player == null || playerComp == null || markPrefab == null) yield break;

        // 스턴 중이 아니면 의미 없음
        if (!playerComp.IsStunned) yield break;

        agent.isStopped = true;

        Vector3 pos = player.transform.position;
        GroundTelegraph tg = Instantiate(markPrefab, pos, Quaternion.Euler(90f, 0f, 0f));
        tg.Setup(executeRadius, executeCharge);
        tg.StartCharge();

        yield return new WaitForSeconds(executeCharge);

        float dis = Vector3.Distance(player.transform.position, pos);
        if (dis <= executeRadius)
        {
            playerComp.TakeDamage(999999f); // 이제 컴포넌트로 호출
        }

        if (tg != null) { tg.StopAndHide(); Destroy(tg.gameObject); }
        agent.isStopped = false;
    }

    IEnumerator Pattern_SummonAdds()
    {
        if (randomMonsterPrefabs == null || randomMonsterPrefabs.Length == 0) yield break;

        agent.isStopped = true;

        int count = Random.Range(summonMin, summonMax + 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(summonAreaMin.x, summonAreaMax.x),
                0f,
                Random.Range(summonAreaMin.y, summonAreaMax.y)
            );

            GameObject prefab = randomMonsterPrefabs[Random.Range(0, randomMonsterPrefabs.Length)];
            Instantiate(prefab, pos, Quaternion.identity);

            yield return new WaitForSeconds(0.08f);
        }

        agent.isStopped = false;
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator Pattern_JumpTop()
    {
        if (topCenterPoint == null) yield break;

        agent.isStopped = true;
        transform.position = topCenterPoint.position; // 워프(연출은 나중에)
        yield return new WaitForSeconds(0.35f);

        agent.isStopped = false;
    }

    IEnumerator Pattern_BulletHell()
    {
        if (topCenterPoint != null)
        {
            agent.isStopped = true;
            transform.position = topCenterPoint.position;
        }

        float t = 0f;
        float fire = 0f;
        float spiralAngle = 0f;

        // 세트 1: 스파이럴 (탄 많게)
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
                FireBulletAtAngle(spiralAngle + 90f);   // 더 빡세게
                FireBulletAtAngle(spiralAngle + 270f);  // 더 빡세게
            }
            yield return null;
        }

        // 세트 2: 링 2번
        FireRing(hellRingBullets);
        yield return new WaitForSeconds(0.55f);
        FireRing(hellRingBullets);
        yield return new WaitForSeconds(0.55f);

        // 세트 3: 부채꼴 2번
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

        agent.isStopped = false;
        yield return new WaitForSeconds(0.4f);
    }

    void FireBulletAtAngle(float angleDeg)
    {
        if (bossBulletPrefab == null) return;

        Vector3 dir = new Vector3(Mathf.Cos(angleDeg * Mathf.Deg2Rad), 0f, Mathf.Sin(angleDeg * Mathf.Deg2Rad));
        Vector3 pos = firepoint != null ? firepoint.position : transform.position + Vector3.up * 1.0f;

        BossBullet b = Instantiate(bossBulletPrefab, pos, Quaternion.LookRotation(dir));
        b.InitSimple(this, dir, bulletSpeed);
    }

    void FireRing(int count)
    {
        float step = 360f / count;
        for (int i = 0; i < count; i++)
            FireBulletAtAngle(i * step);
    }

    void FireFan(float centerAngle, float fanAngle, int count)
    {
        if (count <= 1) { FireBulletAtAngle(centerAngle); return; }

        float start = centerAngle - fanAngle * 0.5f;
        float step = fanAngle / (count - 1);
        for (int i = 0; i < count; i++)
            FireBulletAtAngle(start + step * i);
    }
}
