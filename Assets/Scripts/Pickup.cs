using UnityEngine;
using UnityEngine.Events;

public class Pickup : Interactible
{
    public UnityEvent onPickedUp;
    public UnityEvent onDropped;

    public Sprite icon;

    public override void interact()
    {
        base.interact();
        player.grabPickup(this);
    }
}
