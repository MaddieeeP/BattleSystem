using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alignment { Party, Enemy, None }

public interface IDamageable
{
    int maxHealth { get; }
    int currentHealth { get; set; }
    bool invincible { get; }
    bool parrying { get; }
    Alignment alignment { get; }

    DamageInfo TakeDamage(int baseAmount, bool ignoreInvincibility = false, bool ignoreParry = false);
}