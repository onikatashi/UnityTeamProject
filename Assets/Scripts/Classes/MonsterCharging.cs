using UnityEngine;
using UnityEngine.UI;

public class MonsterCharging : MonoBehaviour
{
    [Header("UI 원 이미지 (Radial360)")]
    public Image rangeFill;     // 차징 보여주는 원 이미지

    [Header("범위 설정")]
    public float chargeTime = 3f; // 최대까지 차오르는 시간

    public float currentRange;  // 지금 커진 실제 반경 값

    float timer = 0f;
    bool isCharging = false;

    MonsterData md;

    private void Start()
    {
        md = GetComponent<MonsterData>();
    }

    void Update()
    {
        if (!isCharging || rangeFill == null) return;

        // 0 → 1 비율
        timer += Time.deltaTime / chargeTime;
        float ratio = Mathf.Clamp01(timer);

        // UI fillAmount (차징 게이지)
        rangeFill.fillAmount = ratio;

        // 실제 공격 범위 (0 → attackRange)
        currentRange = ratio * md.attackRange;
    }

    // 차징 시작
    public void StartCharge()
    {
        isCharging = true;
        timer += Time.deltaTime;
        if (rangeFill != null) rangeFill.fillAmount = 0f;

        //currentRange = 0f;
    }

    // 차징 멈추기
    public void StopCharge()
    {
        isCharging = false;
        timer = 0f;
        currentRange = 0f;

        if (rangeFill != null) rangeFill.fillAmount = 0f;
        

    }
}

                

