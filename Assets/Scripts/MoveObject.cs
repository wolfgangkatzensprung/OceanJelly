using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Vector2 move = new Vector2();
    private void Update()
    {
        transform.Translate(move * Time.deltaTime);
    }
}
