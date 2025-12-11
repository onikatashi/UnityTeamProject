using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int slotIndex;                       // 슬롯 인덱스
    public Image itemIcon;                      // 아이템 아이콘 이미지
    public Image rarityFrame;                   // 아이템 희귀도 프레임 이미지 -> 추후 추가
    public GameObject selectedFrame;            // 선택된 슬롯 프레임 이미지
    public TextMeshProUGUI reinforceLevel;      // 슬롯 강화 레벨 텍스트

    //Action<int> clickCallback;

    InventoryManager inventoryManager;
    UIManager uiManager;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        uiManager = UIManager.Instance;
    }

    private void OnEnable()
    {
        if(selectedFrame != null)
        {
            selectedFrame.SetActive(false);
        }
    }

    public void SetUp(int index)
    {
        slotIndex = index;
        //clickCallback = clickAction;
    }

    // 아이템 슬롯에 마우스 커서 들어가면 아이템 설명창 활성화
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedFrame != null)
        {
            selectedFrame.SetActive(true);
            uiManager.itemDescriptionUIController.
                ShowItemDescription(inventoryManager.Inventory[slotIndex]);
            uiManager.itemDescriptionUIController.slotIndex = slotIndex;
        }
    }

    // 아이템 슬롯에서 마우스 커서 나가면 아이템 설명창 비활성화
    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedFrame != null)
        {
            selectedFrame.SetActive(false);
            // 프리팹을 아이템 풀에 다시 돌려주는 작업
            uiManager.itemDescriptionUIController.HideItemDescription();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Swap 할 코드 넣어야함
    }
}
