using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Outline))]
public class Player : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference interactAction;
    public InputActionReference sprintAction;

    public GameObject playerModel;

    public float movementSpeed = 10.0f;
    public float sprintMultiplier = 1.75f;
    [Range(0, 1)]
    public float rotationStep = .1f;
    public float interactRange = 1f;

    private Interactible interactibleInRange = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction.action.Enable();
        interactAction.action.Enable();
        sprintAction.action.Enable();
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
}
