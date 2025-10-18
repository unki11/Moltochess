using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    // 기본 리소스
    public int health = 10;
    public int gold = 0;
    public int round = 1;
    public int winStreak = 0;
    public int loseStreak = 0;

    // 유닛 관리
    public List<UnitData> benchUnits = new List<UnitData>(); //유닛으로 변경 예정
    public List<string> boardUnits = new List<string>(); //유닛으로 변경 예정
    public int maxBoardSize = 8; //최대 보드 크기
    public int maxBenchSize = 9; //최대 벤치 크기

    // 상점 관련
    public List<string> shopUnits = new List<string>(); //유닛으로 변경 예정
    public int shopRefreshCost = 2; //리롤 비용
    public int shopExpCost = 4; //경험치 비용
    public int level = 1; //레벨
    public int exp = 0; //경험치    

    // 진행 상태
    public bool isAlive = true;
    public bool isReady = false;
    public int damageTakenThisRound = 0;

    private int[] expTable = {0, 2, 4, 8, 16, 24, 36, 50, 70, 100}; //경험치 테이블

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("플레이어 초기화 완료");
        Debug.Log("체력: " + health + ", 골드: " + gold);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) //데미지 입기
    {
        health -= damage;
        if (health <= 0)
        {
            isAlive = false;
            Debug.Log("플레이어 탈락!");
        }
    }

    public void AddExp(int amount) //경험치 추가
    {
        exp += amount;
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
