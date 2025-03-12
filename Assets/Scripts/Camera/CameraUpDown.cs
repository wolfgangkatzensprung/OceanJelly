using Cinemachine;
using UnityEngine;

public class CameraUpDown : MonoBehaviour
{
    public Animator camAnim;
    public CinemachineConfiner confiner;

    //void Update()
    //{
    //    if (PlayerManager.Instance.player != null)
    //        if (PlayerManager.Instance.player.activeSelf)
    //            if (PlayerJump.Instance.grounded)
    //            {
    //                if (Input.GetAxisRaw("Vertical") > 0.7f)
    //                {
    //                    LookUp();
    //                }
    //                else if (Input.GetAxisRaw("Vertical") < -0.7f)
    //                {
    //                    LookDown();
    //                }

    //                if (Input.GetButtonUp("Vertical") || PlayerManager.Instance.GetIsMovingX())
    //                {
    //                    ResetLookingUpDown();
    //                }
    //            }
    //            else if (!(PlayerJump.Instance.grounded) || Input.GetAxis("Horizontal") == 0)
    //            {
    //                ResetLookingUpDown();
    //            }

    //}

    private void ResetLookingUpDown()
    {
        PlayerManager.Instance.anim.SetBool("isLookingUp", false);
        PlayerManager.Instance.anim.SetBool("isLookingDown", false);
        camAnim.SetBool("isLookingUp", false);
        camAnim.SetBool("isLookingDown", false);
    }

    void LookUp()
    {
        if (!PlayerManager.Instance.anim.GetBool("isLookingUp"))
        {
            PlayerManager.Instance.anim.SetBool("isLookingUp", true);
            camAnim.SetBool("isLookingUp", true);
        }
    }

    void LookDown()
    {
        if (!PlayerManager.Instance.anim.GetBool("isLookingDown"))
        {
            PlayerManager.Instance.anim.SetBool("isLookingDown", true);
            camAnim.SetBool("isLookingDown", true);
        }
    }
}
