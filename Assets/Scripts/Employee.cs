using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Employee : Interactible
{
    public UnityEvent onOrderCompleted;


    public GameObject documentPrefab;
    public Transform documentAnchor;

    public int id = 0;
    //[HideInInspector]
    public List<Order> orders;

    //private bool documentDispensed = false;

    public override void interact()
    {
        base.interact();

        Pickup heldPickup = player.getSelectedPickup();
        Document heldDoc = null;
        if (heldPickup)
            heldDoc = heldPickup.GetComponent<Document>();

        Order matchedOrder = null;
        if (heldDoc)
        {
            matchedOrder = getMatchingOrder(heldDoc);
        }

        if (matchedOrder != null && heldDoc.isCompleted) //at this point, the player is holding a completed document that matches one of the orders
        {
            player.releaseHeldPickup();
            heldDoc.transform.SetParent(documentAnchor);
            heldDoc.transform.DOLocalMove(documentAnchor.localPosition, player.pickupAnimDuration).OnComplete(() =>
            {
                Destroy(heldDoc.gameObject);
            });
            orders.Remove(matchedOrder);
            //documentDispensed = false;
            onOrderCompleted.Invoke();

            float points = Random.Range(1, 1);
            player.addPoints(points / 100f);
        }
        else if (orders.Count > 0 && player.isInventorySlotAvailable()) //if player doesn't have a valid doc, an order exists, and the player has room for it:
        {
            Order nextOrder = getFirstNonDispensedOrder();
            if (nextOrder == null) //if all orders have been dispensed, do nothing
                return;

            GameObject newDoc = Instantiate(documentPrefab, documentAnchor);
            Document newDocComponentReference = newDoc.GetComponent<Document>(); //something about this feels like it won't work how I think
            newDocComponentReference.timeRequired = nextOrder.timeRequired;
            newDocComponentReference.type = nextOrder.type;
            newDocComponentReference.color = nextOrder.color;

            player.grabPickup(newDocComponentReference);
            nextOrder.wasDispensed = true;
        }
        
    }


    public Order getMatchingOrder(Document doc)
    {
        foreach (Order o in orders)
        {
            if (o.color == doc.color && o.type == doc.type)
            {
                return o;
            }
        }

        return null;
    }

    public Order getFirstNonDispensedOrder()
    {
        foreach (Order o in orders)
        {
            if (o.wasDispensed == false)
                return o;
        }

        return null;
    }
}
