using UnityEngine;

//소환 테스트용 코드입니다. 실제로는 쓰이지 않습니다.
public class EnemySample : MonoBehaviour
{
    // 플레이어 추적 속도
    public float moveSpeed = 5f;

    // 플레이어 오브젝트를 찾기 위한 변수
    private Transform playerTransform;

    void Start()
    {
        // 씬에서 'Player' 태그를 가진 오브젝트를 찾아 Transform 컴포넌트를 저장합니다.
        // 게임 시작 시 한 번만 실행되므로 성능에 효율적입니다.
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player tag가 설정된 오브젝트를 찾을 수 없습니다. (EnemySample.cs)");
        }
    }

    void Update()
    {
        // 플레이어 트랜스폼이 유효한 경우에만 이동 로직을 실행합니다.
        if (playerTransform != null)
        {
            // 1. 목표 방향 계산
            // 현재 위치에서 플레이어 위치를 뺀 벡터가 목표 방향입니다.
            Vector3 direction = playerTransform.position - transform.position;

            // 2. 방향 벡터 정규화
            // 이동 속도를 일정하게 유지하기 위해 벡터의 크기를 1로 만듭니다.
            direction.Normalize();

            // 3. 이동
            // Rigidbody를 사용하지 않고 Transform으로 직접 이동합니다.
            // *참고: Rigidbody를 사용하여 물리 기반 이동을 하는 것이 더 권장될 수 있습니다.
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    // 다른 Collider와 충돌했을 때 호출됩니다.
    // 적 오브젝트와 플레이어 오브젝트 모두 Rigidbody와 Collider가 있으므로,
    // 이 메서드를 사용하여 충돌 처리를 합니다.
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트의 태그를 확인합니다.
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어와 충돌했으면 이 오브젝트(적)를 파괴합니다.
            Destroy(gameObject);
        }
    }
}