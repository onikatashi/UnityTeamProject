using UnityEngine;

public class BossBullet : MonoBehaviour
{
    MonsterBase owner;
    Vector3 dir;
    float speed;

    bool willSplit;
    float splitDistance;
    int splitCount;
    Vector3 startPos;

    int playerLayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    public void InitSimple(MonsterBase owner, Vector3 dir, float speed)
    {
        this.owner = owner;
        this.dir = dir.normalized;
        this.speed = speed;
        willSplit = false;
        startPos = transform.position;
    }

    public void InitStraight(MonsterBase owner, Vector3 dir, float speed, float splitDistance, int splitCount)
    {
        InitSimple(owner, dir, speed);
        willSplit = true;
        this.splitDistance = splitDistance;
        this.splitCount = splitCount;
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;

        if (willSplit && (transform.position - startPos).sqrMagnitude >= splitDistance * splitDistance)
        {
            Split();
            Destroy(gameObject);
            return;
        }
    }

    void Split()
    {
        float step = 360f / splitCount;
        for (int i = 0; i < splitCount; i++)
        {
            float ang = i * step;
            Vector3 d = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), 0f, Mathf.Sin(ang * Mathf.Deg2Rad));
            BossBullet child = Instantiate(this, transform.position, Quaternion.LookRotation(d));
            child.InitSimple(owner, d, speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != playerLayer) return;

        Player p = other.GetComponent<Player>();
        if (p != null && owner != null && owner.md != null)
        {
            p.TakeDamage(owner.md.attackDamage);
            Destroy(gameObject);
        }
    }
}
