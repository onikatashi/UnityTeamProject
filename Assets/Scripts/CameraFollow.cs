using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 3f, -10f);
    public float maxDistance = 10f;
    public float minDistance = 0.5f;

    private int playerLayerID;
    private LayerMask raycastMask;

    void Start()
    {
        playerLayerID = LayerMask.NameToLayer("Player");
        if (playerLayerID == -1)
        {
            Debug.LogError("Layer 'Player'를 찾을 수 없습니다! Unity Layer 설정 필요. (CameraFollow.cs)");
            return;
        }

        // Player 레이어를 제외한 모든 레이어 충돌 검사
        raycastMask = ~(1 << playerLayerID);

        // 최초 플레이어 탐색
        FindPlayerByLayer();

        maxDistance = Mathf.Abs(offset.z);
    }

    void LateUpdate()
    {
        // 만약 target이 씬 전환 등으로 사라졌다면 다시 찾기
        if (target == null)
        {
            FindPlayerByLayer();
            if (target == null) return;
        }

        Vector3 desiredPosition = target.position + target.rotation * offset;

        Vector3 rayStart = target.position;
        Vector3 rayDirection = (desiredPosition - rayStart).normalized;
        float rayDistance = maxDistance;

        RaycastHit hit;

        if (Physics.Raycast(rayStart, rayDirection, out hit, rayDistance, raycastMask))
        {
            float adjustedDistance = hit.distance;
            float safeDistance = Mathf.Max(minDistance, adjustedDistance - 0.1f);
            transform.position = rayStart + rayDirection * safeDistance;
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.LookAt(target.position);
    }

    /// <summary>
    /// Player 레이어에 속한 오브젝트를 찾아 target으로 설정한다.
    /// </summary>
    private void FindPlayerByLayer()
    {
        // 씬 내 모든 Transform 검색
        Transform[] allObjects = FindObjectsOfType<Transform>();

        foreach (Transform obj in allObjects)
        {
            if (obj.gameObject.layer == playerLayerID)
            {
                target = obj;
                Debug.Log("CameraFollow: Player 레이어 오브젝트 찾음 → " + obj.name);
                return;
            }
        }

        Debug.LogWarning("CameraFollow: Player 레이어 오브젝트를 찾을 수 없습니다.");
    }
}
