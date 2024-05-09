using UnityEngine;

public class WeaponVisual : MonoBehaviour
{
    [SerializeField] private AudioClip m_shottingSound;
    [SerializeField] private AudioClip m_reloadSound;

    private Weapon m_weapon;
    public void OnShoot()
    {
        AudioController.Ins.PlaySound(m_shottingSound);
        CineController.Ins.ShakeTrigger();
    }
    public void OnReload()
    {
        GUIManager.Ins.ShowReloadText(true);
    }
    public void OnReloadDone()
    {
        AudioController.Ins.PlaySound(m_reloadSound);
        GUIManager.Ins.ShowReloadText(false);
    }
}
