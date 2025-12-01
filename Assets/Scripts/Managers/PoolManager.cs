using UnityEngine;

/// <summary>
/// 1. 오브젝트 풀링을 관리하는 매니저
/// 2. 투사체, 몬스터 오브젝트 풀링 등을 처리
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

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
