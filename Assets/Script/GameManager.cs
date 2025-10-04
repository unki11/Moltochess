using System.Collections;
using UnityEngine;
using UnityEngine.Events;  // Unity 이벤트 시스템

public class GameManager : MonoBehaviour {
    // 싱글톤
    public static GameManager Instance;
    
    // 게임 상태
    public enum GameState { Shopping, Battle, GameOver }
    public GameState currentState;
    
    // 타이머 설정 (Inspector에서 조절 가능)
    [Header("Phase Durations")]
    public float shoppingDuration = 30f;
    public float battleDuration = 30f;
    
    // 현재 상태
    [Header("Game Info")]
    public int currentRound = 1;
    public float remainingTime = 0f;
    
    // 이벤트 시스템
    [Header("Events")]
    public UnityEvent OnShoppingStart;     // Unity 이벤트 - 상점 시작 시 발생
    public UnityEvent OnBattleStart;       // Unity 이벤트 - 전투 시작 시 발생
    public UnityEvent<float> OnTimerUpdate; // Unity 이벤트 - 타이머 업데이트 시 발생
    public UnityEvent<int> OnRoundChange;   // Unity 이벤트 - 라운드 변경 시 발생

    [Header("유닛 프리팹")]
    public GameObject unitPrefab;

    [Header("유닛 데이터")]
    public UnitData knightData;
    public UnitData armorData;
    
    // 싱글톤 패턴
    void Awake() {  // Unity 내부 함수 - 오브젝트 생성 시 가장 먼저 실행
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Unity 함수 - 씬 변경해도 유지
        } else {
            Destroy(gameObject);  // Unity 함수 - 중복 오브젝트 파괴
        }
    }
    
    void Start() {  // Unity 내부 함수 - 첫 프레임 시작 전 1번 실행
        StartCoroutine(GameLoop());  // Unity 함수 - 코루틴 시작
    }

    void Update()
    {
        // Space 키로 Knight 소환 테스트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnKnight(new Vector2(-2, 0), true);
        }
        
        // A 키로 Armor 소환 테스트
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnArmor(new Vector2(2, 0), false);
        }
        
        // K 키로 ID를 이용한 Knight 소환 테스트
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnUnitById("knight", new Vector2(-3, 0), true);
        }
        
        // R 키로 ID를 이용한 Armor 소환 테스트
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnUnitById("armor", new Vector2(3, 0), false);
        }
    }

    public UnitBase SpawnUnit(UnitData data, Vector2 position, bool isPlayer)
    {
        // 프리팹 생성
        GameObject unitObj = Instantiate(unitPrefab, position, Quaternion.identity);
        
        // UnitBase 컴포넌트 가져오기
        UnitBase unit = unitObj.GetComponent<UnitBase>();
        
        // 유닛 초기화
        if (unit != null)
        {
            unit.Initialize(data, isPlayer);
            Debug.Log($"유닛 생성: {unit.GetUnitInfo()}");
        }
        
        return unit;
    }
    
    /// <summary>
    /// Knight 유닛을 생성하는 편의 메서드
    /// </summary>
    public UnitBase SpawnKnight(Vector2 position, bool isPlayer = true)
    {
        if (knightData == null)
        {
            Debug.LogError("Knight 데이터가 설정되지 않았습니다!");
            return null;
        }
        return SpawnUnit(knightData, position, isPlayer);
    }
    
    /// <summary>
    /// Armor 유닛을 생성하는 편의 메서드
    /// </summary>
    public UnitBase SpawnArmor(Vector2 position, bool isPlayer = true)
    {
        if (armorData == null)
        {
            Debug.LogError("Armor 데이터가 설정되지 않았습니다!");
            return null;
        }
        return SpawnUnit(armorData, position, isPlayer);
    }

    public void SpawnKnightButton()
    {
        SpawnKnight(new Vector2(-2, 0),true);
    }

    public void SpawnArmorButton()
    {
        SpawnArmor(new Vector2(0, 2),true);
    }
    
    /// <summary>
    /// 유닛 ID로 유닛을 생성하는 메서드
    /// </summary>
    public UnitBase SpawnUnitById(string unitId, Vector2 position, bool isPlayer = true)
    {
        UnitData data = GetUnitDataById(unitId);
        if (data == null)
        {
            Debug.LogError($"유닛 ID '{unitId}'에 해당하는 데이터를 찾을 수 없습니다!");
            return null;
        }
        return SpawnUnit(data, position, isPlayer);
    }
    
    /// <summary>
    /// 유닛 ID로 UnitData를 가져오는 메서드
    /// </summary>
    private UnitData GetUnitDataById(string unitId)
    {
        switch (unitId.ToLower())
        {
            case "knight":
                return knightData;
            case "armor":
                return armorData;
            default:
                return null;
        }
    }
    
    // 메인 게임 루프 (코루틴)
    IEnumerator GameLoop() {  // 코루틴 - 비동기 실행, 중간에 대기 가능
        while (currentState != GameState.GameOver) {
            yield return StartShoppingPhase();  // 상점 페이즈 완료까지 대기
            yield return StartBattlePhase();    // 전투 페이즈 완료까지 대기
            currentRound++;
            OnRoundChange.Invoke(currentRound);
        }
    }
    
    // 상점 페이즈
    IEnumerator StartShoppingPhase() {
        currentState = GameState.Shopping;
        remainingTime = shoppingDuration;
        OnShoppingStart.Invoke();  // 다른 시스템들에게 알림

        Debug.Log("StartShoppingPhase");
        
        // 타이머 코루틴
        while (remainingTime > 0f) {
            yield return new WaitForSeconds(0.1f);  // Unity 코루틴 - 0.1초 대기
            remainingTime -= 0.1f;
            OnTimerUpdate.Invoke(remainingTime);  // UI 업데이트용 이벤트
        }
    }
    
    // 전투 페이즈  
    IEnumerator StartBattlePhase() {
        currentState = GameState.Battle;
        remainingTime = battleDuration;
        OnBattleStart.Invoke();  // 전투 시스템에게 알림
        
        Debug.Log("StartBattlePhase");

        // 전투 타이머
        while (remainingTime > 0f) {
            yield return new WaitForSeconds(0.1f);
            remainingTime -= 0.1f;
            OnTimerUpdate.Invoke(remainingTime);
            
            // 전투가 일찍 끝나면 페이즈 종료
            if (IsBattleFinished()) {
                break;
            }
        }
    }
    
    // 유틸리티 함수들
    bool IsBattleFinished() {
        // BattleManager에서 전투 완료 여부 확인
        return false; // 임시
    }
    
    public void EndGame() {
        currentState = GameState.GameOver;
        StopAllCoroutines();  // Unity 함수 - 모든 코루틴 정지
    }
}