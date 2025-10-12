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

    public float attackRange;

    public UnitData unitData;
    
    [Header("현재 상태")]
    public int currentHealth;           // 현재 체력
    public bool isAlive = true;         // 생존 여부
    
    [Header("전투 상태")]
    public UnitBase currentTarget;      // 현재 공격 대상
    private float attackTimer = 0f;     // 공격 쿨타임
    
    [Header("소속")]
    public bool isPlayerUnit = true;

    private SpriteRenderer spriteRenderer;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;

    public void Initialize(UnitData data, bool isPlayer = true)
    {
        // UnitData에서 정보를 복사
        unitID = data.unitID;
        unitName = data.unitName;
        cost = data.cost;
        maxHealth = data.maxHealth;
        currentHealth = data.currentHealth;
        attack = data.attack;
        attackRate = data.attackRate;
        moveSpeed = data.moveSpeed;
        unitSprite = data.unitSprite;
        attackRange = data.attackRange;
        unitData = data;
        
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

    public void MoveTowards(Vector2 targetPosition)
    {
        if (!isAlive) return;
        
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
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

    public void Attack(UnitBase target)
    {
        if (!isAlive || target == null || !target.isAlive) return;
        
        // 쿨타임 체크
        if (attackTimer > 0) return;
        
        // 사거리 체크
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance > attackRange) return;
        
        // 공격 실행
        target.TakeDamage(attack);
        
        // 쿨타임 설정
        attackTimer = 1f / attackRate;
        
        currentTarget = target;
        
        Debug.Log($"{unitName}이(가) {target.unitName}을(를) 공격! (데미지: {attack})");
    }

    private void Die()
    {
        isAlive = false;
        currentTarget = null;
        
        Debug.Log($"{unitName} 사망!");
        
        // 투명도 조절 (시각적 표시)
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.3f;
            spriteRenderer.color = color;
        }
    }

    public void ResetState()
    {
        // 유닛 상태 초기화
        isAlive = true; // 생존 상태로 변경
        // currentHealth = maxHealth; // 필요하다면 체력도 초기화
        // currentTarget = null;
        
        // ⭐ 투명도 복원 ⭐
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            // Alpha 값을 1.0f (완전히 불투명)로 설정
            color.a = 1.0f; 
            spriteRenderer.color = color;
        }

        Debug.Log($"{unitName} 상태 복원 및 투명도 초기화 완료.");
    }
    
    /// <summary>
    /// 유닛 제거
    /// </summary>
    public void RemoveUnit()
    {
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 유닛 정보 문자열 반환 (디버깅용)
    /// </summary>
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
        if (!isAlive) return;
        
        // 공격 쿨타임 감소
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    

}
