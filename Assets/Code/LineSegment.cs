using UnityEngine;

[System.Serializable]
public struct LineSegment
{
    public Vector2 A;
    public Vector2 B;

    public LineSegment(
        Vector2 a,
        Vector2 b
    )
    {
        A = a;
        B = b;
    }

    public LineSegment(LineSegment other)
    {
        A = other.A;
        B = other.B;
    }
};