using System;
using UnityEngine;

public class Enemy : Actor
{
    private Player m_player;
    private EnemyStarts m_enemyStats;
    private float m_curDamage;
    private float m_XpBonus;

    public float CurDamage { get => m_curDamage; private set => m_curDamage = value; }
    public override void Init()
    {
        m_player = GameManager.Ins.Player;
        if (statData == null || m_player == null) return;
        m_enemyStats = (EnemyStarts)statData;
        m_enemyStats.Load();

        StatsCaculate();
        OnDead.AddListener(OnSpawnCollectable);
        OnDead.AddListener(OnAddXpToPlayer);
    }
    private void FixedUpdate()
    {
        Move();
    }
    protected override void Move()
    {
        if (IsDead || m_player == null) return;
        Vector2 playeDir = m_player.transform.position - transform.position;
        playeDir.Normalize();
        if (!m_IsnockBack)
        {
            Flip(playeDir);
            m_rb.velocity = playeDir * m_enemyStats.moveSpeed * Time.deltaTime;
            return;
        }
        m_rb.velocity = playeDir * -m_enemyStats.knockbackForce * Time.deltaTime;
    }

    private void Flip(Vector2 playeDir)
    {
        if(playeDir.x > 0)
        {
            if (transform.localScale.x > 0) return;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else if(playeDir.x < 0)
        {
            if(transform.localScale.x< 0) return;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    private void StatsCaculate()
    {
        var playerStats = m_player.PlayerStats;
        if (playerStats == null) return;
        float hpUpgrade = m_enemyStats.hpUp * Helper.GetUpgradeFormula(playerStats.level + 1);
        float damageUpgrade = m_enemyStats.damageUp * Helper.GetUpgradeFormula(playerStats.level + 1);
        float randomXpBonus = UnityEngine.Random.Range(m_enemyStats.minXpBonus, m_enemyStats.maxXpBonus);

        CurrentHp = m_enemyStats.hp + hpUpgrade;
        CurDamage = m_enemyStats.damage + damageUpgrade;
        m_XpBonus = randomXpBonus * Helper.GetUpgradeFormula(playerStats.level + 1);

    }
    protected override void Die()
    {
        base.Die();
        m_anim.SetTrigger(AnimiConsts.ENEMY_DEAD);
    }
    private void OnSpawnCollectable()
    {
        CollectableManager.Ins.Spawn(transform.position);
    }

    private void OnAddXpToPlayer()
    {
        GameManager.Ins.Player.AddXp(m_XpBonus);
    }
    private void OnDisable()
    {
        OnDead.RemoveListener(OnSpawnCollectable);
        OnDead.RemoveListener(OnAddXpToPlayer);
    }
}
