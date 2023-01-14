using UnityEngine;

public static class HexMetrics
{
    public const float OuterRadius = 10f;
    public const float InnerRadius = OuterRadius * 0.866025404f;
    public const float SolidFactor = 0.75f;
    public const float BlendFactor = 1f - SolidFactor;

    public static readonly Vector3[] Corners =
    {
        new Vector3(0f, 0f, OuterRadius),
        new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
        new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(0f, 0f, -OuterRadius),
        new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius),
        new Vector3(0f, 0f, OuterRadius),
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return Corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return Corners[(int)direction + 1];
    }

    public static Vector3 GetCorner(HexDirection direction, int cornerNumber)
    {
        return Corners[(int)direction + (cornerNumber-1)];
    }

    public static Vector3 GetSolidCorner(HexDirection direction, int cornerNumber)
    {
        return Corners[(int)direction + (cornerNumber-1)] * SolidFactor;
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (Corners[(int)direction] + Corners[(int)direction + 1]) * BlendFactor;
    }
}
