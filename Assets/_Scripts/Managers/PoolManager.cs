using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 1. 오브젝트 풀링을 관리하는 매니저
/// 2. 투사체, 몬스터 오브젝트 풀링 등을 처리
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<Enums.PoolType, object> pools = new Dictionary<Enums.PoolType, object>();

    private void Awake()
    {
        // 씬 내에 오직 하나만 존재하도록 보장
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 씬 전환 후에 ObejctPool에서 obejct를 꺼내려 하면, 이전 씬에 저장되어있던 오브젝트를 가져오려고 함
    // 하지만 이 오브젝트는 씬이 바뀌면서 이미 파괴된 상태지만 Dictionary는 남아있기 때문에 오류가 발생
    // 그래서 씬을 넘어가기 전에 pool을 비워주는 작업을 실행.
    private void OnEnable()
    {
        SceneManager.sceneUnloaded += ClearPoolOnSceneChange;
    }

    private void ClearPoolOnSceneChange(UnityEngine.SceneManagement.Scene unused)
    {
        pools.Clear();
    }

    // 오브젝트 풀링이 필요한 곳에서 풀 생성
    public void CreatePool<T>(Enums.PoolType key, 
        T prefab, int initialSize, Transform parent = null) where T : Component
    {
        if (pools.ContainsKey(key))
        {
            Debug.Log("이미 풀이 존재");
            return;
        }

        ObjectPool<T> pool = new ObjectPool<T>(prefab, initialSize, parent);
        pools[key] = pool;
    }

    // 풀에서 꺼내기
    public T Get<T>(Enums.PoolType key) where T : Component
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogError($"{key}에 대한 풀이 없음");
            return null;
        }

        return ((ObjectPool<T>)pools[key]).Get();
    }

    // 풀로 반환
    public void Return<T>(Enums.PoolType key, T obj) where T : Component
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogError($"{key}에 대한 풀이 없음");
            obj.gameObject.SetActive(false);
            return;
        }

        ((ObjectPool<T>)pools[key]).Return(obj);
    }

    private void OnDestroy()
    {
        // 씬이 파괴될 때 정적 참조를 비워주어 메모리 누수 방지
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
