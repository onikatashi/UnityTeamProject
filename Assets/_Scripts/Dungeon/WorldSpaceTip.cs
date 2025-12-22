using UnityEngine;
using TMPro;

public class WorldSpaceTip : MonoBehaviour
{
    [Header("Text Settings")]
    public string displayText = "Hello!";
    public float fontSize = 3f;
    public Color textColor = Color.white;

    private TextMeshPro textMesh;
    private Transform mainCamera;

    void Start()
    {
        // 카메라 참조
        mainCamera = Camera.main.transform;

        // TextMeshPro 컴포넌트 생성 또는 가져오기
        textMesh = gameObject.GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TextMeshPro>();
        }

        ApplyTextSettings();
    }

    void Update()
    {
        // 카메라를 바라보도록 회전
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);
    }

    void OnValidate()
    {
        // 에디터에서 값 변경 시 자동 적용
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        if (textMesh != null)
            ApplyTextSettings();
    }

    private void ApplyTextSettings()
    {
        textMesh.text = displayText;
        textMesh.fontSize = fontSize;
        textMesh.color = textColor;
        textMesh.alignment = TextAlignmentOptions.Center;
    }
}
