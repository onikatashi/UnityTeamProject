using UnityEngine;

/// <summary>
/// 1. UI 관리를 담당하는 매니저
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private RectTransform canvas;                           // 캔버스 참조

    [SerializeField]
    private GameObject inventoryPanelPrefab;                // 인벤토리 패널 프리팹
    public InventoryUIController inventoryUIController;     // 인벤토리 UI 컨트롤러

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Canvas 등록
    public void RegisterCanvas(RectTransform canvasTransform)
    {
        // 이미 캔버스가 등록되어 있는 경우 경고 메시지 출력 후 종료
        if (canvas != null && canvas != canvasTransform)
        {
            Debug.LogWarning("UIManager: 이미 캔버스가 등록되어 있습니다.");
            return;
        }
        
        canvas = canvasTransform;

        InstantiateInventoryPanel(canvas);
    }

    // Canvas 등록 해제
    public void UnRegisterCanvas(RectTransform canvasTransform)
    {
        if (canvas == canvasTransform)
        {
            canvas = null;
        }
    }

    // 인벤토리 UI 토글
    public void ToggleInventory()
    {

        inventoryUIController.gameObject.SetActive(!inventoryUIController.gameObject.activeSelf);
    }

    // InventoryPanel 생성
    void InstantiateInventoryPanel(RectTransform canvas)
    {
        // 인벤토리 패널 생성 및 초기화
        GameObject panel = Instantiate(inventoryPanelPrefab, canvas);
        inventoryUIController = panel.GetComponent<InventoryUIController>();

        inventoryUIController.InitInventoryUIController();

        panel.SetActive(false);
        inventoryUIController.UpdateItemIcon();
    }
}
