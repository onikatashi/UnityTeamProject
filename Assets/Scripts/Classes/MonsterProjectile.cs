using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float destroyDistance = 20f;
    Vector3 moveDir;
    Vector3 startPosition;
    Monster2 monster2;
    int playerLayer;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void Update()
    {
        fire();
    }

    public void Init(Monster2 mt2, Vector3 dir)
    {
        this.monster2 = mt2;
        startPosition = transform.position;
        moveDir = dir.normalized;
        gameObject.SetActive(true);

    }

    public void fire()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        float sqrDistaance = (startPosition - transform.position).sqrMagnitude;
        if (sqrDistaance > destroyDistance * destroyDistance)
        {
            ReloadPool();
            return;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerLayer)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(monster2.md.attackDamage);
            }
        }
    }
    

    public void ReloadPool()
    {
        if (monster2 != null)
        {
            monster2.ReturnMonsterProjectile(this);
        }
        else
        {
            //몬스터가 죽어도 계속 날아가려면 이거 지워야함 아마도 
            Destroy(gameObject);
        }
    }
}
