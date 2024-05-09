public class HealthCollectable : Collectable
{
    public override void Trigger()
    {
        if (m_player == null) return;

        m_player.CurrentHp += m_bonus;
        m_player.CurrentHp = UnityEngine.Mathf.Clamp(m_player.CurrentHp, 0, m_player.statData.hp);
        GUIManager.Ins.UpdateHpInfo(m_player.CurrentHp, m_player.PlayerStats.hp);
        AudioController.Ins.PlaySound(AudioController.Ins.healthPickup);
    }
}
