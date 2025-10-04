using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Unity 내장 함수 - 컴포넌트가 활성화될 때 호출
    private void OnEnable()
    {
        Debug.Log("[BoardManager] OnEnable");
    }

    // Unity 내장 함수 - 컴포넌트가 비활성화될 때 호출
    private void OnDisable()
    {
        Debug.Log("[BoardManager] OnDisable");
    }

    // 애플리케이션 시작 시 1회 호출되는 정적 초기화 (스크립트 로드 확인용)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BootstrapLog()
    {
        Debug.Log("[BoardManager] RuntimeInitializeOnLoadMethod - Scripts loaded");
    }
    // 싱글톤 인스턴스 - 다른 스크립트에서 접근하기 위함
    public static BoardManager Instance;

    [Header("Prefabs")]
    public GameObject boardCellPrefab;   // 셀 프리팹 (BoardCell 포함된 오브젝트)

    [Header("Board Size (Columns x Rows per side)")]
    public int columns = 8;              // 가로 8칸
    public int rowsPerSide = 4;          // 각 진영 4줄

    [Header("Layout (Square Grid)")]
    public float cellSize = 1.0f;        // 셀 스케일(정사각 타일 한 변 길이)
    public float gap = 0.05f;            // 격자 간 여백

    [Header("Selection")]
    public GameObject selectedPiece;     // 현재 선택된 기물 (외부 UI/인풋에서 설정)

    // Unity 내장 함수 - 게임 오브젝트 생성 시 가장 먼저 호출
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Unity 내장 함수 - 첫 프레임 전에 1회 호출
    private void Start()
    {
        // 디버그: 프리팹 인식 확인
        if (boardCellPrefab == null)
        {
            Debug.LogError("[BoardManager] boardCellPrefab is NOT assigned in Inspector.");
        }
        else
        {
            var hasCell = boardCellPrefab.GetComponent<BoardCell>() != null;
            var hasRenderer = boardCellPrefab.GetComponentInChildren<SpriteRenderer>() != null;
            Debug.Log($"[BoardManager] Prefab assigned: {boardCellPrefab.name}, Has BoardCell: {hasCell}, Has SpriteRenderer: {hasRenderer}");
        }
        GenerateBoard();
    }

    // Unity 내장 함수 - 매 프레임 호출
    private void Update()
    {
        // 선택/배치와 관련된 실시간 갱신이 필요하면 추가
    }

    // 중요 함수 - 현재 선택된 기물 해제 (배치 완료 후 호출)
    public void ClearSelection()
    {
        selectedPiece = null;
    }

    // 중요 함수 - 아군 8x4, 적군 8x4 총 8x8 보드 생성, 서로 마주보도록 배치
    private void GenerateBoard()
    {
        if (boardCellPrefab == null)
        {
            Debug.LogError("[BoardManager] boardCellPrefab is not assigned.");
            return;
        }

        // 전체 보드 높이: 양 진영 합쳐 rowsPerSide * 2 (원래 방식)
        int totalRows = rowsPerSide * 2;

        // 프리팹 스프라이트 실측으로 격자 크기 계산
        var sampleSR = boardCellPrefab != null ? boardCellPrefab.GetComponentInChildren<SpriteRenderer>() : null;
        Vector2 spriteSize = sampleSR != null && sampleSR.sprite != null ? (Vector2)sampleSR.sprite.bounds.size : new Vector2(1f, 1f);

        // cellSize는 목표 월드 가로폭(픽셀/유닛 스프라이트 기준)을 의미한다고 가정
        // 스케일 계수: 스프라이트 가로폭을 cellSize에 맞춤
        float scaleFactor = (spriteSize.x > 0f) ? (cellSize / spriteSize.x) : 1f;
        float stride = cellSize + gap; // 정사각 격자 한 칸 간격

        // 전체 그리드의 바운딩 박스 대략 계산 (중앙 정렬용)
        float width = (columns - 1) * stride;
        float height = (totalRows - 1) * stride;
        Vector3 origin = transform.position - new Vector3(width * 0.5f, height * 0.5f, 0f);

        int created = 0;
        // 위쪽(적군) 행부터 아래쪽(아군) 행까지 생성하여 서로 마주보게 구성 (2D: XY 사용)
        for (int y = 0; y < totalRows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // 정사각 격자 배치 (오프셋 없음)
                Vector3 pos = origin + new Vector3(x * stride, y * stride, 0f);
                var go = Instantiate(boardCellPrefab, pos, Quaternion.identity, transform);

                var cell = go.GetComponent<BoardCell>();
                if (cell == null)
                {
                    cell = go.GetComponentInChildren<BoardCell>();
                }
                if (cell != null)
                {
                    // y 기준으로 위 절반은 Enemy, 아래 절반은 Ally (원래 방식)
                    bool isEnemy = y < rowsPerSide;
                    cell.teamSide = isEnemy ? BoardCell.TeamSide.Enemy : BoardCell.TeamSide.Ally;

                    // 팀별 로컬 인덱스 (원래 방식)
                    int localY = isEnemy ? (rowsPerSide - 1 - y) : (y - rowsPerSide);

                    cell.gridX = x;
                    cell.gridY = localY;

                    // 이름으로 인덱스 표시
                    go.name = $"Cell_{cell.teamSide}_X{x}_Y{localY}";

                    // 체스판 색상(격자) 적용: (x + y) 짝/홀로 번갈아 색상 지정
                    bool isLight = ((x + y) % 2 == 0);
                    Color light = Color.white;
                    Color dark = new Color(0.7f, 0.7f, 0.7f, 1f);
                    cell.ApplyColor(isLight ? light : dark);
                }
                else
                {
                    // 런타임에 BoardCell이 없으면 자동으로 추가하여 색상/인덱스가 적용되도록 처리
                    cell = go.AddComponent<BoardCell>();
                    // 기본값 설정 후 동일 로직 적용
                    bool isEnemy = y < rowsPerSide;
                    cell.teamSide = isEnemy ? BoardCell.TeamSide.Enemy : BoardCell.TeamSide.Ally;
                    int localY = isEnemy ? (rowsPerSide - 1 - y) : (y - rowsPerSide);
                    cell.gridX = x;
                    cell.gridY = localY;
                    go.name = $"Cell_{cell.teamSide}_X{x}_Y{localY}";

                    bool isLight = ((x + y) % 2 == 0);
                    Color light = Color.white;
                    Color dark = new Color(0.7f, 0.7f, 0.7f, 1f);
                    cell.ApplyColor(isLight ? light : dark);
                }

                // 2D 프리팹 스케일 조정 (Z는 1 유지) - 가로폭이 cellSize가 되도록 균일 스케일
                go.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
                created++;
            }
        }
        //Debug.Log($"[BoardManager] Generated square board: Columns:{columns}, RowsPerSide:{rowsPerSide} (TotalRows:{totalRows}), Created:{created}");
    }
}
