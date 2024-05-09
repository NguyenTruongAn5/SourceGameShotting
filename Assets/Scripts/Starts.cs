using System;
using UnityEngine;

public abstract class Starts : ScriptableObject
{
    public abstract void Save();
    public abstract void Load();
    public abstract void Upgrade(Action OnSucess = null, Action OnFailed = null);
    public abstract bool IsMaxLevel();
}
