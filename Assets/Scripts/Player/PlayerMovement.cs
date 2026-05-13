using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    private float moveSpeed;
    private Vector2 moveDirection;

    // Set this scripts movespeed to a value called from another script/component
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    // Gets player movement input, and normalizes a vector to be used to move later
    void ProcessInputs ()
    {
        // Gets a value, either 0 or 1, based on Horizontal/Vertical input value.
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    // Uses previous vector determined from player input to move the player
    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    // Checks player's inputs every frame
    void Update()
    {
        ProcessInputs();
    }

    // Moves every fixed amount of time, regardless of frame rate
    private void FixedUpdate()
    {
        Move();
    }
}
