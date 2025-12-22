using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceTip : MonoBehaviour
{
    [Header("Tip Text Settings")]
    public string tipText = "이 물체의 설명입니다.";
    public float textSize = 1f;
    public Color textColor = Color.white;

    private Camera mainCam;
    private Canvas canvas;
    private TextMeshProUGUI textUI;

    void Start()
    {
        mainCam = Camera.main;
        CreateTipCanvas();
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            // 항상 카메라를 바라보도록 (빌보드)
            canvas.transform.LookAt(canvas.transform.position + mainCam.transform.rotation * Vector3.forward,
                                    mainCam.transform.rotation * Vector3.up);
        }
    }

    void CreateTipCanvas()
    {
        // Canvas 생성
        GameObject canvasObj = new GameObject("WorldTipCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.up * 2f; // 오브젝트 위에서 표시

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCam;
        canvas.sortingOrder = 9999; // 항상 위에 보이도록

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 배경 없는 TextMeshPro 생성
        GameObject textObj = new GameObject("TipText");
        textObj.transform.SetParent(canvasObj.transform);
        textUI = textObj.AddComponent<TextMeshProUGUI>();
        textUI.text = tipText;
        textUI.fontSize = 36;
        textUI.color = textColor;
        textUI.alignment = TextAlignmentOptions.Center;

        RectTransform rect = textUI.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 200);
        rect.localScale = Vector3.one * textSize;

        // 텍스트가 카메라에 의해 절대 가려지지 않도록
        canvasObj.layer = LayerMask.NameToLayer("UI");
        textObj.layer = LayerMask.NameToLayer("UI");
    }

    private void OnValidate()
    {
        if (textUI != null)
        {
            textUI.text = tipText;
            textUI.color = textColor;
            textUI.rectTransform.localScale = Vector3.one * textSize;
        }
    }
}
