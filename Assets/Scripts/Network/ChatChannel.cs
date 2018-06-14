using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatChannel
{
    public string name;
    public string log;

    public ChatChannel(string name)
    {
        this.name = name;
        log = "";
    }

    public void AddMessage(object author, object message)
    {
        log += "\n" + author + " : " + message;
    }

    public static void Synchronize(object[] arr, List<ChatChannel> list)
    {
        foreach (object item in arr)
        {
            if (!ChatChannel.Contains(list, item.ToString()))
            {
                list.Add(new ChatChannel(item.ToString()));
            }
        }
    }

    public static bool Contains(List<ChatChannel> list, string str)
    {
        foreach (ChatChannel item in list)
        {
            if (item.name == str)
            {
                return true;
            }
        }
        return false;
    }

    public static int GetIndex(object name, List<ChatChannel> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name == name.ToString())
            {
                return i;
            }
        }
        return -1;
    }

    public static object[] ToNameArray(List<ChatChannel> list)
    {
        object[] arr = new object[list.Count];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = list[i].name;
        }
        return arr;
    }
}
