using UnityEngine;

public class EntityPatrol : MonoBehaviour
{
    [Tooltip("up / down / left / right")][SerializeField] string direction;
    [Range(0, 100)][SerializeField] float distance;
    [SerializeField] float speed = 1f;
    Vector3 startPosition;
    Vector3 targetPosition;
    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition; // damit das nicht genutzte x oder y auf der selben linie bleibt
        SetTargetPosition();
    }

    private void FixedUpdate()
    {
        CheckForPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }

    void SetTargetPosition()
    {
        if (direction == "up")
        {
            targetPosition.y = startPosition.y + distance;
        }
        else if (direction == "down")
        {
            targetPosition.y = startPosition.y - distance;
        }
        else if (direction == "right")
        {
            targetPosition.x = startPosition.x + distance;
        }
        else if (direction == "left")
        {
            targetPosition.x = startPosition.x - distance;
        }
        //Debug.Log("New TargetPosition: " + targetPosition);
    }

    void CheckForPosition()
    {
        if (direction == "up")
        {
            if (transform.position.y > startPosition.y + (distance - 1))
            {
                targetPosition = startPosition;
                //Debug.Log("Return to startPosition");
            }
            if (transform.position.y < startPosition.y + 1)
            {
                SetTargetPosition();
            }
        }
        else if (direction == "down")
        {
            if (transform.position.y < startPosition.y - (distance - 1))
            {
                targetPosition = startPosition;
            }
            if (transform.position.y > startPosition.y - 1)
            {
                SetTargetPosition();
            }
        }
        else if (direction == "right")
        {
            if (transform.position.x > startPosition.x + (distance - 1))
            {
                targetPosition = startPosition;
            }
            if (transform.position.x < startPosition.x + 1)
            {
                SetTargetPosition();
            }
        }
        else if (direction == "left")
        {
            if (transform.position.x < startPosition.x - (distance - 1))
            {
                targetPosition = startPosition;
            }
            if (transform.position.x > startPosition.x - 1)
            {
                SetTargetPosition();
            }
        }
    }
}
