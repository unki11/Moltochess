using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("유닛 데이터 (씬에서 직접 할당)")]
    public List<UnitData> allUnits;         // Inspector에서 직접 드래그
    
    [Header("UI 참조")]
    public ShopSlot[] shopSlots;            // 상점 슬롯 3개
    
    private List<UnitData> currentShopUnits = new List<UnitData>();
    
    void Start()
    {
        // 초기 상점 생성
        RefreshShop();
    }
    
    /// <summary>
    /// 상점 갱신 (랜덤 유닛 3개 선택)
    /// </summary>
    public void RefreshShop()
    {
        currentShopUnits.Clear();
        
        // 랜덤하게 유닛 3개 선택
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (allUnits.Count > 0)
            {
                UnitData randomUnit = allUnits[Random.Range(0, allUnits.Count)];
                currentShopUnits.Add(randomUnit);
            }
        }
        
        // UI 업데이트
        UpdateShopUI();
        
        Debug.Log("상점 갱신 완료!");
    }
    
    /// <summary>
    /// 상점 UI 업데이트
    /// </summary>
    void UpdateShopUI()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (i < currentShopUnits.Count)
            {
                shopSlots[i].SetUnit(currentShopUnits[i]);
            }
        }
    }

    void Update()
    {
        // Space 키로 Knight 소환 테스트
        if (Input.GetKeyDown(KeyCode.T))
        {
            RefreshShop();
        }
    }
}