using UnityEngine;
using System.Collections;

public class ISpell : MonoBehaviour
{
    public float Damage;
    public float Casttime;
    public bool Knockback;
    public float KnockbackValue;
    public float Cooldown;
    public float CooldownTimeElapsed;

    public virtual void SetValues() { }

    public virtual void Act(CharacterController c) { }
}
