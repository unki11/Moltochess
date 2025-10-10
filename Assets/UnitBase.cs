using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [Header("유닛 정보")]
    public string unitID;               // 유닛 ID
    public string unitName;             // 유닛 이름
    public int cost;                    // 비용
    public int maxHealth;               // 최대 체력
    public int attack;                  // 공격력
    public float attackRate;            // 초당 공격 수
    public float moveSpeed;             // 이동 속도
    public Sprite unitSprite;           // 유닛 스프라이트
    
    [Header("현재 상태")]
    public int currentHealth;           // 현재 체력
    public bool isAlive = true;         // 생존 여부
    
    [Header("전투 상태")]
    public UnitBase currentTarget;      // 현재 공격 대상
    private float attackTimer = 0f;     // 공격 쿨타임
    
    [Header("소속")]
    public bool isPlayerUnit = true;

    private SpriteRenderer spriteRenderer;

    public void Initialize(UnitData data, bool isPlayer = true)
    {
        // UnitData에서 정보를 복사
        unitID = data.unitID;
        unitName = data.unitName;
        cost = data.cost;
        maxHealth = data.maxHealth;
        attack = data.attack;
        attackRate = data.attackRate;
        moveSpeed = data.moveSpeed;
        unitSprite = data.unitSprite;
        
        isPlayerUnit = isPlayer;
        
        // 초기 스탯 설정
        currentHealth = maxHealth;
        isAlive = true;
        
        // SpriteRenderer 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // SpriteRenderer가 없으면 추가
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            Debug.Log("SpriteRenderer 컴포넌트를 추가했습니다.");
        }
        
        // 비주얼 설정
        if (unitSprite != null && spriteRenderer != null){
            Debug.Log($"이미지 생성 완료: {unitName}");
            spriteRenderer.sprite = unitSprite;
        }
        else
        {
            Debug.LogError($"이미지 설정 실패 - unitSprite: {unitSprite != null}, spriteRenderer: {spriteRenderer != null}");
        }
        // 이름 설정 (디버깅용)
        gameObject.name = unitName;
    }
    
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        
        currentHealth -= damage;
        
        // 체력 0 이하 시 사망
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        
        Debug.Log($"{unitName} 체력: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        isAlive = false;
        currentTarget = null;
        
        Debug.Log($"{unitName} 사망!");
        
        // 비주얼 효과 (투명도 조절)
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.3f;
            spriteRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("spriteRenderer가 null입니다. 비주얼 효과를 적용할 수 없습니다.");
        }
        
        // 나중에 사망 애니메이션이나 이펙트 추가 가능
    }

    public void RemoveUnit()
    {
        Destroy(gameObject);
    }

    public string GetUnitInfo()
    {
        return $"[{unitName}] HP: {currentHealth}/{maxHealth} | ATK: {attack} | Cost: {cost}";
    }

    void Start()
    {
        // Initialize가 호출되지 않은 경우를 대비한 기본값 설정
        if (string.IsNullOrEmpty(unitName))
        {
            Debug.LogWarning("UnitBase가 Initialize되지 않았습니다. Initialize 메서드를 먼저 호출하세요.");
            return;
        }
        
        // SpriteRenderer가 아직 초기화되지 않은 경우 초기화
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                Debug.Log("Start()에서 SpriteRenderer 컴포넌트를 추가했습니다.");
            }
        }
        
        Debug.Log("유닛 생성: " + unitName + " 체력: " + currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
