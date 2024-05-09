public static class Helper 
{
    public static float GetUpgradeFormula(int level)
    {
        return (level / 2 - 0.5f) * 0.5f;
    }
}
