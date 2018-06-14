using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IContextMenu : MonoBehaviour
{
    public List<string> buttons = new List<string>();
    public bool isShown = false;
    public Vector2 position = new Vector2(0, 0);
    public bool showWindow = false;

    public virtual void Show() { }
}
