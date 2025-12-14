using System.Collections.Generic;
using UnityEngine;

public class BuffReceiver : MonoBehaviour
{
    struct BuffInfo
    {
        public float value;     // 공격: >1 배율, 방어: <1 피격배율
        public float endTime;
    }

    // 소스(버프 몬스터)별로 따로 저장 → 여러 마리 중첩
    Dictionary<int, BuffInfo> atkBuffs = new Dictionary<int, BuffInfo>(16);
    Dictionary<int, BuffInfo> defBuffs = new Dictionary<int, BuffInfo>(16);

    static readonly List<int> s_RemoveKeys = new List<int>(32);

    /// <summary>공격 배율(곱연산 중첩)</summary>
    public float AttackMultiplier => ComputeMultiplier(atkBuffs, defaultValue: 1f);

    /// <summary>피해 배율(곱연산 중첩) / 1보다 작을수록 덜 맞음</summary>
    public float DamageTakenMultiplier => ComputeMultiplier(defBuffs, defaultValue: 1f);

    public void ApplyAttackBuff(MonsterBase source, float multiplier, float duration)
        => ApplyBuff(atkBuffs, source, multiplier, duration);

    /// <param name="multiplier">0.8이면 20% 덜 맞음(피해 * 0.8)</param>
    public void ApplyDefenseBuff(MonsterBase source, float multiplier, float duration)
        => ApplyBuff(defBuffs, source, multiplier, duration);

    void ApplyBuff(Dictionary<int, BuffInfo> dict, MonsterBase source, float value, float duration)
    {
        if (source == null) return;

        int key = source.GetInstanceID();
        float end = Time.time + duration;

        if (dict.TryGetValue(key, out BuffInfo info))
        {
            // 같은 소스가 갱신하면 더 강한 값/더 긴 시간으로 갱신
            // 공격: 큰 값이 강함 / 방어(피해배율): 작은 값이 강함
            if (dict == defBuffs) info.value = Mathf.Min(info.value, value);
            else
            {
                info.value = Mathf.Max(info.value, value);
            }
            
            info.endTime = Mathf.Max(info.endTime, end);
            dict[key] = info;
        }
        else
        {
            dict[key] = new BuffInfo { value = value, endTime = end };
        }
    }

    float ComputeMultiplier(Dictionary<int, BuffInfo> dict, float defaultValue)
    {
        float mul = defaultValue;
        if (dict.Count == 0) return mul;

        s_RemoveKeys.Clear();

        foreach (var kv in dict)
        {
            if (Time.time >= kv.Value.endTime) s_RemoveKeys.Add(kv.Key);
            else mul *= kv.Value.value;
        }

        for (int i = 0; i < s_RemoveKeys.Count; i++)
            dict.Remove(s_RemoveKeys[i]);

        return mul;
    }
}
