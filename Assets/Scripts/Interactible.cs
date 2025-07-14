using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour
{
    public UnityEvent onInteracted;
    public Material highlightMaterial;


    public void interact()
    {
        Debug.Log(name + " activated!");
        onInteracted.Invoke();
    }

    public void applyHighlight()
    {
        //TODO: apply outline shader to all children
    }

    public void removeHighlight()
    {
        //TODO: remove outline shader from all children
    }
}
