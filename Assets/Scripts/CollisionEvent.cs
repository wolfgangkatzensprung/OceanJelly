using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    public string collisionTag = "Player";
    public UnityEvent onCollisionWithPlayerEvent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(collisionTag))
        {
            // Call the UnityEvent when a collision with a player happens
            onCollisionWithPlayerEvent.Invoke();
        }
    }
}
