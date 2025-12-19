using UnityEngine;
using System.Collections;

public class PlayerBuffController : MonoBehaviour
{
    private Player player;

    private float damageBuffMultiplier = 1.2f;

    private void Awake()
    {
        player = Player.Instance;
    }

    public IEnumerator ApplyDamageBuff(float multiplier, float duration)
    {
        damageBuffMultiplier *= multiplier;
        UpdateFinalStats();

        yield return new WaitForSecondsRealtime(duration);

        damageBuffMultiplier /= multiplier;
        UpdateFinalStats();
    }

    private void UpdateFinalStats()
    {
        player.SetFinalStat();
    }
}
