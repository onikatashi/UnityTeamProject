using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // 이동할 다음 씬의 이름을 Inspector에서 설정
    public string targetSceneName;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그 확인 (플레이어 오브젝트에 "Player" 태그가 있어야 합니다)
        if (other.CompareTag("Player"))
        {
            // 중앙 SceneManager에게 씬 전환을 요청합니다.
            if (SceneManager.Instance != null)
            {
                // 충돌한 오브젝트(플레이어)를 전달
                SceneManager.Instance.TravelToScene(targetSceneName, other.gameObject);
            }
            else
            {
                Debug.LogError("SceneManager 인스턴스를 찾을 수 없습니다! 씬 전환 실패.");
            }
        }
    }
}