using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Document : Pickup
{
    public UnityEvent onCompleted;

    public GameObject copy;

    [Min(0)]
    public float progress = 0;
    [Min(0)]
    public float timeRequired = 15;
    public documentType type = documentType.star;
    public documentColor color = documentColor.red;
    //public int inkRequired = 10;
    //public int paperRequired = 5;

    [HideInInspector]
    public bool isCompleted = false;

    public void complete()
    {
        copy.gameObject.SetActive(true);
        isCompleted = true;

        Debug.Log($"Document {name} has been completed!");
        onCompleted.Invoke();
    }
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
