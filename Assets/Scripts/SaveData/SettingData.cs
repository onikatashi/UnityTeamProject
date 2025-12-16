using UnityEngine;

[System.Serializable]
// º¼·ı µ¥ÀÌÅÍ
public class SettingData
{
    // º¼·ı ¼öÄ¡
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;

    public SettingData()
    {
        masterVolume = 0.7f;
        bgmVolume = 0.5f;
        sfxVolume = 0.5f;
    }
}
