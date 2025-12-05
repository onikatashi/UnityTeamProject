using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // public -> private으로 변경. 인스펙터에서 사라짐.
    private Transform target;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 3f, -10f); // 기본 오프셋
    public float maxDistance = 10f;                     // 추가: 카메라의 최대 거리 (offset.z의 절대값과 일치)
    public float minDistance = 0.5f;                    // 추가: 카메라가 플레이어에게 접근할 수 있는 최소 거리

    // 플레이어 오브젝트에 설정된 태그
    private const string PlayerTag = "Player";

    void Start()
    {
        // offset의 Z축 값은 카메라의 후방 거리를 나타내므로, maxDistance와 연동하여 설정합니다.
        // offset.z는 일반적으로 음수이므로, maxDistance는 그 절대값으로 초기화합니다.
        maxDistance = Mathf.Abs(offset.z);
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag(PlayerTag);

            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollow: Could not find object with tag " + PlayerTag + ".");
                return;
            }
        }

        // 카메라의 목표 위치 계산 (플레이어로부터 maxDistance만큼 떨어진 지점)
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // -----------------------------
        // Raycast를 이용한 벽 충돌 검사
        // -----------------------------

        // 1. Raycast의 시작점과 방향 설정
        // 시작점: 플레이어 위치
        // 방향: 플레이어에서 원하는 카메라 위치로 향하는 방향
        Vector3 rayStart = target.position;
        Vector3 rayDirection = (desiredPosition - rayStart).normalized;
        float rayDistance = maxDistance; // Ray의 최대 길이 (원래 원하는 카메라 거리)

        RaycastHit hit;

        // 2. Raycast 수행: 플레이어에서 원하는 카메라 위치까지 장애물이 있는지 검사
        // minDistance보다 작은 거리의 충돌은 무시하고 싶으므로, maxDistance를 사용
        if (Physics.Raycast(rayStart, rayDirection, out hit, rayDistance))
        {
            // 충돌 감지! Ray가 벽에 닿음.

            // 3. 충돌 지점까지의 거리 계산
            float adjustedDistance = hit.distance;

            // 4. 최소 거리(minDistance)보다 가까워지지 않도록 조정
            // (충돌 지점은 Collider 겉면이므로 약간의 여유를 둡니다: - 0.1f)
            float safeDistance = Mathf.Max(minDistance, adjustedDistance - 0.1f);

            // 5. 카메라의 새로운 위치를 충돌 지점 앞(safeDistance)으로 설정
            transform.position = rayStart + rayDirection * safeDistance;
        }
        else
        {
            // 충돌 없음: 원하는 최대 거리(maxDistance)까지 이동
            transform.position = desiredPosition;
        }

        // -----------------------------
        // LookAt은 그대로 유지
        // -----------------------------
        transform.LookAt(target.position);
    }
}