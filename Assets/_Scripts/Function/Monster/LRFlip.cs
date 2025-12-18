using UnityEngine;

public class LRFlip : MonoBehaviour
{
    [Header("Detect")]
    public string playerLayerName = "Player";
    public float detectRange = 10f;

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer; 
    public bool invertFlip = false;     

    int playerLayer;
    int playerMask;
    Transform playerTr;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        
        playerMask = 1 << playerLayer;

        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        playerTr = FindPlayerInRange();

        if (playerTr != null) FlipToward(playerTr.position);
    }

    Transform FindPlayerInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, playerMask);

        if (hits == null || hits.Length == 0) return null;

        
        float best = float.PositiveInfinity;
        Transform bestTr = null;

        for (int i = 0; i < hits.Length; i++)
        {
            float d = (hits[i].transform.position - transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                bestTr = hits[i].transform;
            }
        }

        return bestTr;
    }

    void FlipToward(Vector3 targetPos)
    {
        
        bool targetIsLeft = targetPos.x < transform.position.x;

        bool flip = targetIsLeft;

        if (invertFlip) flip = !flip;

        if (spriteRenderer != null) spriteRenderer.flipX = flip;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
