using UnityEngine;
using System.Collections;

public class InGame : IMenu
{
    protected static readonly InGame _instance = new InGame();
    
    protected InGame() { }

    public static InGame Instance()
    {
        return _instance;
    }

    float width;
    float height;
    float offset;
    float textOffset;

    delegate void Act();

    public override void Show()
    {
        GUIStyle spellButton = new GUIStyle();
        spellButton.fontSize = 17;
        spellButton.alignment = TextAnchor.UpperCenter;
        spellButton.normal.textColor = Color.black;

        width = 75;
        height = 75;
        offset = 15;
        textOffset = 12.5f;

        ShowButton(new Rect(offset, Screen.height - height, width, height), GameResources.Fireball, Fireball.Instance().Cooldown, Fireball.Instance().CooldownTimeElapsed, null, spellButton);
        ShowButton(new Rect(offset * 2 + width, Screen.height - height, width, height), GameResources.Blink, Blink.Instance().Cooldown, Blink.Instance().CooldownTimeElapsed, null, spellButton);
        ShowPreview("Character 1");
    }

    

    void Initialize()
    {
        
    }

    void ShowButton(Rect position, Texture2D texture, float cd, float cdTimeElapsed, Act action, GUIStyle style)
    {
        Initialize();
        GUI.Label(new Rect(position.xMin, position.yMin - offset, width, height), GameResources.SpellUI);
        if (GUI.Button(new Rect(position.xMin, position.yMin - offset, width, height), texture))
        {
            if (action != null)
                action.Invoke();
        }
        float cooldown = (cd - cdTimeElapsed);
        if (cooldown <= 0)
            cooldown = 0;
        GUI.Label(new Rect(position.xMin, position.yMin - textOffset, width, height), string.Format("{0, 0:F2} c", cooldown), style);
    }

    void ShowPreview(string name)
    {
        float w = 128;
        float h = 128;
        float hO = 3;
        float wO = 2.5f;
        GUI.DrawTexture(new Rect(offset, offset, w, h), GameResources.Preview);
        GUI.DrawTexture(new Rect(offset + w, offset, w * 2, h), GameResources.PowerBar);
        CharacterController character = GameObject.Find(name).GetComponent<CharacterController>();
        if (character != null)
        {
            float percent = 100 * (character.Hp / character.MaxHp);
            GUI.DrawTexture(new Rect(offset + w + wO, offset + hO, (w * 2 - wO * 2) * percent / 100, h - (hO * 2)), GameResources.PowerFiller);
        }
        //GUI.Label(new Rect(offset + width * 2, offset, width * 2, height * 2), powerFiller);
        //GUI.Label(new Rect(offset + width * 2, offset, width * 2, height * 2), powerFiller);
    }
}
