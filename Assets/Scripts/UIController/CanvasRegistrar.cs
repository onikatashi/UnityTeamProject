using UnityEngine;

public class CanvasRegistrar : MonoBehaviour
{
    private RectTransform canvas;

    UIManager uiManager;
    private void Awake()
    {
        canvas = GetComponent<RectTransform>();
        if(canvas == null)
        {
            Debug.LogError("canavas를 찾을 수 없음.");
        }
    }

    private void Start()
    {
        uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.RegisterCanvas(canvas);
        }
    }

    private void OnDisable()
    {
        if (uiManager != null)
        {
            uiManager.UnRegisterCanvas(canvas);
        }
    }
}
