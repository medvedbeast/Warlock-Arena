using UnityEngine;
using System.Collections;
using System;

public class StartScreen : IMenu
{
    protected static readonly StartScreen _instance = new StartScreen();

    protected StartScreen() { }

    public static StartScreen Instance()
    {
        return _instance;
    }

    public override void Show()
    {
        float wPercent = Screen.width * 0.01f;
        float hPercent = Screen.height * 0.01f;
        float width = (10 * hPercent) * 7.05f;
        float height = 10 * hPercent;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), GameResources.BlackBG);

        GUIStyle label = new GUIStyle();
        label.fontSize = Convert.ToInt32(15 * hPercent);
        label.alignment = TextAnchor.MiddleCenter;
        label.normal.textColor = Color.white;

        GUIStyle button = new GUIStyle();
        button.alignment = TextAnchor.MiddleCenter;
        button.normal.textColor = Color.white;
        button.hover.background = GameResources.BlackBG;
        button.hover.textColor = Color.red;
        button.fontSize = Convert.ToInt32(5f * hPercent);

        GUI.Label(new Rect(0, 0, Screen.width, Screen.height),  "WARLOCK ARENA 2D", label);
        if (GUI.Button(new Rect((Screen.width - width) / 2, Screen.height / 2 + 100, width, height), "start", button))
        {
            Game.State = GameState.LOGIN_SCREEN;
            Game.CurrentMenu = LoginScreen.Instance();
        }
        if (GUI.Button(new Rect((Screen.width - width) / 2, Screen.height / 2 + 200, width, height), "exit", button))
        {
            Application.Quit();
        }
    }
}
