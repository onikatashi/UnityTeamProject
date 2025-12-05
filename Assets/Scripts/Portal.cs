using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string targetSceneName;

    private int playerLayer;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");

        if (playerLayer == -1)
        {
            Debug.LogError("Layer 'Player'를 찾을 수 없습니다! Unity Layer 설정 필요. (Portal.cs)");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어인지 판별
        if (other.gameObject.layer == playerLayer)
        {
            // 플레이어가 씬 이동 중 끊기지 않도록 (원하면)
            DontDestroyOnLoad(other.gameObject);

            // 그냥 바로 씬 로드
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError("targetSceneName이 비어 있습니다! Inspector에서 설정하세요. (Portal.cs)");
            }
        }
    }
}
