using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float gravity = -15f;

    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private Transform CameraTransform;
    private CharacterController characterController;

    [SerializeField]
    private Vector3 playerVelocity;
    private Vector2 moveInput = Vector2.zero;
    private Boolean isJumping = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            Debug.Log("Grounded, resetting velocity.");
            playerVelocity.y = 0;
        }
        var forward = CameraTransform.forward;
        forward.y = 0;
        var right = CameraTransform.right;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = moveInput.x * right + moveInput.y * forward;
        characterController.Move(move * Time.deltaTime * speed);

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.performed;
        Debug.Log("isGrounded " + characterController.isGrounded);
        if (isJumping && characterController.isGrounded)
        {
            Debug.Log("Jumping!");
            playerVelocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight); // Jump height of 0.5 units
        }
    }
}
