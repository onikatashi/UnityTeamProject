using UnityEngine;

// 소환 테스트용 코드입니다. 실제로는 쓰이지 않습니다.
public class EnemySample : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Transform playerTransform;
    private int playerLayer;

    void Start()
    {
        // "Player" 레이어 ID 가져오기
        playerLayer = LayerMask.NameToLayer("Player");

        if (playerLayer == -1)
        {
            Debug.LogError("Layer 'Player'를 찾을 수 없습니다. Unity Layer 설정 확인 필요.");
            return;
        }

        // 플레이어 탐색
        FindPlayerByLayer();
    }

    void Update()
    {
        // 플레이어가 씬 이동/리스폰으로 사라졌다면 다시 탐색
        if (playerTransform == null)
            FindPlayerByLayer();

        if (playerTransform != null)
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.Normalize();

            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void FindPlayerByLayer()
    {
        Transform[] allObjects = FindObjectsOfType<Transform>();

        foreach (Transform obj in allObjects)
        {
            if (obj.gameObject.layer == playerLayer)
            {
                playerTransform = obj;
                Debug.Log("EnemySample: Player 레이어 오브젝트 찾음 → " + obj.name);
                return;
            }
        }

        Debug.LogWarning("EnemySample: Player 레이어 오브젝트를 찾을 수 없습니다.");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            Destroy(gameObject);
        }
    }
}
