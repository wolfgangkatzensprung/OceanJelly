using UnityEngine;

public class PlayerTesting : MonoBehaviour
{
    float startSpeed;
    public float hackSpeed = 1337f;

    private void Start()
    {
        startSpeed = PlayerMovement.Instance.runSpeed;
    }

    //private void Update()
    //{
    //    if (Input.GetButtonDown("Speedhack"))
    //            ToggleSpeedHack();
    //}

    void ToggleSpeedHack()
    {
        Debug.Log("Toggle Speedhack");

        if (PlayerMovement.Instance.runSpeed == startSpeed)
            PlayerMovement.Instance.runSpeed = hackSpeed;
        else
        {
            PlayerMovement.Instance.runSpeed = startSpeed;
        }
    }
}
