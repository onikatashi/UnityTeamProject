using TMPro;
using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    public static ForgeManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject forgePanel;
    [SerializeField] private TextMeshPro tipText;

    // 선택 여부
    public bool isSelected = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 최초 로드 시 이전 forge 의 선택 여부 초기화
        isSelected = false;

        if (forgePanel != null)
            forgePanel.SetActive(false);
    }

    public void OpenForge()
    {
        // 이미 선택을 끝냈다면 다시 창을 열 수 없음
        if (isSelected)
        {
            Debug.Log("이미 선택 완료됨. Forge UI 열리지 않음.");
            return;
        }

        if (forgePanel != null)
        {
            forgePanel.SetActive(true);
            Debug.Log("대장간 UI 활성화");
        }
    }

    public void CloseForge()
    {
        if (forgePanel != null)
        {
            forgePanel.SetActive(false);
            Debug.Log("대장간 UI 비활성화");
        }
    }

    // ---------------------
    // 선택 버튼들 (추후 구현 예정)
    // ---------------------

    public void Select1()
    {
        CompleteSelection();

        // 선택1 기능 구현
        Debug.Log("Select1 눌림");

        CloseForge();
    }

    public void Select2()
    {
        CompleteSelection();

        // 선택2 기능 구현
        Debug.Log("Select2 눌림");

        CloseForge();
    }

    public void Select3()
    {
        CompleteSelection();

        // 선택3 기능 구현
        Debug.Log("Select3 눌림");

        CloseForge();
    }

    private void CompleteSelection()
    {
        isSelected = true; //선택 완료

        if (tipText != null) //월드 팁 텍스트 변경
            tipText.text = "선택 완료됨";

        CloseForge();
    }

}
