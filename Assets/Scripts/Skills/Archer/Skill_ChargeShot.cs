using System;
using System.Collections;
using UnityEngine;

/// <summary>
///
/// </summary>
[CreateAssetMenu(menuName = "Skills/ChargeShot")]
public class Skill_ChargeShot : SkillBase
{
    public float firstDamage = 1.5f;                //첫번째 차지 데미지
    public float secondDamage = 2f;                 //두번째 차지 데미지
    public float thirdDamage = 3f;                  //세번째 차지 데미지
    public float minRange = 2f;                     //스킬 최소 사정 거리
    public float maxRange = 20f;                    //스킬 최대 사정 거리
    public float maxAreaRadius = 20f;               //데미지 주는 범위
    public float chargeStartTime;                   //차지를 시작한 시간
    public float maxChargeTime = 7f;                //최대로 차지할 수 있는 시간 ( 최대로 charge하고도 몇 초동안은 조준을 할 수 있게 해야함 / 이건 중간에 취소 없음 )

    public LayerMask monsterLayer;                  //몬스터 레이어(데미지용)
    public LayerMask groundLayer;                   //바닥 레이어(RayCast용)

    public GameObject rangeIndicatorPrefab;         //스킬 사정거리 UI 프리팹
    public GameObject areaIndicatorPrefab;          //데미지 적용 범위 UI 프리팹

    GameObject rangeInstance;                       //스킬 사정거리 UI 인스턴스
    GameObject areaInstance;                        //데미지 적용 범위 UI 인스턴스


    public override void ResetRuntime()             //처음 쿨타임 초기화용
    {
        base.ResetRuntime();
    }

    public override void Execute(Player player)
    {
        if (!CanUse())
        {
            Debug.Log($"Charge Shot 쿨타임 중, {lastUseTime}");
            return;
        }

        Debug.Log("Charge Shot 코루틴 실행");
        //player.StartCoroutine(ChargeShotRoutine(player));
    }

    /// <summary>
    /// 실제 차지샷 시퀀스
    /// 1.스킬을 활성화 하면
    /// 2.그 자리에 캐릭터가 멈추고 ( 무적 x )
    /// 3.Charge를 하기 시작 
    /// 4.총 Charge시간은 5초
    /// 5.5초 안에 클릭을 안하면 자동으로 마우스 포인트 지점으로 스킬 실행
    /// 6.Charge할 수록 점점 멀리까지 데미지를 적용할 수 있게되고
    /// 7.스킬 데미지도 점점 증가됨 ( 총 3단계로 구성 1단계 : 0 ~ 0.5초, 2단계 0.5초 ~ 1.5초, 3단계 : 1.5초 ~ maxChargeTime)/ 모일 때마다 이펙트를 주던지, area 알파값 단계별 조절하던지 해야함
    /// 8.스킬 활성화 중 마우스로 방향을 정하고
    /// 9.좌클릭을 하면 범위 내 몬스터에게 데미지 ( 데미지 범위는 캐릭터 위치 기준 원뿔모양 )
    /// 10데미지를 준 직후에 플레이어 스킬 시전방향 반대 방향으로 약간의 넉백
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    //IEnumerator ChargeShotRoutine(Player player)
    //{
    //    //캐릭터 못움직이게 하기
    //    var move = player.GetComponent<PlayerMove>();
    //    move.canMove = false;

    //    //차지 시작시간 저장
    //    chargeStartTime = Time.realtimeSinceStartup;
        

    //    float elapse = Time.realtimeSinceStartup - chargeStartTime;

    //    if(rangeIndicatorPrefab != null && rangeInstance == null)               //프리팹 있고 인스턴스 없을 때
    //    {
    //        rangeInstance = GameObject.Instantiate(rangeIndicatorPrefab);       //인스턴스 생성해주기
    //    }

    //    if(areaIndicatorPrefab != null && areaInstance == null)                 
    //    {
    //        areaInstance = GameObject.Instantiate(areaIndicatorPrefab);
    //    }

    //    //Instantiate 된 Instance 켜주기
    //    if(rangeInstance != null) rangeInstance.SetActive(true);
    //    if(areaInstance != null) areaInstance.SetActive(true);

    //    //최종 타겟 목표 (마우스로 찍을 위치)
    //    Vector3 targetPos = player.transform.position;

    //    while (true)
    //    {
    //        Vector3 playerPos = player.transform.position;
    //        playerPos.y = 0;

    //        //초기 aimPos는 플레이어 정면 방향
    //        Vector3 aimPos = playerPos + player.transform.forward * 1f;

    //        //mousePosition을 이용해서 world로 Ray 쏘기
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //        //마우스가 가리키는 지점이 groundLayer에 맞으면 그 지점을 aimPos로 사용
    //        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
    //        {
    //            aimPos = hit.point;
    //            aimPos.y = 0;
    //        }

    //        targetPos = aimPos;

            



    //        if (Input.GetMouseButtonDown(0) || chargeTime >= maxChargeTime)    //조준+클릭해서 시전하거나, 조준 중 maxChargeTime에 도달했을 경우
    //        {
    //            break;          //while문 빠져나가기
    //        }
    //    }

    //    //클릭 or maxChargeTime 한 경우 데미지 주기
    //    lastUseTime = Time.realtimeSinceStartup;        //쿨타임 시작 시간 기록

    //}

    public override void LevelUp()
    {
        base.LevelUp();

        //차지 시간 줄여주기
        maxChargeTime /= 0.9f;
    }


}
