using System.Collections;
using UnityEngine;

public class HookTrigger : MonoBehaviour
{
    Animator anim;
    DistanceJoint2D dj;
    PlayerMovement pm;
    NPC_LookAtPlayer lap;

    [SerializeField] float djDistance = 15;
    [SerializeField] float hookedConstantForce = -15f;
    public bool isHooked = false;

    bool isHookRdy = true;
    Quaternion startRotation;

    private void Start()
    {
        anim = GetComponent<Animator>();
        dj = PlayerManager.Instance.player.GetComponentInChildren<DistanceJoint2D>();
        pm = PlayerManager.Instance.player.GetComponent<PlayerMovement>();
        lap = GetComponent<NPC_LookAtPlayer>();
        dj.autoConfigureDistance = false;
        dj.enabled = false;
        startRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (!isHookRdy)
        {
            return;
        }
        if (isHooked)
        {
            //dj.connectedAnchor = transform.position;
            //if (Input.GetButtonDown("Attack"))   // + || if dash ?
            //{
            //    if (dj != null)
            //    {
            //        if (dj.enabled)
            //        {
            //            dj.enabled = false;
            //        }
            //        ExitKraken();
            //    }
            //}
            //else if (Input.GetButtonDown("Jump"))
            //{
            //    if (PlayerManager.Instance.anim.GetBool("isHooked") && dj != null)
            //    {
            //        if (dj.enabled)
            //        {
            //            dj.enabled = false;
            //        }
            //        PlayerManager.Instance.anim.SetBool("isJumpingHook", true);
            //        ExitKraken();
            //    }
            //}
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isHookRdy)
        {
            return;
        }
        if (!isHooked && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("krakeActive");
            anim.SetBool("krakeActive", true);
            PlayerManager.Instance.anim.SetBool("isHooked", true);
            dj.distance = djDistance;
            dj.enabled = true;
            isHooked = true;
            pm._isHooked = true;
            pm.canMove = false;
            lap.SetLookAtTarget(true);
        }
    }

    private void ExitKraken()
    {
        Debug.Log("ExitKreiselkraken");
        PlayerManager.Instance.anim.SetBool("isHooked", false);
        isHooked = false;
        pm._isHooked = false;
        anim.SetBool("krakeActive", false);
        lap.SetLookAtTarget(false);
        StartCoroutine(HookCooldown());
        transform.rotation = startRotation;
    }

    IEnumerator HookCooldown()
    {
        isHookRdy = false;
        yield return new WaitForSeconds(.3f);
        isHookRdy = true;
    }
}