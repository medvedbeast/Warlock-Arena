using UnityEngine;
using System.Collections;

public class UIResult
{
    public int selectedIndex;
    public bool isExpanded;

    public UIResult(int i, bool b)
    {
        selectedIndex = i;
        isExpanded = b;
    }
}
