using UnityEngine;
using System.Collections;

public class HotSpring : MonoBehaviour
{
    [Header("회복 설정")]
    [Tooltip("한 번 회복할 때 채워줄 체력 양")]
    public float healAmount = 5f;

    [Tooltip("회복 주기 (초)")]
    public float healInterval = 1.0f;

    private Coroutine healCoroutine;

    // 트리거 범위에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        // 레이어 이름이 "Player"인지 확인
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("<color=cyan><b>[온천]</b> 플레이어가 온천에 들어왔습니다!</color>");

            if (healCoroutine == null)
            {
                healCoroutine = StartCoroutine(CoHealPlayer());
            }
        }
    }

    // 트리거 범위에서 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("<color=yellow><b>[온천]</b> 플레이어가 온천에서 나갔습니다.</color>");

            if (healCoroutine != null)
            {
                StopCoroutine(healCoroutine);
                healCoroutine = null;
            }
        }
    }

    // 지속 회복 코루틴
    private IEnumerator CoHealPlayer()
    {
        while (true)
        {
            if (Player.Instance != null)
            {
                Player.Instance.Heal(healAmount); // Player.cs의 Heal 함수 호출

                // 회복 후 현재 체력을 디버그로 표시 (Player.cs의 currentHp 참조)
                Debug.Log($"<color=green>[온천 회복 중]</color> 회복량: {healAmount}, 현재 체력: {Player.Instance.currentHp}");
            }

            yield return new WaitForSeconds(healInterval);
        }
    }
}