using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float moveSpeed = 3f;  // Movement speed of the flying enemy
    public float rotationSpeed = 5f;  // Rotation speed to face the player

    private void Start()
    {
        player = FindFirstObjectByType<InputPlayer>().transform;
    }

    void Update()
    {
        if (player != null)
        {
            // Move towards the player, both horizontally and vertically
            Vector3 direction = player.position - transform.position;

            // Ensure we only rotate in the X-Z plane (horizontal rotation)
            direction.y = 0;

            // Calculate the movement direction
            Vector3 targetPosition = transform.position + direction.normalized * moveSpeed * Time.deltaTime;

            // Keep the y position fixed at 1
            targetPosition.y = 1;

            // Rotate to face the player horizontally (only X-Z plane)
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Move towards the target position, but with fixed Y
            transform.position = targetPosition;
        }
    }
}