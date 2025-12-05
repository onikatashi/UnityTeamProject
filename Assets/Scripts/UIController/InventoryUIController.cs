using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public GameObject InventoryPanel;            // 인벤토리 패널 오브젝트
    public List<Image> itemIconsImage;           // 인벤토리 슬롯 아이콘 이미지 리스트

    InventoryManager inventoryManager;

    // InventoryUIController 초기화
    public void InitInventoryUIController()
    {
        inventoryManager = InventoryManager.Instance;
        FindLastChildObjectRecursive(InventoryPanel.transform);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 특정 부모의 모든 마지막 자식 오브젝트를 찾아 Image 컴포넌트를 리스트에 추가
    // ItemIcon 이미지를 업데이트하기 위한 작업
    void FindLastChildObjectRecursive(Transform transform)
    {
        if (transform.childCount == 0)
        {
            itemIconsImage.Add(transform.gameObject.GetComponent<Image>());
            return;
        }

        foreach (Transform child in transform)
        {
            FindLastChildObjectRecursive(child);
        }
    }

    // 인벤토리 슬롯 아이콘 업데이트
    public void UpdateItemIcon()
    {
        for (int i = 0; i < inventoryManager.Inventory.Length; i++)
        {
            if (inventoryManager.Inventory[i] != null)
            {
                itemIconsImage[i].sprite = inventoryManager.Inventory[i].iIcon;
            }
            else
            {
                itemIconsImage[i].sprite = null;
            }
        }
    }
}