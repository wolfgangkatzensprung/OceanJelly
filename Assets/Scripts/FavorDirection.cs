using UnityEngine;

public class FavorDirection : Singleton<FavorDirection>
{
    public float radius = 10.0f;
    public int numSamples = 30;
    public float lerpSpeed = 5f;

    private Vector2 sumDirections = Vector2.zero;
    private int numDirections = 0;
    private Vector2 prevPlayerPos;

    private Vector2 moveDirection;

    private void Start()
    {
        // Initialize the previous player position
        prevPlayerPos = transform.parent.position;
    }

    private void LateUpdate()
    {
        // Calculate the current direction of the player
        Vector2 currentDirection = (Vector2)transform.parent.position - prevPlayerPos;

        // If the parent hasn't moved, move the child towards the center of the parent
        if (currentDirection == Vector2.zero)
        {
            transform.position = transform.parent.position;
            return;
        }

        prevPlayerPos = transform.parent.position;

        // Add the current direction to the sum of directions and increment the direction count
        sumDirections += currentDirection;
        numDirections++;

        // If we've reached the maximum number of samples, remove the oldest direction
        if (numDirections > numSamples)
        {
            sumDirections -= sumDirections / numDirections;
            numDirections--;
        }

        // Calculate the average direction
        Vector2 avgDirection = sumDirections / numDirections;

        // Calculate the target position on a circle around the player
        Vector2 playerPos = transform.parent.position;
        Vector2 targetPosition = playerPos + avgDirection.normalized * radius;

        // Calculate the direction towards the target position
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Smooth the direction by taking the average of the previous direction and the current direction
        moveDirection = Vector2.Lerp(moveDirection, direction, Time.deltaTime * lerpSpeed);

        // Move the transform in the smoothed direction
        transform.position += (Vector3)moveDirection * Time.deltaTime * lerpSpeed;

        // Move the child object towards the target position while maintaining a fixed distance from the player
        transform.position = playerPos + moveDirection * radius;
    }
}
