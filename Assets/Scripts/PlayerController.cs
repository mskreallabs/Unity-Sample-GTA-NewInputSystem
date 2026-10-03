using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rotationSpeed = 5f;

    [SerializeField] float runningSpeedMultiplier;
    [SerializeField] Transform cameraTransform;


    [SerializeField] InputActionReference moveInputAction;
    [SerializeField] InputActionReference runInputAction;

    Vector2 moveInput;

    Rigidbody rb;
    Animator anim;
    float activeRunningSpeedMultiplier = 1f;

    readonly int walkingAnimatorHash = Animator.StringToHash("Walking");
    readonly int runningAnimatorHash = Animator.StringToHash("Running");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        moveInput = moveInputAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

        Vector3 velocity = moveDirection * movementSpeed * activeRunningSpeedMultiplier;

        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        if (moveDirection == Vector3.zero)
        {
            anim.SetBool(walkingAnimatorHash, false);
            return;
        }

        anim.SetBool(walkingAnimatorHash, true);

        if (runInputAction.action.IsPressed())
        {
            anim.SetBool(runningAnimatorHash, true);
            activeRunningSpeedMultiplier = runningSpeedMultiplier;
        }
        else
        {
            anim.SetBool(runningAnimatorHash, false);
            activeRunningSpeedMultiplier = 1f;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        Quaternion finalRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed);

        rb.MoveRotation(finalRotation);

    }
}
