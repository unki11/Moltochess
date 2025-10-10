using UnityEngine;

public class BenchTile : MonoBehaviour
{
    public int index;
    public UnitBase currentUnit;  // 슬롯에 있는 유닛 (없으면 null)
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
}
