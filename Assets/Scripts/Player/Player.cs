using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : Actor
{
    [Header("Player Setting: ")]
    [SerializeField] private float m_accelerationSpeed;
    [SerializeField] private float m_maxMousePosDistance;
    [SerializeField] private Vector2 m_velocityLimit;
    [SerializeField] private float m_enemyDectionRadius;
    [SerializeField] private LayerMask m_enemyDectionLayer;

    private float m_curSpeed;
    private Actor m_enemyTagreted;
    private PlayerStarts m_playerStats;
    private Vector2 m_enemyTagetedDir;

    [Header("Player Event: ")]
    public UnityEvent OnAddXp;
    public UnityEvent OnLevelUp;
    public UnityEvent OnLostLife;

    public PlayerStarts PlayerStats { get => m_playerStats; private set => m_playerStats = value; }

    public override void Init()
    {
        LoadStats();
    }

    private void LoadStats()
    {
        if (statData == null) return;
        m_playerStats = (PlayerStarts)statData;
        m_playerStats.Load();
        CurrentHp = m_playerStats.hp;
    }
    private void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {
        DetectEnemy();
    }

    private void DetectEnemy()
    {
        var enemyFindeds = Physics2D.OverlapCircleAll(transform.position, m_enemyDectionRadius, m_enemyDectionLayer);
        var finalEnemy = FindNearesEnemy(enemyFindeds);

        if (finalEnemy == null) { return; }
        m_enemyTagreted = finalEnemy;
        WeaponHandle();
    }

    private void WeaponHandle()
    {
        if (m_enemyTagreted == null || weapon == null) return;
        m_enemyTagetedDir = m_enemyTagreted.transform.position - weapon.transform.position;
        m_enemyTagetedDir.Normalize();
        float angle = Mathf.Atan2(m_enemyTagetedDir.y, m_enemyTagetedDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if(m_IsnockBack) return;
        weapon.Shoot(m_enemyTagetedDir);
    }

    private Actor FindNearesEnemy(Collider2D[] enemyFindeds)
    {
        float minDistance = 0;
        Actor finalEnemy = null;

        if (enemyFindeds == null || enemyFindeds.Length <= 0) return null;
        for (int i = 0; i < enemyFindeds.Length; i++)
        {
            var enemyFinded = enemyFindeds[i];
            if (enemyFinded == null) continue;
            if (finalEnemy == null)
            {
                minDistance = Vector2.Distance(transform.position, enemyFinded.transform.position);
            }
            else
            {
                float distanceTemp = Vector2.Distance(transform.position, enemyFinded.transform.position);
                if (distanceTemp > minDistance) continue;
                minDistance = distanceTemp;
            }
            finalEnemy = enemyFinded.GetComponent<Actor>();
        }
        return finalEnemy;
    }

    protected override void Move()
    {
        if (IsDead) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 movingDir = mousePos - (Vector2)transform.position;
        movingDir.Normalize();
        if (!m_IsnockBack)
        {
            if (Input.GetMouseButton(0))
            {
                Run(mousePos, movingDir);
            }
            else
            {
                BackToIdle();
            }
            return;
        }
        m_rb.velocity = m_enemyTagetedDir * -statData.knockbackForce * Time.deltaTime;
        m_anim.SetBool(AnimiConsts.PLAYER_RUN_PARA, false);
    }

    private void BackToIdle()
    {
        m_curSpeed -= m_accelerationSpeed * Time.deltaTime;
        m_curSpeed = Mathf.Clamp(m_curSpeed, 0f, m_curSpeed);
        m_rb.velocity = Vector2.zero;
        m_anim.SetBool(AnimiConsts.PLAYER_RUN_PARA, false);
    }

    private void Run(Vector2 mousePos, Vector2 movingDir)
    {
        m_curSpeed += m_accelerationSpeed * Time.deltaTime;
        m_curSpeed = Mathf.Clamp(m_curSpeed, 0, m_playerStats.moveSpeed);
        float delta = m_curSpeed * Time.deltaTime;
        float distanceToMousePos = Vector2.Distance(transform.position, mousePos);
        distanceToMousePos = Mathf.Clamp(distanceToMousePos, 0f, m_maxMousePosDistance / 3);
        delta *= distanceToMousePos;

        m_rb.velocity = movingDir * delta;
        float velocityLimitX = Mathf.Clamp(m_rb.velocity.x, -m_velocityLimit.x, m_velocityLimit.x);
        float velocityLimitY = Mathf.Clamp(m_rb.velocity.y, -m_velocityLimit.y, m_velocityLimit.y);
        m_rb.velocity = new Vector2(velocityLimitX, velocityLimitY);
        m_anim.SetBool(AnimiConsts.PLAYER_RUN_PARA, true);
    }
    public void AddXp(float xpBonus)
    {
        if (m_playerStats == null) return;
        m_playerStats.Xp += xpBonus;
        m_playerStats.Upgrade(OnUpgradeStats);
        OnAddXp?.Invoke();
        m_playerStats.Save();
    }
    private void OnUpgradeStats()
    {
        OnLevelUp?.Invoke();
    }
    public override void TakeDamage(float damage)
    {
        if(damage <= 0 || m_IsInvincible) return;
        CurrentHp -= damage;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, m_playerStats.hp);
        KnockBack();
        OnTakeDamage?.Invoke();
        if (CurrentHp > 0) return;

        GameManager.Ins.GameOverChecking(OnLostDelegate, OnDeadDelegate);
    }
    private void OnLostDelegate()
    {
        CurrentHp = m_playerStats.hp;
        if(m_stopKnockBackCo != null)
        {
            StopCoroutine(m_stopKnockBackCo);
        }
        if(m_InvincibleCo!= null)
        {
            StopCoroutine(m_InvincibleCo);
        }
        Invincible(3.5f);
        OnLostLife?.Invoke();
    }
    private void OnDeadDelegate()
    {
        CurrentHp = 0;
        Die();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagConsts.ENEMY_TAG))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy!= null)
            {
                TakeDamage(enemy.CurDamage);
            }
        }else if (collision.gameObject.CompareTag(TagConsts.COLLECTABLE_TAG))
        {
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            collectable?.Trigger();

            Destroy(collectable.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(133, 250, 47, 50);
        Gizmos.DrawSphere(transform.position, m_enemyDectionRadius);
    }
}
