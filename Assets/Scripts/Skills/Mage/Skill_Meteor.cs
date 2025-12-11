using System.Collections;
using UnityEngine;

/// <summary>
/// 스킬 사정거리 내에서 
/// 마우스 클릭으로 좌표 지정 후
/// 잠깐의 시간이 지나면 그 좌표에 범위공격
/// </summary>
[CreateAssetMenu(menuName = "Skills/Meteor")]
public class Skill_Meteor : SkillBase
{
    public float maxRange = 6f;                 //스킬 사정거리
    public float areaRadius = 2f;               //데미지 주는 범위
    public float fallTime = 0.5f;               //운석이 떨어지는 시간(데미지 적용 시간으로 보면 됨)
    public float baseDamage = 2f;               //기본 곱연산 데미지(나중에 레벨업하면 증가 시켜야함)

    public LayerMask monsterLayer;              //몬스터 레이어(데미지용)
    public LayerMask groundLayer;               //바닥 레이어(RayCast용)

    public GameObject rangeIndicatorPrefab;     //스킬 사정거리 UI 프리팹
    public GameObject areaIndicatorPrefab;      //데미지 주는 범위 UI 프리팹
    public GameObject meteorPrefab;             //떨어지는 이펙트나 3d Asset 프리팹

    GameObject rangeInstance;                   //사정거리UI 인스턴스
    GameObject areaInstance;                    //데미지 범위UI 인스턴스 (나중에 범위늘려주는 옵션 넣을수도 있음)

    bool isAiming = false;                      //스킬 조준하고 있는 중인지 (나중에 스킬 취소 용)
    Coroutine runningRoutine = null;            //현재 돌고있는 코루틴

    public override void ResetRuntime()
    {
        base.ResetRuntime();
        isAiming = false;
        runningRoutine = null;
    }

    public override void Execute(Player player)
    {
        if (isAiming)                           //이미 스킬 조준 중이면 
        {
            CancelAim(player);                  //조준(스킬) 취소하기
            Debug.Log("Meteor 실행 취소");
            return;
        }

        if (!CanUse())
        {
            Debug.Log($"Meteor 쿨타임중, {lastUseTime} ");
            return;                  //쿨타임이면 취소하기
        }

        isAiming = true;                        //조준모드 시작
        Debug.Log("Meteor 실행 ");
        runningRoutine = player.StartCoroutine(MeteorRoutine(player));      //코루틴 시작(실행+변수 저장까지 다함)
    }

    /// <summary>
    /// 조준 상태 취소 함수
    /// </summary>
    /// <param name="player"></param>
    void CancelAim(Player player)
    {
        isAiming = false;                       //조준 취소로 바꾸기

        if(runningRoutine != null)              //메테오 코루틴이 있을때
        {
            player.StopCoroutine(runningRoutine);   //메테오 코루틴을 멈춘다.
            runningRoutine = null;                  //돌아가고있는 코루틴 값 null시켜주기
        }

        //조준UI 끄기
        if(rangeInstance != null) rangeInstance.SetActive(false);       
        if(areaInstance != null) areaInstance.SetActive(false);
    }

    /// <summary>
    /// 실제 메테오 시퀀스
    /// 1. 사정거리/ 범위 UI생성 + 활성화
    /// 2. 마우스 포인터를 따라 범위UI 이동
    /// 3. 좌클릭 시 해당 위치 확정
    /// 4. 메테오 이펙트 생성 후 fallTime이 지나면 해당 범위에 TakeDamage
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    IEnumerator MeteorRoutine(Player player)
    {
        if(rangeIndicatorPrefab != null && rangeInstance == null)   //프리팹은 있고 인스턴스가 없을 때
        {
            rangeInstance = GameObject.Instantiate(rangeIndicatorPrefab);
        }

        if(areaIndicatorPrefab != null && areaInstance == null)     //위에와 마찬가지
        {
            areaInstance = GameObject.Instantiate(areaIndicatorPrefab);
        }

        //Instantiate 된 Instance 켜주기
        if(rangeInstance != null) rangeInstance.SetActive(true);
        if(areaInstance != null) areaInstance.SetActive(true);

        //최종 타겟 목표 (마우스로 찍을 위치)
        Vector3 targetPos = player.transform.position;

        //2. 마우스 움직이는 동안 계속 조준 위치 업데이트
        while (true)
        {
            //플레이어 위치 (y=0해주기)
            Vector3 playerPos = player.transform.position;
            playerPos.y = 0f;

            //초기 aimPos는 플레이어 정면 방향으로 MaxRange거리 지점
            Vector3 aimPos = playerPos + player.transform.forward * maxRange;

            //mousePosition을 이용해서 world로 쏘는 Ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //마우스가 가리키는 지점이 groundLayer에 맞으면 그 지점을 aimPos로 사용
            if(Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                aimPos = hit.point;
                aimPos.y = 0f;              //높이 맞춰주기
            }

            //플레이어 기준 오프셋
            Vector3 offset = aimPos - playerPos;
            offset.y = 0f;

            //거리 비교는 sqrMagnitude로 최적화
            float sqrMaxRange = maxRange * maxRange;

            //오프셋 길이가 maxRange보다 크면 -> maxRange테두리에 붙여버리기
            if(offset.sqrMagnitude > sqrMaxRange)
            {
                offset = offset.normalized * maxRange;
            }

            //최종 타겟 위치 = 플레이어 위치 + 오프셋
            targetPos = playerPos + offset;

            //사정거리/ 범위 위치&크기 갱신
            if(rangeInstance != null)
            {
                rangeInstance.transform.position = playerPos;                               //스킬 사정거리니까 플레이어 기준
                rangeInstance.transform.localScale = Vector3.one * (maxRange * 2f);         //반지름이니까 2배
            }
            if(areaInstance != null)
            {
              
                areaInstance.transform.position = targetPos + Vector3.up * 0.02f;           //살짝 위로 띄워서 파란원 위에 보이게 하기
                areaInstance.transform.localScale = Vector3.one * (areaRadius * 2f);        //반지름이니까 2배
            }

            if (Input.GetMouseButtonDown(0))
            {
                break;                      //좌클릭을해서 시전 확정 지으면 while문 빠져나가기
            }

            //한 프레임 쉬고 다시 while 반복
            yield return null;
        }

        //시전 확정났을 때
        isAiming = false;                   //조준 끝났고
        runningRoutine = null;              //핸들 비워주고
        lastUseTime = Time.realtimeSinceStartup;            //쿨타임 시작 시간 기록


        //UI비활성화
        if (rangeInstance != null) rangeInstance.SetActive(false);
        if(areaInstance != null) areaInstance.SetActive(false); 

        //메테오 연출 + 데미지
        if(meteorPrefab != null)
        {
            //타겟 위로 10만큼 높이에서 떨어지게하기 (나중에 수치조정 해도됨)
            Vector3 start = targetPos + Vector3.up * 10f;
            GameObject.Instantiate(meteorPrefab, start, Quaternion.identity);
        }

        // 메테오 떨어지는거 기다려주고
        yield return new WaitForSeconds(fallTime);

        //범위 안에 들어온 Monster레이어를 가진 오브젝트들에게 TakeDamage
        float dmg = player.finalStats.attackDamage * baseDamage;
        Collider[] monsters = Physics.OverlapSphere(targetPos, areaRadius, monsterLayer);
        foreach(Collider m in monsters)
        {
            var mob = m.GetComponent<MonsterBase>();
            if (mob != null) mob.TakeDamage(dmg);
            Debug.Log($"맞은 몬스터 : {mob.name}");
        }
    }
}
