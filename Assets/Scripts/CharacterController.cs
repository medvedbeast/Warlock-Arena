using UnityEngine;
using Library;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    public float Hp = 100;
    public float MaxHp = 100;
    public float Speed = 5;
    public Vector3 MousePosition;
    public bool IsControllable = false;
    
    bool IsCasting = false;
    float CurrentTime = 0;
    float CastingTime = 1;
    Vector2 Direction;
    
    ISpell CurrentSpell;

	void Start ()
    {
        Game.PlayerList.Add(this);
        Client.Moved += Move;
	}

    void OnGUI()
    {
        GUI.skin = GameResources.GUIskin;
        float width = 200;
        float height = 40;
        if (IsCasting)
        {
            float percent = CurrentTime / CastingTime * 100;
            GUI.DrawTexture(new Rect((Screen.width - width) / 2, (Screen.height - height - 150), width, height), GameResources.Castbar);
            GUI.DrawTexture(new Rect((Screen.width - width + 4) / 2, (Screen.height - height + 4 - 150), (width - 8) * percent / 100, height - 8), GameResources.Filler);
            float cooldown = (CastingTime - CurrentTime);
            if (cooldown <= 0)
                cooldown = 0;
            GUIStyle gs = new GUIStyle();
            gs.fontSize = 100;
            gs.alignment = TextAnchor.MiddleCenter;
            gs.normal.textColor = Color.white;
            GUI.Label(new Rect((Screen.width - width + 4) / 2 + width / 2, (Screen.height - height + 4 - 150) + 5, width - 8, height - 8), string.Format("{0:F2} / {1}", cooldown, CastingTime));
        }
        
    }

	void FixedUpdate ()
    {
        if (Game.State == GameState.GAME)
        {
            if (CurrentTime <= CastingTime)
            {
                CurrentTime += Time.deltaTime;
            }
            else
            {
                IsCasting = false;
                CurrentTime = 0;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 200);
            if (hit)
            {
                MousePosition = new Vector3(hit.point.x, hit.point.y);
            }
            if (!Game.Rectangles[0].Contains(transform.position))
            {
                Hp -= Time.deltaTime * 10;
            }
            for (int i = 0; i < Game.SpellList.Count; i++)
            {
                if (Game.SpellList[i].CooldownTimeElapsed < Game.SpellList[i].Cooldown)
                {
                    Game.SpellList[i].CooldownTimeElapsed += Time.deltaTime;
                }
            }
            if (Hp <= 0)
            {
                Game.PlayerList.Remove(this);
                GameObject.Destroy(this.transform.gameObject);
            }
            if (IsControllable)
            {
                float x = 0;
                float y = 0;
                bool change = false;
                if (Input.GetKey(KeyCode.A))
                {
                    x -= Speed;
                    change = true;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    x += Speed;
                    change = true;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    y += Speed;
                    change = true;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    y -= Speed;
                    change = true;
                }
                Direction = new Vector2(x, y);
                if (change)
                {
                    this.rigidbody2D.velocity = Direction;
                    Client.Send(Message.GetBytes(new Message(MESSAGE_TYPE.REQUEST_MOVE, Client.login, new object[] { Direction.x, Direction.y })));
                }
                else
                {
                    this.rigidbody2D.velocity = new Vector2(0, 0);
                }
                if (Input.GetKey(KeyCode.Z))
                {
                    CurrentSpell = Fireball.Instance();
                    if (CurrentSpell.CooldownTimeElapsed >= CurrentSpell.Cooldown)
                    {
                        Invoke("SpellCallback", CurrentSpell.Casttime);
                        IsControllable = false;
                        IsCasting = true;
                        CurrentTime = 0;
                        CastingTime = CurrentSpell.Casttime;
                        this.rigidbody2D.velocity = new Vector2(0, 0);
                    }
                    else
                    {
                        CurrentSpell = null;
                    }
                }
                if (Input.GetKey(KeyCode.X))
                {
                    CurrentSpell = Blink.Instance();
                    if (CurrentSpell.CooldownTimeElapsed >= CurrentSpell.Cooldown)
                    {
                        Invoke("SpellCallback", CurrentSpell.Casttime);
                        this.rigidbody2D.velocity = new Vector2(0, 0);
                    }
                    else
                    {
                        CurrentSpell = null;
                    }
                }
            }
        }
	}

    public void SpellCallback()
    {
        CurrentSpell.CooldownTimeElapsed = 0;
        CurrentSpell.Act(this);
    }

    public void Controll()
    {
        IsControllable = true;
    }

    public void Move(Message m)
    {
        for (int i = 0; i < Game.PlayerList.Count; i++)
		{
			 if (i == (int)m.parameters[0])
             {
                 this.rigidbody2D.velocity = new Vector2((float)m.parameters[1], (float)m.parameters[2]);
             }
		}
    }
}
