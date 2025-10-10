using UnityEngine;

public class BenchManager : MonoBehaviour
{
    public static BenchManager Instance { get; private set; }

    public GameObject tilePrefab;
    public Transform tileParent;
    public int tileCount = 8;
    public float spacing = 1.5f;

    private BenchTile[] tiles;

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
        tiles = new BenchTile[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            Debug.Log($"타일 생성 중: {i}");
            Vector3 pos = new Vector3(i * spacing - 5f, -3f, 0);
            GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
            tileObj.name = $"BenchTile_{i}";

            BenchTile tile = tileObj.GetComponent<BenchTile>();
            tile.index = i;
            tiles[i] = tile;
        }
    }

    public void OnTileClicked(BenchTile tile)
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

    public bool AddUnit(UnitBase unit)
    {
        foreach (var tile in tiles)
        {
            if (tile.currentUnit == null)
            {
                tile.currentUnit = unit;
                unit.transform.position = tile.transform.position + Vector3.up * 0.5f; // 시각적으로 위에 배치
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
