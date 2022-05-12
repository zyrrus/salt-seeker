using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private CharacterController cc;
    private PlayerInputAction playerInputAction;
    private Animator animator;

    private Vector2 moveInput;
    private Vector3 appliedMovement;
    private bool isWalking;

    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float normalGravity = -9.8f;
    [SerializeField] private float groundedGravity = -0.5f;

    private int isWalkingHash;
    private int velocityHash;

    private void Awake()
    {
        // Get references

        cc = GetComponent<CharacterController>();
        playerInputAction = new PlayerInputAction();
        animator = GetComponentInChildren<Animator>();


        // Set the hashes

        isWalkingHash = Animator.StringToHash("IsWalking");
        velocityHash = Animator.StringToHash("Velocity");


        // Tie input handlers to PlayerInputAction

        playerInputAction.Player.Move.started += OnMoveInput;
        playerInputAction.Player.Move.performed += OnMoveInput;
        playerInputAction.Player.Move.canceled += OnMoveInput;
    }

    private void OnEnable() => playerInputAction.Enable();
    private void OnDisable() => playerInputAction.Disable();

    private void Update()
    {
        HandleGravity();
        HandleRotation();
        HandleAnimator();

        cc.Move(appliedMovement * Time.deltaTime);
    }

    // Utilities

    private void HandleGravity()
    {
        if (cc.isGrounded) appliedMovement.y = groundedGravity;
        else appliedMovement.y += normalGravity;
    }

    private void HandleRotation()
    {
        Vector3 lookTarget;
        lookTarget.x = moveInput.x;
        lookTarget.y = 0f;
        lookTarget.z = moveInput.y;

        Quaternion currentRotation = transform.rotation;

        if (isWalking && lookTarget.magnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleAnimator()
    {
        animator.SetFloat(velocityHash, cc.velocity.magnitude / moveSpeed);
        isWalking = moveInput.magnitude > 0f;
        animator.SetBool(isWalkingHash, isWalking);
    }


    // Handle Inputs

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        Vector2 scaledInput = moveInput * moveSpeed;
        appliedMovement.x = scaledInput.x;
        appliedMovement.z = scaledInput.y;
    }

}
