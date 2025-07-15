using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.Controls;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;


public class Player : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference interactAction;
    public InputActionReference sprintAction;

    public GameObject playerModel;
    public Transform pickupAnchor;

    public float movementSpeed = 10.0f;
    public float sprintMultiplier = 1.75f;
    [UnityEngine.Range(0, 1)]
    public float rotationStep = .1f;
    public float interactRange = 1f;
    public float pickupAnimDuration = .3f;
    public int inventorySize = 3;

    private Interactible interactibleInRange = null;
    private Pickup[] inventory;
    private int selectedPickup = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction.action.Enable();
        interactAction.action.Enable();
        sprintAction.action.Enable();

        inventory = new Pickup[inventorySize];
    }

    // Update is called once per frame
    void Update()
    {
        //handle movement
        Vector2 moveAxis = (sprintAction.action.IsPressed()) ? moveAction.action.ReadValue<Vector2>() * sprintMultiplier : moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveAxis.x, 0, moveAxis.y);
        transform.position += moveDir * movementSpeed * sprintMultiplier * Time.deltaTime;
        //rotate player in movement direction
        if (moveDir != Vector3.zero)
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(moveDir), rotationStep);


        //handle interaction
        RaycastHit rayHit;
        Ray ray = new Ray(transform.position, playerModel.transform.forward);
        if (Physics.Raycast(ray, out rayHit, interactRange) && rayHit.transform != null)
        {
            interactibleInRange = rayHit.transform.gameObject.GetComponent<Interactible>(); //not sure if this works with something that extends
            if (interactibleInRange)
                interactibleInRange.applyHighlight();
        }
        else
        {
            if (interactibleInRange)
                interactibleInRange.removeHighlight();
            interactibleInRange = null;
        }

        if (interactAction.action.WasPressedThisFrame())
        {
            if (interactibleInRange)
                interactibleInRange.interact();
            else
                dropPickup();
        }
    }

    public void grabPickup(Pickup pickup)
    {
        int slot = getFirstEmptyInventorySlot();
        if (slot == -1)
        {
            Debug.Log("Inventory Full, cannot grab pickup " + pickup.name);
            return;
        }

        pickup.gameObject.transform.SetParent(pickupAnchor);
        pickup.transform.DOLocalMove(pickupAnchor.localPosition, pickupAnimDuration);
        pickup.onPickedUp.Invoke();

        inventory[slot] = pickup;
        selectedPickup = slot;
        displaySelectedPickup();

        Debug.Log("grabbed " + pickup.name);
        debugInventoryContents();
    }

    public void dropPickup()
    {
        if (!inventory[selectedPickup])
        {
            Debug.Log("Nothing to drop!");
            return;
        }

        Pickup currentPickup = inventory[selectedPickup];

        //check if there's something in front of the player before dropping
        RaycastHit rayHit;
        Ray ray = new Ray(transform.position, playerModel.transform.forward);


        currentPickup.transform.SetParent(null);

        LayerMask layerMask = LayerMask.GetMask("Floor");

        

        //if there's something in front of the player, drop straight to the floor, otherwise move pickup forward and then drop
        if (Physics.Raycast(ray, out rayHit, interactRange + .6f) && rayHit.transform != null)
        {
            //Get floor position
            RaycastHit rayHitFloor;
            Ray rayFloor = new Ray(currentPickup.transform.position, currentPickup.transform.up * -1);
            Physics.Raycast(rayFloor, out rayHitFloor, 1000, layerMask);

            currentPickup.transform.DOMove(rayHitFloor.point, pickupAnimDuration);
        }
        else
        {
            Sequence dropSequence = DOTween.Sequence();
            Vector3 pointInfront = playerModel.transform.TransformPoint(playerModel.transform.localPosition + new Vector3(0, 0, interactRange));
            Debug.DrawLine(transform.position, pointInfront, Color.red, 30);
            dropSequence.Append(currentPickup.transform.DOMove( pointInfront, pickupAnimDuration / 2f));

            //Get floor position in front
            RaycastHit rayHitFloor;
            Ray rayFloor = new Ray(pointInfront, currentPickup.transform.up * -1);
            Physics.Raycast(rayFloor, out rayHitFloor, 1000, layerMask);

            //Debug.Log(rayHitFloor.);
            dropSequence.Append(currentPickup.transform.DOMove(rayHitFloor.point, pickupAnimDuration / 2f));
            dropSequence.Play();
        }

        currentPickup.removeHighlight();
        currentPickup.onDropped.Invoke();

        //clean up inventory
        inventory[selectedPickup] = null;

        //TODO: set up functionality for switching selected pickup manually and call one of those functions here
        if (selectedPickup > 0)
            selectedPickup -= 1;
        displaySelectedPickup();
    }

    private int getFirstEmptyInventorySlot()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i])
                return i;
        }

        return -1;
    }

    private void displaySelectedPickup()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i])
            {
                if (i == selectedPickup)
                    inventory[i].gameObject.SetActive(true);
                else
                    inventory[i].gameObject.SetActive(false);
            }
        }
    }

    private void debugInventoryContents()
    {
        string ret = "Inventory contains: \n";
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i])
            {
                ret += inventory[i].name + "\n";
            }
            else
            {
                ret += "Empty Slot\n";
            }
        }
        ret += $"Currently Selected: {selectedPickup}\n";
        Debug.Log(ret);
    }
}
