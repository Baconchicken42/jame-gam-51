using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Printer : Interactible
{
    public UnityEvent onJobCompleted;

    [Header("References")]
    public Transform documentAnchor;
    public SetEmissiveColor colorChanger;

    [Header("Settings")]
    [Range(0,40)]
    public int inkRemaining = 40;
    [Range(0,10)]
    public int paperRemaining = 10;
    public float inkTickRateSeconds = 1f;
    public float paperTickRateSeconds = 5f;



    [Header("UI")]
    //public Canvas uiCanvas;
    //private Camera mainCamera;
    public Image inkImg;
    public Image inkWarning;
    public Image inkEmpty;
    private bool isInkIconOut = false;
    public Image paperImg;
    public Image paperWarning;
    public Image paperEmpty;
    private bool isPaperIconOut = false;
    public Image docImg;
    public Image docCheck;
    public float iconAppearTimeSeconds = .3f;


    private Document currentDocument { get; set; }
    private float inkTimer = 0f;
    private float paperTimer = 0f;

    public override void Start()
    {
        base.Start();

        inkImg.transform.localScale = Vector3.zero;
        paperImg.transform.localScale = Vector3.zero;
        docImg.transform.localScale = Vector3.zero;

        //mainCamera = FindAnyObjectByType<Camera>();

    }

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
                docImg.sprite = currentDocument.icon;
                docImg.transform.DOScale(Vector3.one, iconAppearTimeSeconds);
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
        colorChanger.SetDefault();

        if (currentDocument)
        {
            if (inkRemaining > 0 && paperRemaining > 0 && !currentDocument.isCompleted)
            {
                if (currentDocument.progress >= currentDocument.timeRequired) //if document is finished
                {
                    currentDocument.complete();
                    docCheck.gameObject.SetActive(true);
                    colorChanger.SetSuccess();
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

        }


        if (inkRemaining < 15 && inkRemaining > 0 && !isInkIconOut)
        {
            inkEmpty.gameObject.SetActive(false);
            inkWarning.gameObject.SetActive(true);
            inkImg.transform.DOScale(Vector3.one, iconAppearTimeSeconds);
            isInkIconOut = true;
        }
        else if (inkRemaining == 0)
        {
            inkWarning.gameObject.SetActive(false);
            inkEmpty.gameObject.SetActive(true);
            colorChanger.SetFail();
        }
        else if (isInkIconOut)
        {
            inkImg.transform.DOScale(Vector3.zero, iconAppearTimeSeconds);
            isInkIconOut = false;
        }

        if (paperRemaining < 3 && paperRemaining > 0 && !isPaperIconOut)
        {
            paperEmpty.gameObject.SetActive(false);
            paperWarning.gameObject.SetActive(true);
            paperImg.transform.DOScale(Vector3.one, iconAppearTimeSeconds);
            isPaperIconOut = true;
        }
        else if (paperRemaining == 0)
        {
            paperWarning.gameObject.SetActive(false);
            paperEmpty.gameObject.SetActive(true);
            colorChanger.SetFail();
        }
        else if (isInkIconOut)
        {
            paperImg.transform.DOScale(Vector3.zero, iconAppearTimeSeconds);
            isPaperIconOut = false;
        }


        //uiCanvas.transform.eulerAngles = Vector3.RotateTowards(uiCanvas.transform.eulerAngles, mainCamera.transform.position, 1, 0);
    }

    private void releaseCurrentDocument()
    {
        if (currentDocument)
        {
            if (player.grabPickup(currentDocument)) //try to make the player grab document, and if successful clear reference
            {  
                currentDocument = null;
                docImg.transform.DOScale(Vector3.zero, iconAppearTimeSeconds);
                docCheck.gameObject.SetActive(false);
            }
        }
           
    }
}
