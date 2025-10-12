using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "AutoChess/Unit Data")]
public class UnitData : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string unitID; //유닛 아이디
    public string unitName; //유닛 이름
    public int cost; //비용
    public int maxHealth; //최대 체력
    public int currentHealth; //현재 체력
    public int attack; //공격력
    public float attackRate; // 초당 공격 수
    public float moveSpeed; //이동 속도
    public Sprite icon; //아이콘

    public float attackRange; //공격 

    public Sprite unitSprite;
    
}
