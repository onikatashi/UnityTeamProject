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

    Action<int> clickCallback;

    UIManager uiManager;

    private void Start()
    {
        uiManager = UIManager.Instance;
    }

    private void OnEnable()
    {
        if(selectedFrame != null)
        {
            selectedFrame.SetActive(false);
        }
    }

    public void SetUp(int index, Action<int> clickAction)
    {
        slotIndex = index;
        clickCallback = clickAction;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedFrame != null)
        {
            selectedFrame.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedFrame != null)
        {
            selectedFrame.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭감지");
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            clickCallback?.Invoke(slotIndex);
        }
    }
}
