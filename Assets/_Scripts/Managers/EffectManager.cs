using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public List<EffectData> effectDatas;

    Dictionary<Enums.EffectType, EffectData> effectDict;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else Destroy(gameObject);
    }

    void Init()
    {
        effectDict = new Dictionary<Enums.EffectType, EffectData>();

        foreach (var data in effectDatas)
        {
            if (!effectDict.ContainsKey(data.effectType))
            {
                effectDict.Add(data.effectType, data);
            }
        }
    }

    public void PlayEffect(
        Enums.EffectType type,
        Vector3 pos,
        Quaternion rot,
        Transform followTarget = null)
    {
        if (!effectDict.ContainsKey(type))
        {
            Debug.LogWarning($"Effect Type {type}이 등록되지 않았음");
            return;
        }

        EffectData data = effectDict[type];

        GameObject fx = Instantiate(data.effectPrefab, pos, rot);

        if(data.followTarget && followTarget != null)
        {
            fx.transform.SetParent(followTarget);
            fx.transform.localPosition = Vector3.zero;
        }

        Destroy(fx, data.lifeTime);
    }
}
