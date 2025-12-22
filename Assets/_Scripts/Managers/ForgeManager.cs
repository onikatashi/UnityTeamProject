using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    public static ForgeManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject forgePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (forgePanel != null)
            forgePanel.SetActive(false);
    }

    public void OpenForge()
    {
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
}