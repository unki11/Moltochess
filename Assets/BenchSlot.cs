using UnityEngine;
using UnityEngine.UI;

public class BenchSlot : MonoBehaviour
{

    [Header("UI 컴포넌트")]
    public Image backgroundImage;
    public Image unitIconImage;
    public GameObject emptyIndicator;
    
    [Header("상태")]
    public bool isEmpty = true;
    public UnitData currentUnitData;

    public UnitBase currentUnit;
    
    [Header("프리팹")]
    public GameObject draggableUnitPrefab;  // DraggableUnit 프리팹
    
    private GameObject currentUnitObject;
    public int index;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {

    }

    public void SetUnit(UnitBase unitBase)
    {
        // 기존 유닛 오브젝트 삭제
        if (unitBase != null)
        {
            currentUnit = unitBase;
        }
        
        isEmpty = false;
        
        // 빈 슬롯 표시 숨김
        if (emptyIndicator != null)
        {
            emptyIndicator.SetActive(false);
        }
        
    }

    
    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void ClearSlot()
    {
        // 유닛 오브젝트 삭제
        if (currentUnitObject != null)
        {
            Destroy(currentUnitObject);
        }
        
        currentUnitData = null;
        currentUnit = null;
        isEmpty = true;
    }

    private void OnMouseDown()
    {
        // 클릭 시 BenchManager로 이벤트 전달
        BenchManager.Instance.OnTileClicked(this);
    }

    public void SetHighlight(bool active)
    {
        sr.color = active ? Color.yellow : Color.white;
    }

    public void SetUnitSprite(Sprite sprite)
    {
        sr.sprite = sprite;
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
