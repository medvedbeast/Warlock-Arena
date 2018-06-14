using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Library;

public class Game : MonoBehaviour
{
    public int fieldwidth;
    public int fieldheight;
    public int groundwidth;
    public int groundheight;
    public int players;
    public static int FieldWidth;
    public static int FieldHeight;
    public static int GroundWidth;
    public static int GroundHeight;
    public static int Players;
    public static float SpriteScale = 0.205f;
    public static List<Point> StartPoints = new List<Point>();
    public static List<Rect> Rectangles = new List<Rect>();
    public static List<ISpell> SpellList = new List<ISpell>();
    public static List<CharacterController> PlayerList = new List<CharacterController>();
    public static GameState State;
    public static IMenu CurrentMenu;
    public static IContextMenu CurrentContextMenu;
    public static int MaximumPlayers;
    public static Size Field;
    public static Size Ground;

    public void StartGame(Message m)
    {
        Game.State = GameState.GAME;
        Game.CurrentMenu = InGame.Instance();
    }

    void Awake()
    {
        Client.GameStarted += StartGame;
        CurrentMenu = StartScreen.Instance();
        CurrentContextMenu = EmptyContextMenu.Instance();
        State = GameState.START_SCREEN;
        MaximumPlayers = StartPoints.Count;
        FieldWidth = fieldwidth;
        FieldHeight = fieldheight;
        GroundWidth = groundwidth;
        GroundHeight = groundheight;
        Players = players;
        Field.Width = FieldWidth;
        Field.Height = FieldHeight;
        Ground.Width = GroundWidth;
        Ground.Height = GroundHeight;
        StartPoints.Add(new Point(0 - (Ground.Width / 2), 0 - (Ground.Height / 2)));
        StartPoints.Add(new Point(0 - (Ground.Width / 2),  (Ground.Height / 2) - 1));
        StartPoints.Add(new Point(Ground.Width / 2, Ground.Height / 2));
        StartPoints.Add(new Point(Ground.Width / 2, 0 - ((Ground.Height / 2) - 1)));
        Rectangles.Add(new Rect(0 - Ground.Width / 2, 0 - Ground.Height / 2, Ground.Width, Ground.Height));
        Rectangles.Add(new Rect(0 - Field.Width / 2, 0 - Field.Height / 2, Field.Width, Field.Height));
        SpellList.Add(Fireball.Instance());
        SpellList.Add(Blink.Instance());
    }

	void Start ()
    {        
        FillStats();
        Game.CreateField();
        Game.SetCharacters();
	}
	
	void Update ()
    {
        if (PlayerList.Count == 1)
        {
            Debug.Log("GAME OVER");
        }
        if (Input.GetMouseButtonUp(1))
        {
            Game.CurrentContextMenu.showWindow = false;
            Game.CurrentContextMenu.position = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            Game.CurrentContextMenu.isShown = !Game.CurrentContextMenu.isShown;
        }
	}

    void OnGUI()
    {
        GUI.skin = GameResources.GUIskin;
        CurrentMenu.Show();
        if (CurrentContextMenu.isShown)
        {
            CurrentContextMenu.Show();
        }
    }

    public static void CreateField()
    {
        Point start = new Point(0 - (Field.Width / 2), 0 - (Field.Height / 2));
        Point current = start.Clone();
        GameObject lawa = GameObject.Find("Lawa");
        GameObject ground = GameObject.Find("Ground");
        for (int n = 0; n < Field.Height; n++)
        {
            current.X = start.X;
            for (int m = 0; m < Field.Width; m++)
            {
                GameObject sprite = GameObject.Instantiate(GameResources.LawaPrefab) as GameObject;
                sprite.transform.position = new Vector3(current.X, current.Y, 0);
                sprite.transform.parent = lawa.transform;
                sprite.name = "Lawa " + n + "x" + m;
                current.X++;
            }
            current.Y++;
        }
        start = new Point(0 - (Ground.Width / 2), 0 - (Ground.Height / 2));
        current = start.Clone();
        for (int n = 0; n < Ground.Height; n++)
        {
            current.X = start.X;
            for (int m = 0; m < Ground.Width; m++)
            {
                GameObject sprite = GameObject.Instantiate(GameResources.GroundPrefab) as GameObject;
                sprite.transform.position = new Vector3(current.X, current.Y, -1);
                sprite.transform.parent = ground.transform;
                sprite.name = "Ground " + n + "x" + m;
                current.X++;
            }
            current.Y++;
        }
    }

    public static void SetCharacters()
    {
        for (int i = 0; i < Game.Players; i++)
        {
            GameObject sprite = GameObject.Instantiate(GameResources.CharacterPrefab) as GameObject;
            sprite.transform.position = new Vector3(StartPoints[i].X, StartPoints[i].Y, -2);
            sprite.transform.localScale = new Vector3(Game.SpriteScale, Game.SpriteScale);
            sprite.transform.parent = sprite.transform;
            sprite.name = "Character " + (i + 1);
            if (i == 0)
            {
                CharacterController controller = sprite.GetComponent<CharacterController>() as CharacterController;
                controller.Controll();
                sprite.tag = "ControlledPlayer";
            }
        }
    }

    public void FillStats()
    {
        Fireball.Instance().SetValues();
        Blink.Instance().SetValues();
    }
}
