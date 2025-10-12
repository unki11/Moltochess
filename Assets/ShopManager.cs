using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("유닛 데이터 (씬에서 직접 할당)")]
    public List<UnitData> allUnits;         // Inspector에서 직접 드래그
    
    [Header("UI 참조")]
    public ShopSlot[] shopSlots;            // 상점 슬롯 3개

    [Header("UI 설정")]
    public RectTransform sellZoneRect; 
    
    private List<UnitData> currentShopUnits = new List<UnitData>();
    
    void Start()
    {
        // 초기 상점 생성
        RefreshShop();
    }

    private void Awake()
    {
        // A. 이미 인스턴스가 존재하고, 그 인스턴스가 자신이 아니라면 (중복 생성 시도)
        if (Instance != null && Instance != this)
        {
            // 중복된 객체(자신)를 파괴하여 단일 인스턴스 보장
            Destroy(gameObject);
            return; // 이후 코드는 실행하지 않음
        }

        // B. 인스턴스가 아직 없다면, 현재 객체를 싱글톤 인스턴스로 지정
        Instance = this;

        // C. (선택 사항) 씬이 바뀌어도 파괴되지 않게 하려면 이 코드를 추가합니다.
        // DontDestroyOnLoad(gameObject); 

        Debug.Log("ShopManager 싱글톤 인스턴스 초기화 완료.");
    }

    public bool TrySellUnit(GameObject unitObject, Vector2 screenPoint)
    {
        // 1. 판매 영역 RectTransform이 할당되었는지 확인합니다.
        if (sellZoneRect == null)
        {
            Debug.LogError("Sell Zone RectTransform이 BattleManager에 할당되지 않았습니다.");
            return false;
        }

        // 2. RectTransformUtility를 사용하여 마우스 위치가 판매 영역 내에 있는지 확인합니다.
        // ScreenPoint: 마우스 드롭 위치 (스크린 좌표)
        // Camera: UI 렌더링에 사용되는 카메라 (Canvas 설정에 따라 null일 수 있음)
        // isWorldCamera: null을 사용하여 캔버스가 Screen Space - Overlay 모드일 때 작동
        bool isInsideSellZone = RectTransformUtility.RectangleContainsScreenPoint(
            sellZoneRect, 
            screenPoint, 
            null // Screen Space - Overlay 캔버스 모드인 경우 null
        );
        
        Debug.Log("위치 값 : " + isInsideSellZone);

        if (isInsideSellZone)
        {
            UnitBase unit = unitObject.GetComponent<UnitBase>();

            if (unit != null)
            {
                PerformSell(unit);
                return true;
            }
        }

        return false;
    }

    public void PerformSell(UnitBase unit)
    {
        Destroy(unit.gameObject);
        // // 1. 유닛 리스트에서 제거
        // if (allyUnits.Contains(unit))
        // {
        //     allyUnits.Remove(unit);
        // }
        
        // // 2. 판매 금액 계산 및 재화 획득 (예시: 유닛 비용의 80%)
        // int sellValue = Mathf.FloorToInt(unit.cost * 0.8f); 
        
        // // BattleManager나 GameManager의 재화 변수에 추가
        // // GameManager.Instance.AddGold(sellValue); 
        
        // Debug.Log($"유닛 '{unit.unitName}' 판매 완료. {sellValue} 골드 획득.");
    }
    
    /// <summary>
    /// 상점 갱신 (랜덤 유닛 3개 선택)
    /// </summary>
    public void RefreshShop()
    {
        Debug.Log("--- RefreshShop() 실행 시작 ---");
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            RefreshShop();
        }
    }
}