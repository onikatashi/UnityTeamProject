using UnityEngine;

/// <summary>
/// 1. 씬 전환을 관리하는 매니저
/// 2. 씬 로드, 언로드, 전환 애니메이션 등을 처리
/// </summary>
public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    private void Awake()
    {
        if(Instance == null)
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
