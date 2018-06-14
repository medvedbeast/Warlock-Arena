using UnityEngine;
using System.Collections;

public class Fireball : ISpell
{
    protected static readonly Fireball _instance = new Fireball();

    protected Fireball() { }

    public static Fireball Instance()
    {
        return _instance;
    }

	void Start ()
    {
        
	}

    public override void SetValues()
    {
        Damage = 10;
        Casttime = 1;
        Knockback = true;
        KnockbackValue = 20;
        Cooldown = 2;
        CooldownTimeElapsed = Cooldown;
    }

	void Update ()
    {
        if (!Game.Rectangles[1].Contains(transform.position))
        {
            GameObject.Destroy(this.transform.gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (Knockback)
            {
                other.GetComponent<CharacterController>().Hp -= Damage;
                other.rigidbody2D.AddForce(-(this.transform.up * KnockbackValue));
            }
            GameObject.Destroy(this.transform.gameObject);
        }
    }

    public override void Act(CharacterController c)
    {
        GameObject spell = GameObject.Instantiate(GameResources.FireballPrefab) as GameObject;
        spell.transform.position = new Vector3(c.transform.position.x, c.transform.position.y, -2);
        spell.transform.localScale = new Vector3(Game.SpriteScale, Game.SpriteScale);
        Vector3 direction = spell.transform.position - c.MousePosition;
        direction = new Vector3(direction.x, direction.y, 0);
        spell.transform.up = direction;
        spell.transform.rigidbody2D.velocity = -(direction.normalized * (c.Speed * 2));
        c.IsControllable = true;
    }
}
