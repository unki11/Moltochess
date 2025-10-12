using UnityEngine;

public class BoardSlot : MonoBehaviour
{
    // 체스판 진영 구분 (아군/적군)
    public enum TeamSide { Ally, Enemy }

    [Header("Cell Info")]
    public TeamSide teamSide;          // 이 칸이 속한 진영
    public int gridX;                  // 가로 인덱스 (0~)
    public int gridY;                  // 세로 인덱스 (0~)

    [Header("Runtime State")]
    public GameObject occupant;        // 현재 이 칸에 놓인 기물(없으면 null)

    [Header("상태")]
    public bool isEmpty = true;
    public UnitData currentUnitData;

    public UnitBase unitBase;

    // Unity 내장 함수 - 오브젝트가 생성될 때(활성화 직후) 1회 호출
    // 초기화, 컴포넌트 보장 등에 사용
    private void Awake()
    {
        // 2D 클릭 이벤트를 받기 위해 Collider2D가 필요합니다. 없으면 자동 추가
        if (GetComponent<Collider2D>() == null)
        {
            // 육각형(뾰족) 2D 오브젝트에 적합한 폴리곤 콜라이더를 기본 추가
            gameObject.AddComponent<PolygonCollider2D>();
        }
    }

    // Unity 내장 함수 - 첫 프레임 전에 1회 호출
    // 외부 매니저(예: BoardManager) 의 생성 이후 의존 초기화가 필요할 때 사용
    private void Start()
    {
        // 필요 시 런타임 설정을 여기에 추가
    }

    // Unity 내장 함수 - 매 프레임 호출
    private void Update()
    {
        // 셀 자체는 매 프레임 갱신할 내용이 현재는 없습니다
    }

    public void SetUnit(UnitBase unitBase)
    {

        if (unitBase != null)
        {
            this.unitBase = unitBase;
        }
        
        isEmpty = false;
    }

    public void ClearSlot()
    {        
        currentUnitData = null;
        unitBase = null;
        isEmpty = true;
    }

 

    // 중요 함수 - 셀의 색상을 변경 (SpriteRenderer 필요)
    public void ApplyColor(Color color)
    {
        // Unity 내장 함수 활용: 모든 하위 SpriteRenderer에 색상 적용
        var srs = GetComponentsInChildren<SpriteRenderer>(true);
        if (srs != null && srs.Length > 0)
        {
            for (int i = 0; i < srs.Length; i++)
            {
                srs[i].color = color;
            }
        }
        else
        {
            //Debug.LogWarning($"[BoardCell] No SpriteRenderer found to apply color at Team:{teamSide} X:{gridX} Y:{gridY}");
        }
        //Debug.Log($"[BoardCell] Applied color {color} to Team:{teamSide} X:{gridX} Y:{gridY}");
    }
}
