using UnityEngine;

/// <summary>
/// 1. UI 관리를 담당하는 매니저
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private RectTransform canvas;                                   // 캔버스 참조

    [SerializeField]
    [Header("인벤토리 UI")]
    private GameObject inventoryPanelPrefab;                        // 인벤토리 패널 프리팹
    public InventoryUIController inventoryUIController;             // 인벤토리 UI 컨트롤러

    [SerializeField]
    [Header("아이템 설명 UI")]
    private GameObject itemDescriptionPanelPrefab;                  // 아이템 설명 패널 프리팹
    public ItemDescriptionUIController itemDescriptionUIController; // 아이템 설명 UI 컨트롤러

    [SerializeField]
    [Header("시너지 효과 패널 UI")]
    private GameObject synergyEffectPanelPrefab;                    // 시너지 효과 활성화 패널 프리팹
    public SynergyEffectUIController synergyEffectUIController;     // 시너지 효과 UI 컨트롤러

    [SerializeField]
    [Header("시너지 설명 UI")]
    private GameObject synergyDescriptionPanelPrefab;               // 시너지 설명 패널 프리팹
    public SynergyDescriptionUIController synergyDescriptionUIController;   // 시너지 설명 패널 UI 컨트롤러

    [SerializeField]
    [Header("스왑, 강화 모드 UI")]
    private GameObject modeUIPrefab;                                // 강화, 스왑 모드 UI 프리팹
    public ModeUIController modeUIController;                       // 강화, 스왑 모드 UI 컨트롤러

    [SerializeField]
    [Header("세팅 UI")]
    private GameObject settingUIPrefab;                             // 세팅 UI 프리팹
    public SettingUIController settingUIController;                 // 세팅 UI 컨트롤러


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
        InstantiateSynergyDescriptionPanel(canvas);
        InstantiateModeUI(canvas);
        InstantiateSettingUI(canvas);
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
        // 인벤토리 UI 토글
        inventoryUIController.gameObject.SetActive(!inventoryUIController.gameObject.activeSelf);

        // 인벤토리가 비활성화 되어있는데 아이템 설명창이 남아있는 경우 아이템 설명창도 hide
        if (!inventoryUIController.gameObject.activeSelf && itemDescriptionUIController.gameObject.activeSelf)
        {
            itemDescriptionUIController.HideItemDescription();
        }

        // 시너지 효과는 오브젝트 풀을 쓰기 때문에 프리팹을 돌려주기 위해서 이렇게 경우를 나눔
        // ModeUI도 이 때 같이 켜져야 하기 때문에 추가적인 조건 없이 여기에 추가

        modeUIController.UpdateModeUI();
        if (synergyEffectUIController.gameObject.activeSelf)
        {
            synergyEffectUIController.HideSynergyEffectUI();
            modeUIController.HideModeUI();
        }
        else
        {
            synergyEffectUIController.ShowSynergyEffect();
            modeUIController.ShowModeUI();
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

    // ItemDescriptionPanel 생성
    void InstantiateItemDescriptionPanel(RectTransform canvas)
    {
        // 아이템 설명 패널 생성 및 초기화
        GameObject panel = Instantiate(itemDescriptionPanelPrefab, canvas);
        itemDescriptionUIController = panel.GetComponent<ItemDescriptionUIController>();
        panel.SetActive(false);
    }

    // SynergyEffectPanel 생성
    void InstantiateSynergyEffectPanel(RectTransform canvas)
    {
        // 시너지 활성화 패널 생성 및 초기화
        GameObject panel = Instantiate(synergyEffectPanelPrefab, canvas);
        synergyEffectUIController = panel.GetComponent<SynergyEffectUIController>();

        panel.SetActive(false);
    }

    // SynergyDescriptionPanel 생성
    void InstantiateSynergyDescriptionPanel(RectTransform canvas)
    {
        // 시너지 설명 패널 생성 및 초기화
        GameObject panel = Instantiate(synergyDescriptionPanelPrefab, canvas);
        synergyDescriptionUIController = panel.GetComponent<SynergyDescriptionUIController>();

        panel.SetActive(false);
    }

    // ModeUI 생성
    void InstantiateModeUI(RectTransform canvas)
    {
        GameObject ui = Instantiate(modeUIPrefab, canvas);
        modeUIController = ui.GetComponent<ModeUIController>();

        ui.SetActive(false);
    }

    // SettingUI 생성
    void InstantiateSettingUI(RectTransform canvas)
    {
        GameObject ui = Instantiate(settingUIPrefab, canvas);
        settingUIController = ui.GetComponent<SettingUIController>();

        ui.SetActive(true);
    }

    public Transform GetCanvas()
    {
        return canvas;
    }
}
