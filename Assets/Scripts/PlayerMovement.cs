using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    void Start()
    {

    }

    private void Update()
    {
        // Capture input for horizontal and vertical movement
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrow keys
        float vertical = Input.GetAxisRaw("Vertical"); // W/S or Up/Down arrow keys

        // Create a direction vector based on input
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        // Check if there is any input
        if (direction != Vector3.zero)
        {
            // Normalize the direction to prevent faster diagonal movement
            direction = direction.normalized;

            // Move the player based on direction and speed
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }
}
