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
        Transform root = collision.transform.root;

        if (root.gameObject.layer == playerLayer)
        {
            Destroy(gameObject);
        }
    }
}
