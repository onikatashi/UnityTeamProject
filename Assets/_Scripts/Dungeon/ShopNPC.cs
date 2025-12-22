using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 5f; // 상호작용 반경 (기본값 5m)
    [SerializeField] private KeyCode interactionKey = KeyCode.F; // 상호작용 키

    private void Update()
    {
        // 플레이어와 NPC 사이의 거리 계산 (Player.Instance 활용)
        if (Player.Instance == null) return;

        float distance = Vector3.Distance(transform.position, Player.Instance.transform.position);

        // 반경 이내에서 F키를 누르면 상점 오픈
        if (distance <= interactionRange)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                ShopManager.Instance.OpenShop();
            }
        }
    }

    // 에디터 뷰에서 반경을 시각적으로 확인하기 위함
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}