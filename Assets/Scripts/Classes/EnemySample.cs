using UnityEngine;

public class EnemySample : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Transform player;
    private int playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");

        if (playerLayer == -1)
        {
            Debug.LogError("EnemySample: 'Player' 레이어 없음.");
            return;
        }

        if (Player.Instance != null)
        {
            player = Player.Instance.transform;
        }
    }

    private void Update()
    {
        if (player == null && Player.Instance != null)
        {
            player = Player.Instance.transform;
        }

        if (player != null)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌체의 루트 오브젝트 가져오기
        Transform root = collision.transform.root;

        // null 체크 추가
        if (root == null || root.gameObject == null) return;

        // 레이어 체크
        if (root.gameObject.layer == playerLayer)
        {
            // 충돌 시 Destroy(gameObject) 전에 혹시 모를 Null 체크 추가
            if (this != null && gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
