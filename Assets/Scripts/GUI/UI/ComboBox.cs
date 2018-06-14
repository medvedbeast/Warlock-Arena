using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ComboBox
{
    
    public static UIResult Show(Rect position, object[] content, GUIStyle buttonStyle, GUIStyle listStyle, int selectedIndex, bool expanded)
    {
        try
        {
            float borderThickness = 4;
            GUIStyle border = new GUIStyle(buttonStyle);
            border.normal.background = GameResources.WhiteBG;
            border.hover.background = GameResources.WhiteBG;

            GUI.Box(position, "", border);
            if (GUI.Button(new Rect(position.x + borderThickness / 2, position.y + borderThickness / 2, position.width - borderThickness, position.height - borderThickness), content[selectedIndex].ToString(), buttonStyle))
            {
                expanded = !expanded;
            }
            if (expanded)
            {
                float height = position.height * content.Length;
                GUI.Box(new Rect(position.x, position.y - height - borderThickness * 2, position.width, height + borderThickness * 2), "", border);
                for (int i = 0; i < content.Length; i++)
                {
                    if (GUI.Button(new Rect(position.x + borderThickness / 2, position.y - (position.height * (i + 1)) - borderThickness / 2, position.width - borderThickness, position.height - borderThickness / 2), content[i].ToString(), listStyle))
                    {
                        selectedIndex = i;
                        expanded = !expanded;
                        break;
                    }
                }
            }
        }
        catch (System.Exception exception)
        {
			Debug.Log(exception.Message);
            //Output.Write(exception);
        }
        return new UIResult(selectedIndex, expanded);
    }

}
