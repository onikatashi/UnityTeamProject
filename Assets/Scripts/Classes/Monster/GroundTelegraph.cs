using UnityEngine;
using UnityEngine.UI;

public class GroundTelegraph : MonoBehaviour
{
    [Header("UI 원 이미지")]
    public Image ex;                 
    public RectTransform exRect;

    [Header("차징")]
    public float chargeTime = 1.5f;  // 표시 후 폭발까지 시간
    public float radius = 3f;        // 월드 단위 반경

    [Tooltip("월드 1유닛당 UI 스케일 보정값(프리팹에 맞게 조절)")]
    public float worldToUIScale = 1f;

    float timer;
    bool charging;

    Vector2 startSize; 
    Vector2 endSize;

    void Awake()
    {
        if (exRect == null && ex != null) exRect = ex.rectTransform;
        if (exRect != null) startSize = exRect.sizeDelta;
    }

    public void Setup(float radius, float chargeTime)
    {
        this.radius = radius;
        this.chargeTime = chargeTime;

        // 목표 사이즈 = 지름
        float diameter = radius * 2f * worldToUIScale;

        if (exRect != null)
        {
            endSize = new Vector2(diameter, diameter);
            // 시작 사이즈는 현재 exRect.sizeDelta를 사용
            startSize = exRect.sizeDelta;
        }
    }

    public void StartCharge()
    {
        charging = true;
        timer = 0f;
        if (ex != null) ex.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!charging || exRect == null) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / chargeTime);

        exRect.sizeDelta = Vector2.Lerp(startSize, endSize, t);
    }

    public bool IsDone() => charging && timer >= chargeTime;

    public void StopAndHide()
    {
        charging = false;
        if (ex != null) ex.gameObject.SetActive(false);
        timer = 0f;
        if (exRect != null) exRect.sizeDelta = startSize;
    }
}
