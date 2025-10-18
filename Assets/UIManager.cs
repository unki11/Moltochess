using UnityEngine;
using UnityEngine.UI; 
using System.Collections; 
using TMPro; // ⭐ TextMeshPro를 사용하려면 이 네임스페이스가 필요합니다.

public class UIManager : MonoBehaviour
{
    // 싱글톤 패턴 생략
    public static UIManager Instance { get; private set; } 
    
    // 유니티 인스펙터에서 할당
    [Header("결과 UI")]
    public CanvasGroup resultCanvasGroup; 
    
    // ⭐ UnityEngine.UI.Text 대신 TMPro.TextMeshProUGUI 사용
    public TextMeshProUGUI resultText; // 결과 문구가 표시될 TextMeshPro 컴포넌트
    // ----------------------------------------------------------------------
    public TextMeshProUGUI gameRoundText;

    public TextMeshProUGUI roundText;
    public GameObject roundResultPanel;

    [Header("UI 컴포넌트 (TextMeshPro)")]
    public TextMeshProUGUI healthTextTMP;
    public TextMeshProUGUI goldTextTMP;
    public TextMeshProUGUI levelTextTMP;

    public TextMeshProUGUI expTextTMP;

    // 페이드 인/아웃 속도
    private float fadeDuration = 0.5f; 

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

        // C. (선택 사항) 씬이 바뀌어도 파괴되지 않게 하려면 추가합니다.
        // DontDestroyOnLoad(gameObject); 

        // 1. GameManager가 초기화될 때까지 기다립니다.
        // GameManager가 먼저 Awake에서 Instance를 설정했다고 가정합니다.
        
        // 2. GameManager의 이벤트에 UI 업데이트 함수들을 등록합니다.
        if (GameManager.Instance != null)
        {
            // OnRoundChange 이벤트 (int 매개변수) 구독
            GameManager.Instance.OnRoundChange.AddListener(UpdateRoundUI);
            
            
            // OnShoppingStart 이벤트 (매개변수 없음) 구독
            GameManager.Instance.OnShoppingStart.AddListener(OnShoppingStart);
            
            // OnBattleStart 이벤트 (매개변수 없음) 구독
            GameManager.Instance.OnBattleStart.AddListener(OnBattleStart);
        }
        else
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다. 이벤트 구독 실패!");
        }

        Debug.Log("UIManager 싱글톤 인스턴스 초기화 완료.");
    }

    // ⭐ 마우스 또는 키보드 입력을 감지하는 로직
    private void Update()
    {
        // 1. 결과 창이 완전히 표시되었고 (Alpha가 1에 가깝고), 
        // 2. 상호작용이 가능할 때만 (사용자가 창을 봤을 때) 입력을 감지합니다.
        if (resultCanvasGroup.alpha >= 0.99f && resultCanvasGroup.interactable)
        {
            // 마우스 버튼 (왼쪽, 오른쪽, 휠) 중 하나가 눌렸거나, 
            // 아무 키보드 키(AnyKey)가 눌렸을 때 감지합니다.
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.anyKeyDown)
            {
                // 다음 라운드 준비 로직 대신 창만 닫는 로직을 호출합니다.
                // 만약 버튼 클릭과 동일한 역할을 하게 하려면 OnNextRoundButtonClicked()를 호출하세요.
                HideRoundResult(); 
            }
        }

        UpdateUI();
    }
    
    void UpdateUI()
    {
        PlayerData player = PlayerData.Instance;
        if (player == null) return;
        
        GameManager game = GameManager.Instance;
        
        // TextMeshPro 업데이트
        if (healthTextTMP != null)
            healthTextTMP.text = $"HP: {player.playerHealth}";
        
        if (goldTextTMP != null)
            goldTextTMP.text = $"Gold: {player.playerGold}G";
        
        if (levelTextTMP != null)
            levelTextTMP.text = $"Level: {player.level}";

            if (expTextTMP != null)
            expTextTMP.text = $"exp: {player.exp}";
    }

    void Start()
    {
        
    }

    public void UpdateRoundUI(int round)
    {
        if (roundText != null && gameRoundText != null)
        {
            roundText.text = $"Round: {round}";
            Debug.Log($"UI 업데이트: 현재 라운드 {round}");

            string phaseName = GameManager.Instance.currentState.ToString();

            gameRoundText.text = phaseName;
            Debug.Log($"UI 업데이트: 게임 라운드 {phaseName}");
        }
        else
        {
            Debug.Log($"roundText와 gameRoundText가 없습니다.");
        }
    }
    
    // UnityEvent<float> (타이머)에 연결될 함수
    
    // UnityEvent (매개변수 없음)에 연결될 함수 (예시)
    public void OnShoppingStart()
    {
        Debug.Log("UI: 상점 페이즈 UI를 준비합니다.");
        // 상점 관련 UI 활성화/전투 UI 비활성화 등의 로직
    }

    // UnityEvent (매개변수 없음)에 연결될 함수 (예시)
    public void OnBattleStart()
    {
        Debug.Log("UI: 전투 페이즈 UI를 준비합니다.");
        // 전투 관련 UI 활성화/상점 UI 비활성화 등의 로직
    }
    
    // ⭐ 결과 창을 숨기는 새로운 함수
    public void HideRoundResult()
    {
        // 이미 투명도를 낮추는 코루틴이 실행 중이라면 중복 실행을 막습니다.
        if (resultCanvasGroup.alpha > 0f)
        {
            StartCoroutine(FadeResultPanel(0f, false)); // Alpha 0으로 페이드 아웃
            Debug.Log("결과 창이 입력에 의해 숨겨집니다.");
            
            // ⭐ (필요시) 여기서 다음 라운드 준비 로직을 호출합니다.
            // BattleManager.Instance.PrepareForNextRound(); 
        }
    }
    // 라운드 종료 시 호출되는 메인 함수
    public void ShowRoundResult(bool isWin)
    {
        Debug.Log("UI 호출 !!!!!!!!!");
        // 1. 결과 텍스트 설정
        if (isWin)
        {
            resultText.text = "Win!";
            // ⭐ TextMeshPro는 color 속성에 직접 접근합니다.
            resultText.color = Color.yellow; 
        }
        else
        {
            resultText.text = "Lose...";
            resultText.color = Color.red;
        }

        // 2. 결과 창을 서서히 나타나게 하는 코루틴 시작
        StartCoroutine(FadeResultPanel(1f, true)); 
    }

    /// <summary>
    /// Canvas Group의 투명도를 조절하는 코루틴 (페이드 효과)
    /// </summary>
    // ... (FadeResultPanel 코루틴은 동일하게 유지) ...
    private IEnumerator FadeResultPanel(float targetAlpha, bool interactable)
    {
        float startAlpha = resultCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            resultCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        resultCanvasGroup.alpha = targetAlpha;
        resultCanvasGroup.interactable = interactable;
        resultCanvasGroup.blocksRaycasts = interactable;
    }
    
    public void OnNextRoundButtonClicked()
    {
        StartCoroutine(FadeResultPanel(0f, false)); 
    }
}