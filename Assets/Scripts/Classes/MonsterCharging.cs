using UnityEngine;
using UnityEngine.UI;

public class MonsterCharging : MonoBehaviour
{
    [Header("UI 원 이미지 (Radial360)")]
    public Image rangeFill;     // 차징 보여주는 원 이미지

    [Header("차징 시간")]
    public float chargeTime = 3f; // 꽉 차는 데 걸리는 시간

    float timer = 0f;
    bool isCharging = false;

    public MonsterData md;      // 공격 범위 참조할 데이터

    void Start()
    {
        // 인스펙터에서 안 넣었으면 MonsterBase에서 가져오기
        if (md == null)
        {
            var baseMonster = GetComponent<MonsterBase>();
            if (baseMonster != null) md = baseMonster.md;
        }
    }

    void Update()
    {
        if (!isCharging || rangeFill == null || md == null) return;

        timer += Time.deltaTime;

        float ratio = Mathf.Clamp01(timer / chargeTime);

        // UI fillAmount (차징 게이지)
        rangeFill.fillAmount = ratio;

    }

    public void StartCharge()
    {
        isCharging = true;
        timer = 0f;
        
        if (rangeFill != null) rangeFill.fillAmount = 0f;
    }

}
