using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference interactAction;
    public InputActionReference sprintAction;

    public float movementSpeed = 10.0f;
    public float sprintMultiplier = 1.75f;


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
        transform.position += new Vector3(moveAxis.x, 0, moveAxis.y) * movementSpeed * sprintMultiplier * Time.deltaTime;
    }
}
