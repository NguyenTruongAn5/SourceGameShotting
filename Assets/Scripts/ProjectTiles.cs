using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectTiles : MonoBehaviour
{
    [Header("Base Setting: ")]
    [SerializeField] private float m_Speed;
    private float m_Damaged;
    private float m_CurrentSpeed;
    [SerializeField] private GameObject m_bodyHitPrefab;
    private Vector2 m_lastPostion;
    private RaycastHit2D m_RaycastHit;

    public float Damaged { get => m_Damaged; set => m_Damaged = value; }
    private void Start()
    {
        m_CurrentSpeed = m_Speed;
        RefreshLastPos();
    }
    private void Update()
    {
        transform.Translate(transform.right * m_CurrentSpeed * Time.deltaTime, Space.World);
        DealDamage();
        RefreshLastPos();
    }

    private void DealDamage()
    {
        Vector2 rayDirection = (Vector2)transform.position - m_lastPostion;
        m_RaycastHit = Physics2D.Raycast(m_lastPostion, rayDirection, rayDirection.magnitude);
        var collider = m_RaycastHit.collider;

        if(!m_RaycastHit || collider == null) { return; }
        if (collider.CompareTag(TagConsts.ENEMY_TAG))
        {
            DealDamageToEnemy(collider);
        }
    }

    private void DealDamageToEnemy(Collider2D collider)
    {
        Actor actorComp = collider.GetComponent<Actor>();
        actorComp?.TakeDamage(m_Damaged);
        if (m_bodyHitPrefab)
        {
            Instantiate(m_bodyHitPrefab, (Vector3)m_RaycastHit.point, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void RefreshLastPos()
    {
        m_lastPostion = transform.position;
    }
    private void OnDisable()
    {
        m_RaycastHit = new RaycastHit2D();
    }
}
