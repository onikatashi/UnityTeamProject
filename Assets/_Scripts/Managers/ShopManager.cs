using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject shopPanel; // 인스펙터에서 Shop 오브젝트 연결

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 최초 로딩 시 상점은 무조건 비활성화
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    // 상점 열기
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        // TODO: 상점이 열릴 때 아이템 목록을 갱신하는 로직 등 (추후 구현 예정)
        Debug.Log("상점을 열었습니다.");
    }

    // 상점 닫기 (Exit 버튼에 연결)
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Debug.Log("상점을 닫았습니다.");
    }
}