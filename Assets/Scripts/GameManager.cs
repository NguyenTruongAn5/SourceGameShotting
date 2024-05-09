using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    STARTING,
    PLAYING,
    PAUSED,
    GAMEOVER
}
public class GameManager : Singleton<GameManager>
{
    public static GameState gameState;
    [SerializeField] private Map m_mapPrefab;
    [SerializeField] private Player m_playerPrefab;
    [SerializeField] private Enemy[] m_enemyPrefabs;
    [SerializeField] private GameObject m_enemySpawnVfx;
    [SerializeField] private float m_enemySpawnTime;
    [SerializeField] private int m_playerMaxLife;
    [SerializeField] private int m_playerStartingLife;

    private Map m_map;
    private PlayerStarts m_playerStarts;
    private int m_currentLife;

    private Player m_player;

    public Player Player { get => m_player; private set => m_player = value; }
    public int CurrentLife
    {
        get => m_currentLife;
        set
        {
            m_currentLife = value;
            m_currentLife = Mathf.Clamp(m_currentLife, 0, m_playerMaxLife);
        }
    }

    protected override void Awake()
    {
        MakeSingleton(false);
    }
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        gameState = GameState.STARTING; // test
        m_currentLife = m_playerStartingLife;
        SpawnMap_Player();
        GUIManager.Ins.ShowGameGUI(false);

    }
    private void SpawnMap_Player()
    {
        if(m_mapPrefab == null || m_playerPrefab == null) return;
        m_map = Instantiate(m_mapPrefab, Vector3.zero, Quaternion.identity);
        m_player = Instantiate(m_playerPrefab, m_map.playerSpawnPoint.position, Quaternion.identity);
    }
    public void PlayGame()
    {
        gameState = GameState.PLAYING;
        m_playerStarts = m_player.PlayerStats;

        SpawnEnemy();

        if (m_playerStarts == null || m_player == null) return;
        
        GUIManager.Ins.ShowGameGUI(true);
        GUIManager.Ins.UpdateLifeInfo(m_currentLife);
        GUIManager.Ins.UpdateCoinCounting(Prefs.coins);
        GUIManager.Ins.UpdateHpInfo(m_player.CurrentHp, m_playerStarts.hp);
        GUIManager.Ins.UpdateLevelInfo(m_playerStarts.level, m_playerStarts.Xp, m_playerStarts.levelUpXpRequired);
    }
    private void SpawnEnemy()
    {
        var randomEnemy = GetRandomEnemy();
        if (randomEnemy == null || m_map == null) return;
        StartCoroutine(SpawnEnemy_Coroutine(randomEnemy));
    }

    private IEnumerator SpawnEnemy_Coroutine(Enemy randomEnemy)
    {
        yield return new WaitForSeconds(3f);
        while(gameState == GameState.PLAYING)
        {
            if (m_map.RandomAISpawnPoint == null) break;
            Vector3 spawnPoint = m_map.RandomAISpawnPoint.position;
            if(m_enemySpawnVfx)
                Instantiate(m_enemySpawnVfx, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
            Instantiate(randomEnemy, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(m_enemySpawnTime);
        }
        yield return null;
    }

    private Enemy GetRandomEnemy()
    {
       if(m_enemyPrefabs == null || m_enemyPrefabs.Length <= 0) return null;
       int randomIndex = UnityEngine.Random.Range(0, m_enemyPrefabs.Length);
        return m_enemyPrefabs[randomIndex];
    }
    public void GameOverChecking(Action OnLostLife = null, Action OnDead = null)
    {
        if(m_currentLife <= 0) return;
        
        m_currentLife--;
        OnLostLife?.Invoke();
        if(m_currentLife <= 0)
        {
            gameState = GameState.GAMEOVER;
            OnDead?.Invoke();
        }
    }
}
