using System.Collections;
using UnityEngine;

public class PlayerJumpBoostReceiver : MonoBehaviour
{
    [Header("References")]
    public CharacterController characterController;

    [Header("Jump Boost Settings")]
    public float gravity = -25f;
    public float maxBoostTime = 2.5f;

    [Header("Air Control")]
    public bool allowPlayerMotorInAir = true;

    private Coroutine boostCoroutine;
    private bool isBoosting;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    public void BoostJump(float jumpHeight)
    {
        if (characterController == null)
        {
            Debug.LogWarning("CharacterController is missing on Player.");
            return;
        }

        if (isBoosting)
            return;

        Debug.Log("BOOST JUMP CALLED. Height: " + jumpHeight);

        boostCoroutine = StartCoroutine(BoostJumpRoutine(jumpHeight));
    }

    private IEnumerator BoostJumpRoutine(float jumpHeight)
    {
        isBoosting = true;

        float verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        float timer = 0f;

        while (timer < maxBoostTime)
        {
            timer += Time.deltaTime;

            verticalVelocity += gravity * Time.deltaTime;

            Vector3 verticalMove = Vector3.up * verticalVelocity * Time.deltaTime;

            characterController.Move(verticalMove);

            if (characterController.isGrounded && verticalVelocity < 0f && timer > 0.4f)
                break;

            yield return null;
        }

        isBoosting = false;
        boostCoroutine = null;

        Debug.Log("Boost finished. Player can move normally.");
    }
}