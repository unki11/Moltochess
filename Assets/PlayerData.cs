using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("플레이어 정보")]
    public int playerHealth = 100;      // 플레이어 체력
    public int playerGold = 100;         // 플레이어 골드
    public int playerLevel = 1;         // 플레이어 레벨
    
    [Header("골드 설정")]
    public int baseGoldPerRound = 5;    // 기본 라운드 보상 골드
    public int winGold = 1;             // 승리 보너스 골드
    public int roundGoldBonus = 3;      // 라운드 종료 보너스 골드

    public int shopRefreshCost = 2; //리롤 비용
    public int shopExpCost = 4; //경험치 비용
    public int level = 1; //레벨
    public int exp = 0; //경험치  
    
    private int[] expTable = {0, 2, 4, 8, 16, 24, 36, 50, 70, 100}; //경험치 테이블
    
    // 싱글톤 패턴
    public static PlayerData Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 골드 추가
    /// </summary>
    public void AddGold(int amount)
    {
        playerGold += amount;
        Debug.Log($"💰 골드 +{amount} (현재: {playerGold}G)");
    }
    
    /// <summary>
    /// 골드 차감
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            Debug.Log($"💰 골드 -{amount} (현재: {playerGold}G)");
            return true;
        }
        else
        {
            Debug.Log("❌ 골드 부족!");
            return false;
        }
    }
    
    /// <summary>
    /// 플레이어 체력 감소
    /// </summary>
    public void TakeDamage(int damage = 1)
    {
        playerHealth -= damage;
        Debug.Log($"❤️ 체력 -{damage} (현재: {playerHealth}HP)");
        
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("💀 게임 오버!");
        }
    }
    
    /// <summary>
    /// 라운드 시작
    /// </summary>
    public void StartRound()
    {
        Debug.Log($" ===== 라운드 시작 =====");
    }
    
    /// <summary>
    /// 라운드 종료 (정산)
    /// </summary>
    public void EndRound(bool isWin)
    {
        // 기본 보상
        int totalGold = baseGoldPerRound;
        
        // 승리 보너스
        if (isWin)
        {
            totalGold += winGold;
            Debug.Log("🎉 승리! +1G");
        }
        else
        {
            Debug.Log("💀 패배!");
        }
        
        // 라운드 보너스
        totalGold += roundGoldBonus;
        Debug.Log($" 라운드 완료 보너스: +{roundGoldBonus}G");
        
        // 골드 지급
        AddGold(totalGold);
        
        // 다음 라운드로
        Debug.Log($" ✅ 라운드 종료 - 다음 라운드");
    }
    
    /// <summary>
    /// 플레이어 정보 조회
    /// </summary>
    public string GetPlayerInfo()
    {
        return $"[라운드] ❤️ HP: {playerHealth} | 💰 Gold: {playerGold}G | 📊 Level: {playerLevel}";
    }
    
    /// <summary>
    /// 게임 오버 체크
    /// </summary>
    public bool IsGameOver()
    {
        return playerHealth <= 0;
    }

    public void AddExp(int amount = 4) //경험치 추가
    {
        exp += amount;
        SpendGold(amount);
        CheckLevelUp();
    }

    private void CheckLevelUp() //레벨 업 체크
    {
        while (level < expTable.Length && exp >= expTable[level])
        {
            exp -= expTable[level];
            level++;
            Debug.Log("레벨 업! 현재 레벨: " + level);
        }
    }
}