using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUnit : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("드래그 설정")]
    public Canvas canvas;
    public UnitData unitData;
    
    private Camera mainCamera;
    private Vector3 offset; // 마우스와 오브젝트 중심 사이의 거리
    private SpriteRenderer spriteRenderer;
    private Collider2D objectCollider;

    // 원래 상태 저장을 위한 변수
    private Vector3 originalPosition;
    private Transform originalParent;
    private int originalSortingOrder;
    
    void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();
    }
    
    /// <summary>
    /// 드래그 시작
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {

        if(spriteRenderer == null || objectCollider == null || mainCamera == null)
        {
            Debug.LogWarning($"{name}: 드래그를 시작할 수 없음 (컴포넌트 누락)");
            return;
        }
        originalPosition = transform.position;
        originalParent = transform.parent;
        originalSortingOrder = spriteRenderer.sortingOrder;

        Debug.Log($"{originalParent} : originalParent 확인");

        // 2. 마우스와 오브젝트 간격 계산 (오브젝트가 마우스 커서 중심으로 튀는 현상 방지)
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - mouseWorldPos;

        // 3. 드래그 중 시각적 피드백
        // 반투명 처리
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.6f);
        // 다른 오브젝트 위에 보이도록 Sorting Order를 높임
        spriteRenderer.sortingOrder = 100;
        // 레이캐스트 방해 방지 (드롭 위치를 감지하기 위함)
        objectCollider.enabled = false;

        Debug.Log($"{gameObject.name} 드래그 시작");
    }
    
    /// <summary>
    /// 드래그 중
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 따라 이동
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouseWorldPos.x + offset.x, mouseWorldPos.y + offset.y, originalPosition.z);
    }
    
    /// <summary>
    /// 드래그 종료
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        spriteRenderer.sortingOrder = originalSortingOrder;
        objectCollider.enabled = true;

        // 2. 드롭 위치에 어떤 오브젝트가 있는지 확인
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        foreach(var hit in hits)
        {
            Debug.Log("ㅡㅡㅡㅡㅡㅡㅡㅡ : " + hit.collider.gameObject.name); 
            if(hit.collider.GetComponent<BoardSlot>() != null)
            {
                BoardSlot boardSlot = hit.collider.GetComponent<BoardSlot>();
                Debug.Log($"{boardSlot} boardSlot 시작");
                HandleBoardDrop(boardSlot);
                break;

            }else if(hit.collider.GetComponent<BenchSlot>() != null){

                BenchSlot benchSlot = hit.collider.GetComponent<BenchSlot>();
                Debug.Log($"{benchSlot} benchSlot 시작");
                HandleBenchDrop(benchSlot);
                break;

            }else{

            }

            ReturnToOriginal();
        }


        // GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        
        // if(dropTarget != null) Debug.Log($"{dropTarget.name} 밴치 확인");

        // if (dropTarget != null)
        // {
        //     // DropSlot이라는 컴포넌트를 가진 슬롯에 드롭했는지 확인
        //     BenchSlot benchSlot = dropTarget.GetComponent<BenchSlot>();
        //     BoardSlot boardSlot = dropTarget.GetComponent<BoardSlot>();

        //     if (benchSlot != null)
        //     {
        //         HandleBenchDrop(benchSlot);
        //     }
        //     else if (boardSlot != null)
        //     {
        //         HandleBoardDrop(boardSlot);
        //     }
        //     else
        //     {
        //         // 잘못된 위치 → 원위치
        //         ReturnToOriginal();
        //     }
        // }
        // else
        // {
        //     // 허공에 드롭 -> 원래 위치로
        //     ReturnToOriginal();
        // }
    }
    
    /// <summary>
    /// 벤치 슬롯에 드롭
    /// </summary>
    void HandleBenchDrop(BenchSlot benchSlot)
    {
        if (benchSlot.isEmpty)
        {
            // 빈 슬롯 → 이동
            benchSlot.SetUnit(GetComponent<UnitBase>());
            ClearOriginalSlot();
            transform.SetParent(benchSlot.transform);
            transform.position = benchSlot.transform.position; 
            
            Debug.Log("BenchSlot 유닛을(를) 보드로 배치");
        }
        else
        {   
            // 찬 슬롯 → 교환
            UnitBase tempUnit = benchSlot.currentUnit;
            benchSlot.SetUnit(GetComponent<UnitBase>());
            SetOriginalSlotUnit(tempUnit);
            transform.SetParent(benchSlot.transform);
            transform.position = benchSlot.transform.position;
            tempUnit.transform.SetParent(originalParent.transform);
            tempUnit.transform.position = originalParent.transform.position;
            
            Debug.Log("BenchSlot 유닛교환");
        }
    }
    
    /// <summary>
    /// 보드 슬롯에 드롭
    /// </summary>
    void HandleBoardDrop(BoardSlot boardSlot)
    {
        if (boardSlot.isEmpty)
        {
            // 빈 슬롯 → 이동
            boardSlot.SetUnit(GetComponent<UnitBase>());
            ClearOriginalSlot();
            transform.SetParent(boardSlot.transform);
            transform.position = boardSlot.transform.position; 
            
            Debug.Log("BoardSlot 유닛을(를) 보드로 배치");
        }
        else
        {
            // 찬 슬롯 → 교환
            UnitBase tempUnit = boardSlot.currentUnit;
            boardSlot.SetUnit(GetComponent<UnitBase>());
            SetOriginalSlotUnit(tempUnit);
            transform.SetParent(boardSlot.transform);
            transform.position = boardSlot.transform.position;
            tempUnit.transform.SetParent(originalParent.transform);
            tempUnit.transform.position = originalParent.transform.position;
            
            Debug.Log("BoardSlot 유닛교환");
        }
    }
    
    /// <summary>
    /// 원래 슬롯 비우기
    /// </summary>
    void ClearOriginalSlot()
    {
        if (originalParent == null)
        {
            Debug.LogWarning($"{name}: originalParent가 null이라 슬롯을 비울 수 없음");
            return;
        }
        BenchSlot bench = originalParent.GetComponent<BenchSlot>();
        BoardSlot board = originalParent.GetComponent<BoardSlot>();
        
        if (bench != null) bench.ClearSlot();
        if (board != null) board.ClearSlot();
    }
    
    /// <summary>
    /// 원래 슬롯에 유닛 설정 (교환용)
    /// </summary>
    void SetOriginalSlotUnit(UnitBase unitBase)
    {
        BenchSlot bench = originalParent.GetComponent<BenchSlot>();
        BoardSlot board = originalParent.GetComponent<BoardSlot>();
        
        if (bench != null) bench.SetUnit(unitBase);
        if (board != null) board.SetUnit(unitBase);
    }
    
    /// <summary>
    /// 원위치로 되돌리기
    /// </summary>
    void ReturnToOriginal()
    {
        if (originalParent == null)
        {
            Debug.LogWarning($"{name}: originalParent가 null이라 원위치 복구 불가");
            return;
        }

        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        Debug.Log($"원위치");
    }
}