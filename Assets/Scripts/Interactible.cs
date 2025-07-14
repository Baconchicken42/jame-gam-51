using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class Interactible : MonoBehaviour
{
    public UnityEvent onInteracted;


    private Outline[] outlines;
    protected Player player;


    private void Start()
    {
        outlines = GetComponentsInChildren<Outline>();
        removeHighlight();

        player = GameObject.FindWithTag("player").GetComponent<Player>();
    }

    public virtual void interact()
    {
        Debug.Log(name + " interacted!");
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
