using System.Security.Cryptography;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] public float rotateSpeed;
    [SerializeField] public float additionalRotationSpeed;

    public float rotationDeterminer = 0f;

    void processInputs()
    {
        // Returns whether or not the player is currently holding down SPACE
        bool isAccelerating = Input.GetKey(KeyCode.Space);

        // Sets a determiner variable to 1 to boost rotation if holding space, OTHERWISE
        // it will remain at 0, meaning the additional rotation speed is multiplied by 0
        if (isAccelerating == true)
        {
            rotationDeterminer = 1f;
        }
    }
    void RotateRing()
    {
        // Rotates the current component based on the current vector, and then the speed
        this.transform.Rotate(Vector3.forward, (rotateSpeed + (additionalRotationSpeed * rotationDeterminer)) * Time.deltaTime);

        // Reset determiner to 0 so next frame can check if space is still being held down
        rotationDeterminer = 0f;
    }

    private void Update()
    {
        processInputs();
    }

    private void FixedUpdate()
    {
        RotateRing();
    }
}
