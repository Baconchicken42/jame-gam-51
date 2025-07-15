using UnityEngine;

public class Document : Pickup
{
    public GameObject copy;

    [Range(0, 100)]
    public float progress = 0;
    public documentType type = documentType.star;
    public documentColor color = documentColor.red;
}

public enum documentType
{
    star,
    circle,
    triangle,
    square
}

public enum documentColor
{
    red,
    blue,
    yellow,
    pink,
    purple
}
