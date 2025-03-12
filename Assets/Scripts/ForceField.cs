using UnityEngine;

public class ForceField : MonoBehaviour
{
    // The force with which the player will be pushed away
    public float pushForce = 5f;

    private Rigidbody2D playerRigidbody;

    // This function is called when the script is initialized
    private void Start()
    {
        // Get the player's Rigidbody2D component from the PlayerManager
        playerRigidbody = PlayerManager.Instance.rb;
    }

    // This function is called every frame while the player stays inside the trigger
    private void OnTriggerStay2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player") && playerRigidbody != null)
        {
            // Calculate the direction away from the trigger's center
            Vector2 direction = other.transform.position - transform.position;

            // Normalize the direction to get a consistent force regardless of the distance
            direction.Normalize();

            // Apply the force to push the player away from the trigger's center
            playerRigidbody.AddForce(direction * pushForce, ForceMode2D.Force);
        }
    }
}
