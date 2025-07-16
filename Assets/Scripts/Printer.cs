using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Printer : Interactible
{
    public UnityEvent onJobCompleted;

    public Transform documentAnchor;

    [Range(0, 100)]
    public int inkRemaining = 100;
    public int paperRemaining = 50;
    public float inkTickRateSeconds = 1f;
    public float paperTickRateSeconds = 5f;

    
    private Document currentDocument { get; set; }
    private float inkTimer = 0f;
    private float paperTimer = 0f;

    public override void interact()
    {
        base.interact();


        //check if the player is holding something, if not try to give them the current document.
        Pickup playerPickup = player.getSelectedPickup();
        if (!playerPickup)
        {
            releaseCurrentDocument();
            return;
        }

        //if they are holding something, start figuring out what it is.
        if (playerPickup.GetComponent<Document>()) //maybe change this to tags instead of using getcomponent?
        {
            if (!currentDocument)
            {
                currentDocument = player.releaseHeldDocument();
                currentDocument.transform.SetParent(documentAnchor);
                currentDocument.transform.DOLocalMove(documentAnchor.localPosition, player.pickupAnimDuration);
                currentDocument.transform.DOLocalRotate(documentAnchor.localEulerAngles, player.pickupAnimDuration);
            }
        }
        //TODO: Check for pickups like ink and paper and handle them appropriately
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


    public void grabHeldPickup()
    {
        
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
