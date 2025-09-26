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