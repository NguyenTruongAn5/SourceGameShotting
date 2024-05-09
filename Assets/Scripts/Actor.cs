using System;
using System.Collections;
using System.Collections.Generic;
using UDEV;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    [Header("Common: ")]
    public ActorStarts statData;

    [LayerList]
    [SerializeField] private int m_invicinbleLayer;
    [LayerList]
    [SerializeField] private int m_normalPlayer;

    public Weapon weapon;

    protected bool m_IsnockBack;
    protected bool m_IsInvincible;
    private bool m_IsDead;
    private float m_currentHp;

    protected Rigidbody2D m_rb;
    protected Animator m_anim;

    [Header("Event: ")]
    public UnityEvent OnInit;
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDead;

    protected Coroutine m_stopKnockBackCo;
    protected Coroutine m_InvincibleCo;
    public bool IsDead { get => m_IsDead; set => m_IsDead = value; }
    public float CurrentHp
    {
        get => m_currentHp;
        set
        {
            m_currentHp = value;
        }
    }
    protected virtual void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponentInChildren<Animator>();
    }
    protected virtual void Start()
    {
        Init();
        OnInit?.Invoke();
    }
    public virtual void Init()
    {

    }
    public virtual void TakeDamage(float damage)
    {
        if (damage < 0 || m_IsInvincible) { return; }
        m_currentHp -= damage;
        KnockBack();
        if (m_currentHp <= 0)
        {
            m_currentHp = 0;
            Die();
        }
        OnTakeDamage?.Invoke();
    }

    protected virtual void Die()
    {
        m_IsDead = true;
        m_rb.velocity = Vector3.zero;
        OnDead?.Invoke();
        Destroy(gameObject, 0.5f);
    }

    protected void KnockBack()
    {
        if (m_IsDead || m_IsnockBack || m_IsInvincible) return;
        m_IsnockBack = true;
        m_stopKnockBackCo = StartCoroutine(StopKnockBack());
    }
    protected void Invincible(float invincibleTime)
    {
        m_IsnockBack = false;
        m_IsInvincible = true;
        gameObject.layer = m_invicinbleLayer;

        m_InvincibleCo = StartCoroutine(StopInvincible(invincibleTime));
    }
    private IEnumerator StopKnockBack()
    {
        yield return new WaitForSeconds(statData.knockBackTime);
        Invincible(statData.invincibleTime);
    }
    private IEnumerator StopInvincible(float invincibleTime)
    {
        yield return new WaitForSeconds(invincibleTime);
        m_IsInvincible = false;
        gameObject.layer = m_normalPlayer;
    }
    protected virtual void Move()
    {

    }
}
