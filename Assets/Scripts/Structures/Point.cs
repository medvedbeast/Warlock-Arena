using UnityEngine;
using System.Collections;

public class Point
{
    public float X;
    public float Y;

    public Point()
    {
        X = 0;
        Y = 0;
    }

    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Point Clone()
    {
        return new Point(X, Y);
    }

    public override string ToString()
    {
        return "[" + X + ":" + Y + "]";
    }
}
