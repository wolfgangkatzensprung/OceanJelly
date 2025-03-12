using UnityEngine;

public class MoveOutScreen : MonoBehaviour
{
    bool movingOut;
    public Vector2 direction = Vector2.left;

    private void FixedUpdate()
    {
        if (movingOut)
        {
            Debug.Log("Moving out of screen");
            transform.Translate(direction);
        }
    }

    public void StartMovingOut()
    {
        movingOut = true;
    }
}
