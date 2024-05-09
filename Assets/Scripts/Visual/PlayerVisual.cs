using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : ActorVisual
{
    [SerializeField] private GameObject m_deathVfxPrefab;
    private Player m_player;
    private PlayerStarts m_playerStats;
    private void Start()
    {
        m_player = (Player) m_actor;
        m_playerStats = m_player.PlayerStats;
    }
    public override void OnTakeDamage()
    {
        base.OnTakeDamage();
        GUIManager.Ins.UpdateHpInfo(m_actor.CurrentHp, m_actor.statData.hp);
    }
    public void OnLostLife()
    {
        if (m_playerStats == null || m_player == null) return;
        AudioController.Ins.PlaySound(AudioController.Ins.lostLife);
        GUIManager.Ins.UpdateLifeInfo(GameManager.Ins.CurrentLife);
        GUIManager.Ins.UpdateHpInfo(m_player.CurrentHp, m_playerStats.hp);
    }
    public void OnDead()
    {
        if (m_deathVfxPrefab)
        {
            Instantiate(m_deathVfxPrefab, transform.position, Quaternion.identity);
        }
        AudioController.Ins.PlaySound(AudioController.Ins.playerDeath);
        GUIManager.Ins.ShowGameOverDialog();
    }
    public void OnAddXp()
    {
        if (m_playerStats == null) return;
        GUIManager.Ins.UpdateLevelInfo(m_playerStats.level, m_playerStats.Xp, m_playerStats.levelUpXpRequired);
    }
    public void OnLevelUp()
    {
        AudioController.Ins.PlaySound(AudioController.Ins.levelUp);
    }
}
