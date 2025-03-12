using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public bool openGate;
    Vector2 startPosition;
    public bool closeAgain;

    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (openGate)
        {
            transform.position -= new Vector3(0, .1f, 0);
            if (transform.position.y < startPosition.y - 100f)
            {
                if (!closeAgain)
                    Destroy(gameObject);
                else
                {
                    openGate = false;
                }
            }
        }
        else if (closeAgain && transform.position.y < startPosition.y)
        {
            Debug.Log("Close It");
            transform.position += new Vector3(0, .1f, 0);
        }
    }

    public void SetOpenGate(bool open)
    {
        openGate = open;
    }

}
