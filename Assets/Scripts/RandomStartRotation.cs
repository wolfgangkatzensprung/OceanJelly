using UnityEngine;

public class RandomStartRotation : MonoBehaviour
{
    void Start()
    {
        // Generate a random angle in degrees
        float randomAngle = Random.Range(0f, 360f);

        // Convert the angle to radians
        float randomAngleRad = randomAngle * Mathf.Deg2Rad;

        // Set the object's rotation
        transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
    }
}
