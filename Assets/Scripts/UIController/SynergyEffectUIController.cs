using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class SynergyEffectUIController : MonoBehaviour
{
    public GameObject synergyEffectUIPanel;             // 시너지 효과 설명 패널

    public Transform synergyEffectScroll;               // 시너지 효과 슬롯 프리팹의 부모
    public SynergySlotUI synergySlot;                   // 시너지 슬롯 프리팹
    public List<SynergySlotUI> synergySlotUIList;       // 활성화 되어있는 시너지 슬롯 UI를 저장하는 리스트

    PoolManager poolManager;
    InventoryManager inventoryManager;
    SynergyManager synergyManager;

    private void Awake()
    {
        poolManager = PoolManager.Instance;
        inventoryManager = InventoryManager.Instance;
        synergyManager = SynergyManager.Instance;

        poolManager.CreatePool<SynergySlotUI>(Enums.PoolType.SynergyEffects,
            synergySlot, 1, synergyEffectScroll);
        synergySlotUIList = new List<SynergySlotUI>();
    }


    public void ShowSynergyEffect()
    {
        synergyEffectUIPanel.SetActive(true);
        
        foreach (var keys in inventoryManager.synergyCount.Keys)
        {
            SynergySlotUI slot = poolManager.Get<SynergySlotUI>(Enums.PoolType.SynergyEffects);

            slot.synergyIcon.sprite = synergyManager.GetSynergyData(keys).synergyIcon;
            slot.synergyName.text = synergyManager.GetSynergyData(keys).synergyName;
            slot.activeSynergyCount.text = inventoryManager.synergyActiveCount[keys].ToString()
                + " / " + synergyManager.GetSynergyData(keys).maxLevel.ToString();

            synergySlotUIList.Add(slot);
        }
    }

    public void HideSynergyEffectUI()
    {
        ReturnSynergySlot();
        synergyEffectUIPanel.SetActive(false);
    }

    public void ReturnSynergySlot()
    {
        if (synergySlotUIList == null || synergySlotUIList.Count == 0) return;

        for (int i = 0; i < synergySlotUIList.Count; i++)
        {
            poolManager.Return(Enums.PoolType.SynergyEffects, synergySlotUIList[i]);
        }

        synergySlotUIList.Clear();
    }
}
