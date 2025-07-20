using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Printer : Interactible
{
    public UnityEvent onJobCompleted;

    public Transform documentAnchor;

    [Range(0,40)]
    public int inkRemaining = 40;
    [Range(0,10)]
    public int paperRemaining = 10;
    public float inkTickRateSeconds = 1f;
    public float paperTickRateSeconds = 5f;

    
    private Document currentDocument { get; set; }
    private float inkTimer = 0f;
    private float paperTimer = 0f;

    public override void interact()
    {
        base.interact();



        Document heldDoc = null;
        Ink heldInk = null;
        Paper heldPaper = null;

        Pickup playerPickup = player.getSelectedPickup();
        if (playerPickup)
        {
            heldDoc = playerPickup.GetComponent<Document>();
            heldInk = playerPickup.GetComponent<Ink>();
            heldPaper = playerPickup.GetComponent<Paper>();
        }

        

        //if they are holding something, start figuring out what it is.
        if (heldDoc) //maybe change this to tags instead of using getcomponent?
        {
            if (!currentDocument)
            {
                currentDocument = player.releaseHeldPickup().GetComponent<Document>();
                currentDocument.transform.SetParent(documentAnchor);
                currentDocument.transform.DOLocalMove(documentAnchor.localPosition, player.pickupAnimDuration);
                currentDocument.transform.DOLocalRotate(documentAnchor.localEulerAngles, player.pickupAnimDuration);
                return;
            }
        }
        else if (heldInk)
        {
            inkRemaining += heldInk.inkAmount;
            Destroy(player.releaseHeldPickup().gameObject);
            return;
        }
        else if (heldPaper)
        {
            paperRemaining += heldPaper.paperAmount;
            Destroy(player.releaseHeldPickup().gameObject);
            return;
        }

        releaseCurrentDocument();
        //The way that was structured should make sure that if the player is holding a document they can still pick up the one in the printer
    }

    private void Update()
    {
        if (currentDocument)
        {
            if (inkRemaining > 0 && paperRemaining > 0 && !currentDocument.isCompleted)
            {
                if (currentDocument.progress >= currentDocument.timeRequired) //if document is finished
                {
                    currentDocument.complete();
                    onJobCompleted.Invoke();
                }
                else
                {
                    currentDocument.progress += Time.deltaTime;
                    inkTimer += Time.deltaTime;
                    paperTimer += Time.deltaTime;

                    if (inkTimer >= inkTickRateSeconds)
                    {
                        inkTimer = 0;
                        inkRemaining--;
                    }

                    if (paperTimer >= paperTickRateSeconds)
                    {
                        paperTimer = 0;
                        paperRemaining--;
                    }
                }
            }
            else
            {
                //TODO: update UI to show that ink and/or paper are needed
            }

        }
        
    }

    private void releaseCurrentDocument()
    {
        if (currentDocument)
        {
            if (player.grabPickup(currentDocument)) //try to make the player grab document, and if successful clear reference
                currentDocument = null;
        }
           
    }
}
