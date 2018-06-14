using UnityEngine;
using System.Collections;
using Library;
using System;

public class LoginScreen : IMenu
{
    protected static readonly LoginScreen _instance = new LoginScreen();

    protected LoginScreen() { }

    public static LoginScreen Instance()
    {
        return _instance;
    }

    public string login = "";
    public static bool failedLogin = false;
    public static string helpText = "";

    public override void Show()
    {

        float wPercent = Screen.width * 0.01f;
        float hPercent = Screen.height * 0.01f;
        float width = (10 * hPercent) * 7.05f;
        float height = 10 * hPercent;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), GameResources.BlackBG);

        GUIStyle textField = new GUIStyle();
        textField.alignment = TextAnchor.MiddleCenter;
        textField.normal.textColor = Color.white;
        textField.normal.background = GameResources.BlackBG;
        textField.fontSize = Convert.ToInt32(4 * hPercent);

        GUIStyle border = new GUIStyle();
        border.normal.background = GameResources.WhiteBG;

        GUIStyle button = new GUIStyle();
        button.alignment = TextAnchor.MiddleCenter;
        button.normal.textColor = Color.white;
        button.hover.textColor = Color.red;
        button.hover.background = GameResources.BlackBG;
        button.fontSize = Convert.ToInt32(6 * hPercent);

        GUIStyle label = new GUIStyle();
        label.fontSize = Convert.ToInt32(10 * hPercent);
        label.alignment = TextAnchor.MiddleCenter;
        label.normal.textColor = Color.white;

        GUIStyle label2 = new GUIStyle();
        label2.fontSize = Convert.ToInt32(4 * hPercent);
        label2.alignment = TextAnchor.MiddleCenter;
        label2.normal.textColor = Color.red;

        GUI.Label(new Rect((Screen.width - 300) / 2, (Screen.height - 50) / 2 - 125, 300, 50), "enter your nickname here:", label);
        if (failedLogin)
        {
            GUI.Label(new Rect((Screen.width - 300) / 2, (Screen.height - 50) / 2 - 75, 300, 50), helpText, label2);
        }
        GUI.Box(new Rect((Screen.width - 500) / 2 - 4, (Screen.height - 50) / 2 - 4, 508, 58), "", border);
        login = GUI.TextArea(new Rect((Screen.width - 500) / 2, (Screen.height - 50) / 2, 500, 50), login, 32, textField);
        if (GUI.Button(new Rect((Screen.width - 300) / 2, (Screen.height - 50) / 2 + 150, 300, 50), "login", button))
        {
            Client.Start(login);
        }
    }

    public static void ConfirmedLogin(bool b)
    {
        if (b)
        {
            Game.State = GameState.ROOM_FINDING_SCREEN;
            Game.CurrentMenu = RoomFindingScreen.Instance();
            Game.CurrentContextMenu = ChannelMenu.Instance();
        }
        else
        {
            LoginScreen.failedLogin = true;
            helpText = "(try another nick)";
            Client.ReinitializeSocket();
        }
    }
}
