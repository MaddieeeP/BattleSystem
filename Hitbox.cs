using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hitbox : PhysicActor, IDamageable
{
    [SerializeField] protected Alignment _alignment;
    [SerializeField] protected bool _activeHitbox = false;
    [Space]
    [SerializeField] protected bool _overrideInvincible = false;
    [SerializeField] protected float _invinciblePeriod;
    [SerializeField] protected int _maxHealth = 100;
    [Space]
    [SerializeField] protected float _mediumDamageProportion = 0.1f;
    [SerializeField] protected float _highDamageProportion = 0.15f;

    protected int _currentHealth;
    protected Timer _invincibleTimer = new Timer();

    //getters and setters
    public Alignment alignment { get { return _alignment; } }
    public int maxHealth { get { return _maxHealth; } }
    public int currentHealth { get { return _currentHealth; } set { SetHealth(value); } }
    public virtual bool invincible { get { return _overrideInvincible || _invincibleTimer.GetTime() < _invinciblePeriod; } }

    public virtual bool parrying { get { return false; } }

    public void SetAlignment(Alignment newAlignment)
    {
        _alignment = newAlignment;
    }

    public void SetHealth(int health)
    {
        _currentHealth = health;
    }

    public virtual void Activate()
    {
        _activeHitbox = true;
        OnActivate();
    }

    public virtual void Deactivate()
    {
        _activeHitbox = false;
        OnDeactivate();
    }

    public void Attack(Collider[] colliders, bool ignoreInvincibility = false, bool ignoreParry = false)
    {
        if (paused || !_activeHitbox)
        {
            return;
        }

        foreach (Collider collider in colliders)
        {
            IDamageable damageable = collider.transform.GetComponentInChildren<IDamageable>();
            if (damageable != null)
            {
                if (damageable.alignment == alignment)
                {
                    continue;
                }
                if (damageable.invincible && !ignoreInvincibility)
                {
                    OnBlocked(damageable, new DamageInfo(0, DamageInteraction.Block));
                    continue;
                }
                if (damageable.parrying && !ignoreParry)
                {
                    OnParried(damageable, new DamageInfo(0, DamageInteraction.Parry));
                    continue;
                }
                DamageInfo damageInfo = GiveDamage(damageable, ignoreInvincibility, ignoreParry);
                switch (damageInfo.interaction)
                {
                    case DamageInteraction.Land:
                        OnLand(damageable, damageInfo);
                        break;
                    case DamageInteraction.Block:
                        OnBlocked(damageable, damageInfo);
                        break;
                    case DamageInteraction.Absorb:
                        OnAbsorbed(damageable, damageInfo);
                        break;
                    case DamageInteraction.PassThrough:
                        break;
                }
            }
        }
    }

    public virtual DamageInfo GiveDamage(IDamageable damageable, bool ignoreInvincibility = false, bool ignoreParry = false)
    {
        return new DamageInfo(0, DamageInteraction.PassThrough);
    }

    public DamageInfo TakeDamage(int baseAmount, bool ignoreInvincibility = false, bool ignoreParry = false)
    {
        //FIX - add absorb

        int finalAmount = (int)Math.Min(baseAmount, _currentHealth);

        if (invincible && !ignoreInvincibility)
        {
            finalAmount = 0;
        }

        _currentHealth = _currentHealth - finalAmount;
        _invincibleTimer.Start();

        if (_currentHealth == 0)
        {
            OnDie();
            return new DamageInfo(finalAmount, DamageInteraction.Land);
        }

        float damageProportion = (float)baseAmount / _currentHealth;

        if (damageProportion == 0f)
        {
            OnBlock();
            return new DamageInfo(finalAmount, DamageInteraction.Block);
        }
        else if (damageProportion < _mediumDamageProportion)
        {
            OnLowDamage();
        }
        else if (damageProportion < _highDamageProportion)
        {
            OnMediumDamage();
        }
        else
        {
            OnHighDamage();
        }

        return new DamageInfo(finalAmount, DamageInteraction.Land);
    }

    protected virtual void OnDie()
    {
        Deactivate();
    }

    public virtual void OnBlocked(IDamageable damageable, DamageInfo damageInfo) 
    {
        Deactivate();
    }

    public virtual void OnParried(IDamageable damageable, DamageInfo damageInfo)
    {
        Deactivate();
    }

    public virtual void OnAbsorbed(IDamageable damageable, DamageInfo damageInfo)
    {
        Deactivate();
    }

    public virtual void OnLand(IDamageable damageable, DamageInfo damageInfo)
    {
        Deactivate();
    }

    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }

    public virtual void OnBlock() { }
    public virtual void OnParry() { }

    protected virtual void OnLowDamage() { }
    protected virtual void OnMediumDamage() { }
    protected virtual void OnHighDamage() { }
}