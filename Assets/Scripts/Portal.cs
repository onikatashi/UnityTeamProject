using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // 이동할 대상 씬 이름
    public string targetSceneName;

    private int playerLayer;

    private void Start()
    {
        // Player 레이어 번호 캐싱
        playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer == -1)
        {
            Debug.LogError("Portal: 'Player' 레이어가 존재하지 않습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 최상위 루트 확인
        Transform root = other.transform.root;

        // 루트 오브젝트가 Player 레이어인 경우만 처리
        if (root.gameObject.layer == playerLayer)
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                // 지정된 씬으로 이동
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError("Portal: targetSceneName이 설정되지 않았습니다.");
            }
        }
    }
}
