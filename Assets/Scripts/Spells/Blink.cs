using UnityEngine;
using System.Collections;

public class Blink : ISpell
{

    protected static readonly Blink _instance = new Blink();

    protected Blink() { }

    public static Blink Instance()
    {
        return _instance;
    }

	void Start ()
    {
        
	}

    public override void SetValues()
    {
        Damage = 0;
        Casttime = 0;
        Knockback = true;
        KnockbackValue = 1000;
        Cooldown = 5;
        CooldownTimeElapsed = Cooldown;
    }

	void Update ()
    {
        
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (Knockback)
            {
                other.rigidbody2D.AddForce(-(this.transform.up * KnockbackValue));
            }
            GameObject.Destroy(this.transform.gameObject);
        }
    }

    public override void Act(CharacterController c)
    {
        Vector3 direction = c.transform.position - c.MousePosition;
        direction = new Vector3(direction.x, direction.y, 0);
        c.transform.rigidbody2D.AddForce(-direction * KnockbackValue);
    }
}
