using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Employee : Interactible
{
    public UnityEvent onOrderCompleted;

    [Header("References")]
    public GameObject documentPrefab;
    public Transform documentAnchor;

    [Header("Settings")]
    public int id = 0;
    //[HideInInspector]
    public List<Order> orders;

    [Header("UI")]
    public RectTransform groupRectTransform;
    public Image docImgTemplate;
    private List<Image> docImgs;
    private UIManager uiManager;

    public override void Start()
    {
        base.Start();

        uiManager = FindAnyObjectByType<UIManager>();
        docImgs = new List<Image>();
    }

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

            Image linkedImg = docImgs[orders.IndexOf(matchedOrder)];
            docImgs.Remove(linkedImg);
            Destroy(linkedImg.gameObject);

            orders.Remove(matchedOrder);
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

            Color newColor = docImgs[orders.IndexOf(nextOrder)].color;
            newColor.a = 0.3f;
            docImgs[orders.IndexOf(nextOrder)].color = newColor;

            player.grabPickup(newDocComponentReference);
            nextOrder.wasDispensed = true;
        }
        
    }

    public void addOrder(Order newOrder)
    {
        orders.Add(newOrder);
        GameObject docImgElement = Instantiate(docImgTemplate.gameObject, groupRectTransform);
        docImgElement.SetActive(true);
        Image newDocImg = docImgElement.GetComponent<Image>();
        newDocImg.sprite = uiManager.getMatchingIcon(newOrder.color, newOrder.type);
        docImgs.Add(newDocImg);
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
