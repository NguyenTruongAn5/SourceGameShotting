using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStarts : Starts
{
    [Header("Base Start: ")]
    public float hp;
    public float damage;
    public float moveSpeed;
    public float knockbackForce;
    public float knockBackTime;
    public float invincibleTime;
    public override bool IsMaxLevel()
    {
        return false;
    }

    public override void Load()
    {
        
    }

    public override void Save()
    {
        
    }

    public override void Upgrade(Action OnSucess = null, Action OnFailed = null)
    {
        
    }
}
