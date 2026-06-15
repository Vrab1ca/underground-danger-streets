using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;

    private CharacterController controller;
    private Vector3 velocity;

    public bool IsGrounded => controller != null && controller.enabled && controller.isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        standingHeight = controller.height;
    }

    void Update()
    {
        if (controller == null || !controller.enabled || !gameObject.activeInHierarchy)
            return;

        bool grounded = controller.isGrounded;

        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        bool crouch = Input.GetKey(KeyCode.LeftControl);

        float speed = crouch ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded && !crouch)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        controller.height = Mathf.Lerp(
            controller.height,
            crouch ? crouchHeight : standingHeight,
            12f * Time.deltaTime
        );

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}