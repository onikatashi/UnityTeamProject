using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float destroyDistance = 20f;
    Vector3 startPosition;

    public Transform firepoint;
    MonsterData md;
    Monster2 monster2;
    float timer = 0;
    
    void Start()
    {
        startPosition = transform.position;
        md = GetComponent<MonsterData>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > md.attackSpeed)
        {
            fire();
        }

    }

    public void Init(Monster2 mt2)
    {
        this.monster2 = mt2;

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

        timer = 0;
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
