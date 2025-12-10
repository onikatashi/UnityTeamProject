using UnityEngine;

/// <summary>
/// 1. UI 관리를 담당하는 매니저
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private RectTransform canvas;                                   // 캔버스 참조

    [SerializeField]
    private GameObject inventoryPanelPrefab;                        // 인벤토리 패널 프리팹
    public InventoryUIController inventoryUIController;             // 인벤토리 UI 컨트롤러
    [SerializeField]
    private GameObject itemDescriptionPanelPrefab;                  // 아이템 설명 패널 프리팹
    public ItemDescriptionUIController itemDescriptionUIController; // 아이템 설명 UI 컨트롤러
    [SerializeField]
    private GameObject synergyEffectPanelPrefab;                    // 시너지 효과 활성화 패널 프리팹
    public SynergyEffectUIController synergyEffectUIController;     // 시너지 효과 UI 컨트롤러

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
        InstantiateItemDescriptionPanel(canvas);
        InstantiateSynergyEffectPanel(canvas);
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
        if (synergyEffectUIController.gameObject.activeSelf)
        {
            synergyEffectUIController.HideSynergyEffectUI();
        }
        else
        {
            synergyEffectUIController.ShowSynergyEffect();
        }
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

    void InstantiateItemDescriptionPanel(RectTransform canvas)
    {
        // 아이템 설명 패널 생성 및 초기화
        GameObject panel = Instantiate(itemDescriptionPanelPrefab, canvas);
        itemDescriptionUIController = panel.GetComponent<ItemDescriptionUIController>();
        panel.SetActive(false);
    }

    void InstantiateSynergyEffectPanel(RectTransform canvas)
    {
        // 시너지 활성화 패널 생성 및 초기화
        GameObject panel = Instantiate(synergyEffectPanelPrefab, canvas);
        synergyEffectUIController = panel.GetComponent<SynergyEffectUIController>();

        panel.SetActive(false);
    }

    public Transform GetCanvas()
    {
        return canvas;
    }
}
