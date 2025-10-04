using UnityEngine;

public class BoardCell : MonoBehaviour
{
    // 체스판 진영 구분 (아군/적군)
    public enum TeamSide { Ally, Enemy }

    [Header("Cell Info")]
    public TeamSide teamSide;          // 이 칸이 속한 진영
    public int gridX;                  // 가로 인덱스 (0~)
    public int gridY;                  // 세로 인덱스 (0~)

    [Header("Runtime State")]
    public GameObject occupant;        // 현재 이 칸에 놓인 기물(없으면 null)

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

    // 중요 함수 - 유저가 셀을 클릭했을 때 호출되는 빌트인 이벤트
    // (Collider가 있어야 동작하며, 카메라 Raycast가 닿는 경우에만 호출)
    private void OnMouseUpAsButton()
    {
        // 선택된 기물이 있으면 이 칸의 중앙에 스냅하여 배치
        var manager = BoardManager.Instance;
        if (manager == null) return;

        if (occupant != null) return;            // 이미 점유된 칸이면 무시
        if (manager.selectedPiece == null) return; // 선택된 기물이 없으면 무시

        PlacePiece(manager.selectedPiece);
        manager.ClearSelection();
    }

    // 중요 함수 - 외부에서 기물을 이 칸 중앙에 배치하고 로그를 남김
    public void PlacePiece(GameObject piece)
    {
        if (piece == null) return;
        if (occupant != null) return; // 중복 배치 방지

        // 칸의 월드 중앙으로 스냅 (2D: XY 평면 사용, Z는 유지/0으로 고정 권장)
        piece.transform.SetParent(transform);
        var p = transform.position;
        piece.transform.position = new Vector3(p.x, p.y, 0f);
        piece.transform.rotation = Quaternion.identity;

        occupant = piece;

        // 배치 로그 출력 (인덱스와 월드 좌표)
        //Debug.Log($"[BoardCell] Placed piece at Team:{teamSide} X:{gridX} Y:{gridY} World:{transform.position}");
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
