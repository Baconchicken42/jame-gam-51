using UnityEngine;

public class PickupSpawner : Interactible
{
    public GameObject pickupPrefab;
    private Pickup pickup;

    public override void Start()
    {
        base.Start();

        pickup = pickupPrefab.GetComponent<Pickup>();

        if (!pickup)
        {
            Debug.LogError($"{pickupPrefab.name} is not a pickup!");
        }
    }

    public override void interact()
    {
        base.interact();

        if (player.isInventorySlotAvailable())
        {
            Pickup newPickup = Instantiate(pickupPrefab, transform).GetComponent<Pickup>();
            player.grabPickup(newPickup);
        }
    }

}
