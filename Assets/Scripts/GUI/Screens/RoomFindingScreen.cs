using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Library;
using System.IO;

public class RoomFindingScreen : IMenu
{
    protected static readonly RoomFindingScreen _instance = new RoomFindingScreen();

    protected RoomFindingScreen() {  }

    public static RoomFindingScreen Instance()
    {
        return _instance;
    }

    public static List<string> players = new List<string>();
    public static List<ChatChannel> chatChannels = new List<ChatChannel>();

    public string selectedPlayer;
    public int selectedChatChannel = 0;
    public string message = "";

    bool isExpandedChatChannelSelector = false;
    float borderThickness = 4;

    public override void Show()
    {
        try
        {
            float wPercent = Screen.width * 0.01f;
            float hPercent = Screen.height * 0.01f;
            float width = Screen.width;
            float height = Screen.height;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), GameResources.BlackBG);

            GUIStyle border = new GUIStyle();
            border.normal.background = GameResources.WhiteBG;

            GUIStyle button = new GUIStyle();
            button.normal.textColor = Color.white;
            button.alignment = TextAnchor.MiddleCenter;
            button.fontSize = Convert.ToInt32(3.5f * hPercent);
            button.normal.background = GameResources.BlackBG;
            button.hover.background = GameResources.BlackBG;
            button.hover.textColor = Color.red;

            GUIStyle box = new GUIStyle();
            box.normal.background = GameResources.BlackBG;
            box.normal.textColor = Color.white;

            GUIStyle textBox = new GUIStyle();
            textBox.normal.background = GameResources.BlackBG;
            textBox.normal.textColor = Color.white;
            textBox.alignment = TextAnchor.MiddleLeft;

            GUIStyle name = new GUIStyle();
            name.normal.textColor = Color.white;
            name.alignment = TextAnchor.MiddleLeft;
            name.fontSize = Convert.ToInt32(2.5f * hPercent);
            name.hover.background = GameResources.BlackBG;
            name.hover.textColor = Color.red;

            GUIStyle nameSelected = new GUIStyle();
            nameSelected.normal.textColor = Color.blue;
            nameSelected.alignment = TextAnchor.MiddleLeft;
            nameSelected.fontSize = Convert.ToInt32(2.5f * hPercent);
            nameSelected.hover.background = GameResources.BlackBG;
            nameSelected.hover.textColor = Color.blue;

            GUIStyle comboButton = new GUIStyle();
            comboButton.normal.textColor = Color.white;
            comboButton.alignment = TextAnchor.MiddleCenter;
            comboButton.normal.background = GameResources.BlackBG;
            comboButton.hover.textColor = Color.red;
            comboButton.hover.background = GameResources.BlackBG;

            GUIStyle comboList = new GUIStyle();
            comboList.normal.textColor = Color.white;
            comboList.alignment = TextAnchor.MiddleCenter;
            comboList.normal.background = GameResources.BlackBG;
            comboList.hover.textColor = Color.red;
            comboList.hover.background = GameResources.BlackBG;

            GUI.Box(new Rect(38 * wPercent, 82.5f * hPercent, 35.5f * wPercent, 5 * hPercent), "", border);
            message = GUI.TextField(new Rect(38 * wPercent + borderThickness / 2, 82.5f * hPercent + borderThickness / 2, 35.5f * wPercent - borderThickness, 5 * hPercent - borderThickness), message, textBox);
            GUI.Box(new Rect(0, 0, 38 * wPercent, Screen.height), "", box);
            GUI.Box(new Rect(73.5f * wPercent, 0, 26.5f * wPercent, Screen.height), "", box);

            GUI.Box(new Rect(28 * wPercent, 5 * hPercent, 45.5f * wPercent, 77.5f * hPercent), "", border);
            GUI.Box(new Rect(28 * wPercent + borderThickness / 2, 5 * hPercent + borderThickness / 2, 45.5f * wPercent - borderThickness, 77.5f * hPercent - borderThickness), chatChannels[selectedChatChannel].log, box);

            GUI.Box(new Rect(73.5f * wPercent, 5 * hPercent, 23.5f * wPercent, 77.5f * hPercent), "", border);
            GUI.Box(new Rect(73.5f * wPercent + borderThickness / 2, 5 * hPercent + borderThickness / 2, 23.5f * wPercent - borderThickness, 77.5f * hPercent - borderThickness), "", box);

            GUI.Box(new Rect(73.5f * wPercent, 82.5f * hPercent, 23.5f * wPercent, 5 * hPercent), "", border);
            if (GUI.Button(new Rect(73.5f * wPercent + borderThickness / 2, 82.5f * hPercent + borderThickness / 2, 23.5f * wPercent - borderThickness, 5 * hPercent - borderThickness), "send", button))
            {
                Client.Send(Message.GetBytes(new Message(MESSAGE_TYPE.REQUEST_MESSAGE, Client.login, new object[] { chatChannels[selectedChatChannel].name, message })));
                message = "";
            }

            GUI.Button(new Rect(28 * wPercent, 90 * hPercent, 22 * wPercent, 5 * hPercent), "logout", button);

            GUI.Button(new Rect(51.5f * wPercent, 90 * hPercent, 22 * wPercent, 5f * hPercent), "upgrade", button);

            if (GUI.Button(new Rect(75 * wPercent, 90 * hPercent, 22 * wPercent, 5f * hPercent), "start", button))
            {
                Client.Send(Message.GetBytes(new Message(MESSAGE_TYPE.REQUEST_START_GAME, Client.login, null)));
            }
            
            GUI.Box(new Rect(3 * wPercent, 5 * hPercent, 23.5f * wPercent, 90 * hPercent), "", border);
            GUI.Box(new Rect(3 * wPercent + borderThickness / 2, 5 * hPercent + borderThickness / 2, 23.5f * wPercent - borderThickness, 90 * hPercent - borderThickness), "", box);
            for (int i = 0; i < 15; i++)
            {
                if (i < players.Count)
                {
                    GUIStyle style;
                    if (players[i] == Client.login)
                    {
                        style = nameSelected;
                    }
                    else
                    {
                        style = name;
                    }
                    if (GUI.Button(new Rect(4.5f * wPercent, (6 * hPercent) + ((5.5f * hPercent) * i), 20.5f * wPercent, 6 * hPercent), players[i], style))
                    {
                        selectedPlayer = players[i];
                    }
                }
            }

            UIResult u = ComboBox.Show(new Rect(28 * wPercent, 82.5f * hPercent, 10 * wPercent, 5 * hPercent), ChatChannel.ToNameArray(chatChannels), comboButton, comboList, selectedChatChannel, isExpandedChatChannelSelector);
            selectedChatChannel = u.selectedIndex;
            isExpandedChatChannelSelector = u.isExpanded;

        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message + "\n" + exception.StackTrace);
        }
    }
}
