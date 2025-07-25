using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Document : Pickup
{
    public UnityEvent onCompleted;

    [Header("References")]
    public GameObject copy;

    [Header("Settings")]
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
    [HideInInspector]
    public UIManager uiManager;

    public override void Start() 
    {
        base.Start();

        uiManager = FindObjectsByType<UIManager>(FindObjectsSortMode.InstanceID)[0];

        icon = uiManager.getMatchingIcon(color, type);
        uiManager.refreshInventoryUI();
    }

    public void complete()
    {
        copy.gameObject.SetActive(true);
        isCompleted = true;
        icon = uiManager.getMatchingIcon(color, type, true);

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
