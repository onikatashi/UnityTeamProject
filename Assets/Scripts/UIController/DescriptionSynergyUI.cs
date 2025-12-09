using UnityEngine;

public class DescriptionSynergyUI : MonoBehaviour
{
    public Sprite synergyIcon;
    public string synergyName;

    public void SetUp(Sprite synergyIcon, string synergyName)
    {
        this.synergyIcon = synergyIcon;
        this.synergyName = synergyName;
    }
}
