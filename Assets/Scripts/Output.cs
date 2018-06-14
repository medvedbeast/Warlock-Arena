using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Output
{
    public static void Write(Exception exception)
    {
        FileStream fs = new FileStream("debug.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine("*******************************************************");
        sw.WriteLine("[" + DateTime.Now + "] -> Exception");
        sw.WriteLine("*******************************************************");
        sw.WriteLine(exception.Message);
        sw.WriteLine(exception.StackTrace);
        sw.WriteLine("*******************************************************");
        sw.Close();
        fs.Close();
    }

    public static void Write(object o)
    {
        FileStream fs = new FileStream("debug.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine("*******************************************************");
        sw.WriteLine("[" + DateTime.Now + "] -> Output");
        sw.WriteLine("*******************************************************");
        sw.WriteLine(o);
        sw.WriteLine("*******************************************************");
        sw.Close();
        fs.Close();
    }
}
