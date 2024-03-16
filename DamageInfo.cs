using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageInteraction { PassThrough, Land, Block, Parry, Absorb }

public class DamageInfo
{
    private int _amount;
    private DamageInteraction _interaction;

    public int amount { get { return _amount; } }
    public DamageInteraction interaction { get { return _interaction; } }

    public DamageInfo(int damageAmount, DamageInteraction damageInteraction)
    {
        _amount = damageAmount;
        _interaction = damageInteraction;
    }
}
