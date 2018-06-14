using UnityEngine;
using System.Collections;

public class DEBUG : MonoBehaviour
{
    ChannelMenu contextMenu;

	void Start ()
    {
        contextMenu = ChannelMenu.Instance();
	}

    void OnGUI()
    {
        if (contextMenu.isShown)
        {
            contextMenu.Show();
        }
    }

	void Update ()
    {
        if (Input.GetMouseButtonDown(1))
        {
            contextMenu.position = new Vector3(Input.mousePosition.x, -(Input.mousePosition.y - Screen.height));
            contextMenu.isShown = !contextMenu.isShown;
            //contextMenu.showWindow = false;
        }
	}
}
