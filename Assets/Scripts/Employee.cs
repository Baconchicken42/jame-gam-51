using DG.Tweening;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;

public class Employee : Interactible
{
    public UnityEvent onOrderCompleted;

    public Transform documentAnchor;

    public int id = 0;
    public Document requiredDocument;
    public GameObject documentPrefab;

    private bool documentDispensed = false;

    public override void interact()
    {
        base.interact();

        if (requiredDocument && !documentDispensed && player.isInventorySlotAvailable())
        {
            GameObject newDoc = Instantiate(documentPrefab, documentAnchor);
            Document newDocComponentReference = newDoc.GetComponent<Document>(); //something about this feels like it won't work how I think
            newDocComponentReference.timeRequired = requiredDocument.timeRequired;
            newDocComponentReference.type = requiredDocument.type;
            newDocComponentReference.color = requiredDocument.color;

            player.grabPickup(newDocComponentReference);
        }
        else if (requiredDocument && documentDispensed)
        {
            Document heldDoc = player.getSelectedPickup().GetComponent<Document>();
            if (!heldDoc)
                return;

            if (heldDoc.isCompleted && heldDoc.type == requiredDocument.type && heldDoc.color == requiredDocument.color)
            {
                player.releaseHeldPickup();
                heldDoc.transform.SetParent(documentAnchor);
                heldDoc.transform.DOLocalMove(documentAnchor.localPosition, player.pickupAnimDuration).OnComplete(() =>
                {
                    Destroy(heldDoc.gameObject);
                });
                requiredDocument = null; //the order manager should be the one setting this
                documentDispensed = false;
                onOrderCompleted.Invoke();
            }
        }
        
    }
}
