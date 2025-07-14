using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class Interactible : MonoBehaviour
{
    public UnityEvent onInteracted;


    private Outline[] outlines;


    private void Start()
    {
        outlines = GetComponentsInChildren<Outline>();
        removeHighlight();
    }

    public void interact()
    {
        Debug.Log(name + " activated!");
        onInteracted.Invoke();
    }

    public void applyHighlight()
    {
        foreach (Outline ol in outlines)
        {
            ol.enabled = true;
        }
    }

    public void removeHighlight()
    {
        foreach (Outline ol in outlines)
        {
            ol.enabled = false;
        }
    }
}
