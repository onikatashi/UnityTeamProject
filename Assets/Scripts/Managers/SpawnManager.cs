using UnityEngine;

/// <summary>
/// 1. 던전 방에서 몬스터 소환을 관리
/// 2. 몬스터 웨이브를 처리
/// 4. 몬스터 소환 위치 관리
/// 5. 몬스터 소환 타이밍 관리
/// </summary>
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

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
