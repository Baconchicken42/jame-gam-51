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

        if (interactAction.action.WasPressedThisFrame() && interactibleInRange != null)
        {
            interactibleInRange.interact();
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
        pickup.transform.DOMove(pickupAnchor.position, pickupAnimDuration);
        pickup.onPickedUp.Invoke();

        inventory[slot] = pickup;
        selectedPickup = slot;
        displaySelectedPickup();

        Debug.Log("grabbed " + pickup.name);
        debugInventoryContents();
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
                    inventory[i].enabled = true;
                else
                    inventory[i].enabled = false;
            }
        }
    }

    private void debugInventoryContents()
    {
        string ret = "Inventory contains: \n"
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
