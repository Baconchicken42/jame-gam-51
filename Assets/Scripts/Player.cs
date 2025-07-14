using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
        Vector2 moveAxis = (sprintAction.action.IsPressed()) ? moveAction.action.ReadValue<Vector2>() * sprintMultiplier : moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveAxis.x, 0, moveAxis.y);
        transform.position += moveDir * movementSpeed * sprintMultiplier * Time.deltaTime;
        if (moveDir != Vector3.zero)
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(moveDir), rotationStep);

    }
}
