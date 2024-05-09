public class LifeCollectable : Collectable
{
    public override void Trigger()
    {
        GameManager.Ins.CurrentLife += m_bonus;
        GUIManager.Ins.UpdateLifeInfo(GameManager.Ins.CurrentLife);

        AudioController.Ins.PlaySound(AudioController.Ins.lifePickup);
    }
}
