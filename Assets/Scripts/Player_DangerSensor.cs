using UnityEngine;

public class Player_DangerSensor : MonoBehaviour
{
    Camera mainCam;
    Animator anim;

    [Tooltip("Transform of DangerSensor Indicator. Must not be a child of player")]
    public Transform dangerSensor;
    SpriteRenderer sensorSr;
    [Tooltip("Max Alpha of Indicator Color")]
    public float maxAlpha = .7f;

    [Tooltip("Which layers to be detected by Danger Sense")]
    public LayerMask sensorMask;
    [Tooltip("Radius around player that is being checked for enemies")]
    public float sensorRadius = 120f;
    [Tooltip("Radius around player where the DangerSensor Indicator is placed to show the direction of potential threat")]
    public float indicatorRadius = 50f;

    Vector3 lastHitPos;

    [Header("Damping")]
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    int dangerSenseAbilityLevel = 0;

    private void Start()
    {
        mainCam = Camera.main;
        anim = dangerSensor.GetComponentInChildren<Animator>();
        sensorSr = dangerSensor.GetComponentInChildren<SpriteRenderer>();

        AbilityManager.Instance.onAbilityIncrease += SetAbilityLevel;
        PlayerHealth.Instance.onDeath += ResetAbilityLevel;
    }

    private void LateUpdate()
    {
        RadiusCheck();
    }
    private void SetAbilityLevel(AbilityManager.Abilities ability)
    {
        if (ability.Equals(AbilityManager.Abilities.DangerSense))
            dangerSenseAbilityLevel = AbilityManager.Instance.abilityLevels[ability];
    }

    private void ResetAbilityLevel()
    {
        dangerSenseAbilityLevel = 0;
    }


    private void RadiusCheck()
    {
        if (dangerSenseAbilityLevel == 0)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position, sensorRadius, sensorMask);

        foreach (Collider2D hit in hits)
        {
            Vector3 screenPoint = mainCam.WorldToViewportPoint(hit.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                if (onScreen)
                {
                    Debug.Log("Danger inside Screen");
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && sensorSr.color.a >= maxAlpha)
                    {
                        Debug.Log("FadeOUT");
                        anim.Play("FadeOut");
                    }
                }
                else if (!onScreen && sensorSr.color.a == 0)
                {
                    Debug.Log("Danger outside the screen");
                    {
                        Debug.Log("FadeIN");
                        anim.Play("FadeIn");
                    }
                }
            }
        }

        if (hits != null && hits.Length != 0)
        {
            lastHitPos = hits[0].transform.position;
        }
        if (lastHitPos != null)
        {
            RotateDangerSensorIndictor(lastHitPos);
        }
    }

    private void RotateDangerSensorIndictor(Vector3 hitPos)
    {
        Vector3 direction = (hitPos - transform.position).normalized;
        dangerSensor.up = Vector3.SmoothDamp(dangerSensor.up, direction, ref velocity, smoothTime);
        dangerSensor.up = direction;
    }
}
