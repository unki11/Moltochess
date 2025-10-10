using UnityEngine;

public class BenchManager : MonoBehaviour
{
    public static BenchManager Instance { get; private set; }

    public GameObject tilePrefab;
    public Transform tileParent;
    public int tileCount = 8;
    public float spacing = 1.5f;

    private BenchSlot[] tiles;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GenerateTiles();
    }

    void GenerateTiles()
    {
        Debug.Log($"타일 생성 시작, tileCount = {tileCount}");
        tiles = new BenchSlot[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            Debug.Log($"타일 생성 중: {i}");
            Vector3 pos = new Vector3(i * spacing - 5f, -3f, 0);
            GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
            tileObj.name = $"BenchSlot_{i}";

            BenchSlot tile = tileObj.GetComponent<BenchSlot>();
            tile.index = i;
            tiles[i] = tile;
        }
    }

    public void OnTileClicked(BenchSlot tile)
    {
        if (tile.currentUnit != null)
        {
            Debug.Log($"[벤치] {tile.index}번 유닛 클릭: {tile.currentUnit.unitName}");
            // TODO: 드래그 or 전장 이동 기능
        }
        else
        {
            Debug.Log($"[벤치] {tile.index}번 슬롯 비어있음");
        }

        // 클릭 시 시각적 표시
        foreach (var t in tiles)
            t.SetHighlight(t == tile);
    }

    public bool AddUnit(UnitData unit)
    {
        foreach (var tile in tiles)
        {
            if (tile.currentUnit == null)
            {
                Vector2 spawnPos = new Vector2(-3f, 0f);
                UnitBase currentUnit = GameManager.Instance.SpawnUnit(unit, spawnPos, true);
                currentUnit.transform.SetParent(tile.transform);
                currentUnit.transform.position = tile.transform.position; // 시각적으로 위에 배치
                tile.currentUnit = currentUnit;
                tile.isEmpty = false;
                return true;
            }
        }
        Debug.Log("⚠️ 벤치가 가득 찼습니다.");
        return false;
    }

    /// <summary>
    /// tiles의 currentUnit 중 하나라도 존재하면 true, 전부 null이면 false
    /// </summary>
   public bool HasAnyEmptyTile()
    {
        if (tiles == null || tiles.Length == 0)
            return false;

        foreach (var tile in tiles)
        {
            if (tile != null && tile.currentUnit == null)
            {
                return true;
            }
        }
        
        return false;
    }
}
