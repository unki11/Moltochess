using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 컴포넌트")]
    public Image unitIconImage;         // 유닛 아이콘
    public TextMeshProUGUI  unitNameText;           // 유닛 이름
    public TextMeshProUGUI  unitCostText;           // 유닛 비용    
    private UnitData currentUnit;

    void Awake()
    {

    }
    
    /// <summary>
    /// 슬롯에 유닛 설정
    /// </summary>
    public void SetUnit(UnitData unitData)
    {
        currentUnit = unitData;
        
        Debug.Log($"SetUnit 호출: {unitData.unitName}");
        
        // 유닛 아이콘
        if (unitIconImage != null)
        {
            if (unitData.icon != null)
            {
                unitIconImage.sprite = unitData.icon;
                unitIconImage.color = Color.white;
                Debug.Log($"아이콘 설정: {unitData.icon.name}");
            }
            else
            {
                // 아이콘 없으면 코스트별 색상
                unitIconImage.sprite = null;
                unitIconImage.color = GetColorByCost(unitData.cost);
                Debug.Log($"색상으로 표시: {GetColorByCost(unitData.cost)}");
            }
        }
        
        // 유닛 이름
        if (unitNameText != null)
        {
            unitNameText.text = unitData.unitName;
            Debug.Log($"이름 설정: {unitData.unitName}");
        }
        else
        {
            Debug.LogError("unitNameText가 null입니다!");
        }
        
        // 유닛 비용
        if (unitCostText != null)
        {
            unitCostText.text = $"{unitData.cost}G";
            Debug.Log($"비용 설정: {unitData.cost}G");
        }
        else
        {
            Debug.LogError("unitCostText가 null입니다!");
        }
    }
    
    /// <summary>
    /// 패널 클릭 시 이 슬롯의 유닛 하나를 소환
    /// GameManager.SpawnUnit 로직 사용
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance 를 찾을 수 없습니다.");
            return;
        }
        if (currentUnit == null)
        {
            Debug.LogWarning("현재 슬롯에 설정된 유닛이 없습니다.");
            return;
        }
        if (BenchManager.Instance.HasAnyEmptyTile() == false)
        {
            Debug.LogWarning("벤치에 남은 자리가 없습니다.");
            return;
        }
        
        BenchManager.Instance.AddUnit(currentUnit);
        Debug.Log($"소환: {currentUnit.unitName}");
    }
    
    /// <summary>
    /// 코스트별 색상
    /// </summary>
    Color GetColorByCost(int cost)
    {
        switch (cost)
        {
            case 1: return new Color(0.7f, 0.7f, 0.7f); // 회색
            case 2: return new Color(0.3f, 1f, 0.3f);   // 초록
            case 3: return new Color(0.3f, 0.5f, 1f);   // 파랑
            default: return Color.white;
        }
    }
}