using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{

    public static BattleManager Instance { get; private set; }

    [Header("보드 설정")]
    public List<BoardSlot> allyCells = new List<BoardSlot>();   // 아군 진영 셀
    public List<BoardSlot> enemyCells = new List<BoardSlot>();  // 적군 진영 셀
    
    [Header("상태")]
    public bool isInBattle = false;
    public List<UnitBase> allyUnits = new List<UnitBase>();
    public List<UnitBase> enemyUnits = new List<UnitBase>();
    
    // 배틀 전 상태 저장
    private List<CellUnitState> savedAllyStates = new List<CellUnitState>();
    private List<CellUnitState> savedEnemyStates = new List<CellUnitState>();
    
    /// <summary>
    /// 셀의 유닛 상태 저장 클래스
    /// </summary>
    [System.Serializable]
    private class CellUnitState
    {
        public BoardSlot cell;
        public UnitBase originalUnit;      // 원본 유닛 참조
        public int currentHealth;         // 원본 체력
    }
    
    /// <summary>
    /// 전투 시작
    /// </summary>
    public void StartBattle()
    {
        if (isInBattle) return;
        
        isInBattle = true;
        allyUnits.Clear();
        enemyUnits.Clear();
        
        // 배틀 전 상태 저장
        SaveCellState();
        
        // 셀의 기물들을 전투용으로 활성화
        ActivateAllyUnits();
        ActivateEnemyUnits();
        
        Debug.Log($"⚔️ 전투 시작! 아군 {allyUnits.Count}명 vs 적군 {enemyUnits.Count}명");
    }
    
    /// <summary>
    /// 셀 상태 저장
    /// </summary>
    void SaveCellState()
    {
        savedAllyStates.Clear();
        savedEnemyStates.Clear();
        
        // 아군 셀 상태 저장
        foreach (var cell in allyCells)
        {
            if (cell.unitBase != null)
            {
                Debug.Log($"아군 확인 : {cell.unitBase}");
                UnitBase unit = cell.unitBase;
                if (unit != null)
                {
                    CellUnitState state = new CellUnitState
                    {
                        cell = cell,
                        originalUnit = unit,
                        currentHealth = unit.currentHealth
                    };
                    savedAllyStates.Add(state);
                }
            }
        }
        
        // 적군 셀 상태 저장
        foreach (var cell in enemyCells)
        {
            if (cell.unitBase != null)
            {
                UnitBase unit = cell.unitBase;
                if (unit != null)
                {
                    CellUnitState state = new CellUnitState
                    {
                        cell = cell,
                        originalUnit = unit,
                        currentHealth = unit.currentHealth
                    };
                    savedEnemyStates.Add(state);
                }
            }
        }
        
        Debug.Log($"💾 상태 저장: 아군 {savedAllyStates.Count}명, 적군 {savedEnemyStates.Count}명");
    }
    
    /// <summary>
    /// 아군 유닛 활성화
    /// </summary>
    void ActivateAllyUnits()
    {
        foreach (var cell in allyCells)
        {
            if (cell.unitBase != null)
            {
                UnitBase unit = cell.unitBase;
                if (unit != null)
                {
                    unit.isPlayerUnit = true;
                    allyUnits.Add(unit);
                    
                    Debug.Log($"🟢 아군 활성화: {unit.unitData.unitName}");
                }
            }
        }
    }
    
    /// <summary>
    /// 적군 유닛 활성화
    /// </summary>
    void ActivateEnemyUnits()
    {
        foreach (var cell in enemyCells)
        {
            if (cell.unitBase != null)
            {
                UnitBase unit = cell.unitBase;
                if (unit != null)
                {
                    unit.isPlayerUnit = false;
                    enemyUnits.Add(unit);
                    
                    Debug.Log($"🔴 적군 활성화: {unit.unitData.unitName}");
                }
            }
        }
    }
    
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartBattle();
        }

        if (!isInBattle) return;
        
        // 죽은 유닛 제거
        allyUnits.RemoveAll(u => u == null || !u.isAlive);
        enemyUnits.RemoveAll(u => u == null || !u.isAlive);
        
        // 전투 진행
        UpdateBattle();
        
        // 전투 종료 조건 확인
        CheckBattleEnd();
    }
    
    /// <summary>
    /// 전투 업데이트
    /// </summary>
    void UpdateBattle()
    {
        // 아군 유닛 이동/공격
        foreach (var unit in allyUnits)
        {
            if (unit.isAlive)
            {
                UnitBase target = FindNearestEnemy(unit, enemyUnits);
                
                if (target != null && target.isAlive)
                {
                    float distance = Vector2.Distance(unit.transform.position, target.transform.position);
                    if (distance <= unit.attackRange)
                    {
                        unit.Attack(target);
                    }
                    else
                    {
                        // 이동 후 경계 확인
                        Vector2 newPosition = (Vector2)unit.transform.position + 
                            (Vector2)((target.transform.position - unit.transform.position).normalized * unit.moveSpeed * Time.deltaTime);
                        
                        // 전체 보드 경계 내에서만 이동
                        newPosition = ClampPositionToBoard(newPosition);
                        unit.transform.position = newPosition;
                    }
                }
            }
        }
        
        // 적군 유닛 이동/공격
        foreach (var unit in enemyUnits)
        {
            if (unit.isAlive)
            {
                UnitBase target = FindNearestEnemy(unit, allyUnits);
                
                if (target != null && target.isAlive)
                {
                    float distance = Vector2.Distance(unit.transform.position, target.transform.position);
                    if (distance <= unit.attackRange)
                    {
                        unit.Attack(target);
                    }
                    else
                    {
                        // 이동 후 경계 확인
                        Vector2 newPosition = (Vector2)unit.transform.position + 
                            (Vector2)((target.transform.position - unit.transform.position).normalized * unit.moveSpeed * Time.deltaTime);
                        
                        // 전체 보드 경계 내에서만 이동
                        newPosition = ClampPositionToBoard(newPosition);
                        unit.transform.position = newPosition;
                    }
                }
            }
        }
    }

    Vector2 ClampPositionToBoard(Vector2 position)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        
        // 모든 셀(아군 + 적군)의 경계 계산
        foreach (var cell in allyCells)
        {
            Vector2 cellPos = cell.transform.position;
            
            minX = Mathf.Min(minX, cellPos.x - 0.5f);
            maxX = Mathf.Max(maxX, cellPos.x + 0.5f);
            minY = Mathf.Min(minY, cellPos.y - 0.5f);
            maxY = Mathf.Max(maxY, cellPos.y + 0.5f);
        }
        
        foreach (var cell in enemyCells)
        {
            Vector2 cellPos = cell.transform.position;
            
            minX = Mathf.Min(minX, cellPos.x - 0.5f);
            maxX = Mathf.Max(maxX, cellPos.x + 0.5f);
            minY = Mathf.Min(minY, cellPos.y - 0.5f);
            maxY = Mathf.Max(maxY, cellPos.y + 0.5f);
        }
        
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        
        return position;
    }
    
    /// <summary>
    /// 가장 가까운 적 찾기
    /// </summary>
    UnitBase FindNearestEnemy(UnitBase attacker, List<UnitBase> enemies)
    {
        UnitBase nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (var enemy in enemies)
        {
            if (enemy.isAlive)
            {
                float distance = Vector2.Distance(attacker.transform.position, enemy.transform.position);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// 전투 종료 조건 확인
    /// </summary>
    void CheckBattleEnd()
    {
        bool allyDefeated = allyUnits.Count == 0 || allyUnits.TrueForAll(u => !u.isAlive);
        bool enemyDefeated = enemyUnits.Count == 0 || enemyUnits.TrueForAll(u => !u.isAlive);
        
        if ((allyDefeated || enemyDefeated) && isInBattle)
        {
            StartCoroutine( EndBattle(3.0f,enemyDefeated));
           
        }
    }
    
    /// <summary>
    /// 전투 종료
    /// </summary>
    private IEnumerator EndBattle(float delay,bool allyWon)
    {
        isInBattle = false;

        yield return new WaitForSeconds(delay);
        
        if (allyWon)
        {
            Debug.Log("🎉 아군 승리!");
        }
        else
        {
            PlayerData.Instance.TakeDamage();
            Debug.Log("💀 아군 패배!");
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowRoundResult(allyWon);
        }
        
        // 셀 상태 복구
        RestoreCellState();
        
        // 유닛 목록 초기화
        allyUnits.Clear();
        enemyUnits.Clear();
    }
    
    /// <summary>
    /// 셀 상태 복구
    /// </summary>
    void RestoreCellState()
    {
        // 아군 상태 복구
        foreach (var state in savedAllyStates)
        {
            if (state.cell == null || state.originalUnit == null) continue;
            
            
            if (state.cell.unitBase != null && state.originalUnit != null)
            {
                UnitBase restoreUnit =  state.cell.unitBase;
                // 원본 유닛의 정보로 초기화
                restoreUnit.currentHealth = state.currentHealth;  // 원본 체력으로 복구
   
                restoreUnit.ResetState();
                
                // 셀에 배치
                restoreUnit.transform.localPosition = Vector3.zero;
                
                Debug.Log($"✅ 복구 (아군): {restoreUnit.unitName} - HP: {restoreUnit.currentHealth}/{restoreUnit.maxHealth}");
            }
        }
        
        // 적군 상태 복구
        foreach (var state in savedEnemyStates)
        {
            if (state.cell == null || state.originalUnit == null) continue;
            
           if (state.cell.unitBase != null && state.originalUnit != null)
            {
                // 원본 유닛의 정보로 초기화
                UnitBase restoreUnit = state.cell.unitBase;  

                restoreUnit.currentHealth = state.currentHealth;   // 원본 체력으로 복구
   
                restoreUnit.ResetState();
        
                // 셀에 배치
                restoreUnit.transform.localPosition = Vector3.zero;
                
                Debug.Log($"✅ 복구 (적군): {restoreUnit.unitName} - HP: {restoreUnit.currentHealth}/{restoreUnit.maxHealth}");
            }
        }
    }

    private void Awake()
    {
        // A. 이미 인스턴스가 존재하고, 그 인스턴스가 자신이 아니라면 (중복 생성 시도)
        if (Instance != null && Instance != this)
        {
            // 중복된 객체(자신)를 파괴하여 단일 인스턴스 보장
            Destroy(gameObject);
            return; // 이후 코드는 실행하지 않음
        }

        // B. 인스턴스가 아직 없다면, 현재 객체를 싱글톤 인스턴스로 지정
        Instance = this;

        // C. (선택 사항) 씬이 바뀌어도 파괴되지 않게 하려면 이 코드를 추가합니다.
        // DontDestroyOnLoad(gameObject); 

        Debug.Log("BattleManager 싱글톤 인스턴스 초기화 완료.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}