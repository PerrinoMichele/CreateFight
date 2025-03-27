using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float moveSpeed = 3f;  // Movement speed of the enemy
    public float rotationSpeed = 5f;  // Speed at which the enemy rotates to face the player

    private void Start()
    {
        player = FindFirstObjectByType<InputPlayer>().transform;
    }
    void Update()
    {
        // Check if the player is assigned
        if (player != null)
        {
            // Calculate direction towards the player
            Vector3 direction = (player.position - transform.position).normalized;

            // Rotate the enemy to face the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Move the enemy towards the player
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }
}