using UnityEngine;
using System.Collections;

public class GameResources : MonoBehaviour
{

    public static GameObject GroundPrefab;
    public static GameObject LawaPrefab;
    public static GameObject CharacterPrefab;
    public static GameObject FireballPrefab;

    public static Texture2D BlackBG;
    public static Texture2D WhiteBG;
    public static Texture2D Castbar;
    public static Texture2D Filler;
    public static Texture2D Fireball;
    public static Texture2D Blink;
    public static Texture2D SpellUI;
    public static Texture2D Preview;
    public static Texture2D PowerBar;
    public static Texture2D PowerFiller;

    public static GUISkin GUIskin;


    void Awake()
    {
        GroundPrefab = Resources.Load("Models/GroundPrefab") as GameObject;
        LawaPrefab = Resources.Load("Models/LawaPrefab") as GameObject;
        CharacterPrefab = Resources.Load("Models/CharacterPrefab") as GameObject;
        FireballPrefab = Resources.Load("Models/Spells/FireballPrefab") as GameObject;

        BlackBG = Resources.Load("Textures/PixelBlack") as Texture2D;
        WhiteBG = Resources.Load("Textures/PixelWhite") as Texture2D;
        Castbar = Resources.Load("Textures/Castbar") as Texture2D;
        Filler = Resources.Load("Textures/Filler") as Texture2D;
        Fireball = Resources.Load("Textures/Fireball") as Texture2D;
        Blink = Resources.Load("Textures/Blink") as Texture2D;
        SpellUI = Resources.Load("Textures/SpellUI") as Texture2D;
        Preview = Resources.Load("Textures/CharacterPreview") as Texture2D;
        PowerBar = Resources.Load("Textures/Powerbar") as Texture2D;
        PowerFiller = Resources.Load("Textures/PowerbarFiller") as Texture2D;

        GUIskin = Resources.Load("GUISkin") as GUISkin;
    }
}
