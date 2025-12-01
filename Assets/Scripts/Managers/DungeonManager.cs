using UnityEngine;

/// <summary>
/// 1. 던전 진행을 관리하는 매니저
/// 2. 방 클리어, 보상 지급, 다음 방 이동 등을 처리
/// </summary>
public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
