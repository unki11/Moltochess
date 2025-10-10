using UnityEngine;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    public Image backgroundImage;
    public Image unitIconImage;
    
    [Header("상태")]
    public bool isEmpty = true;
    public UnitBase currentUnit;

    
    [Header("색상")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;
    
    void Start()
    {
        ClearSlot();
    }
    
    /// <summary>
    /// 유닛 배치

    public void SetUnit(UnitBase unitBase)
    {
        if (unitBase != null)
        {
            currentUnit = unitBase;
        }
        isEmpty = false;
        
        if (unitIconImage != null)
        {
            unitIconImage.gameObject.SetActive(true);
            
            // if (unitData.icon != null)
            // {
            //     unitIconImage.sprite = unitData.icon;
            // }
            // else
            // {
            //     unitIconImage.color = GetColorByCost(unitData.cost);
            // }
        }
    }
    
    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void ClearSlot()
    {
        currentUnit = null;
        isEmpty = true;
        
        if (unitIconImage != null)
        {
            unitIconImage.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 하이라이트 표시
    /// </summary>
    public void Highlight(bool show)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = show ? highlightColor : normalColor;
        }
    }
    
    Color GetColorByCost(int cost)
    {
        switch (cost)
        {
            case 1: return new Color(0.7f, 0.7f, 0.7f);
            case 2: return new Color(0.3f, 1f, 0.3f);
            case 3: return new Color(0.3f, 0.5f, 1f);
            default: return Color.white;
        }
    }
}