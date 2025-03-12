using UnityEngine;

/// <summary>
/// AoE Abilities on Player Position, like ElectricField
/// </summary>
public class Player_AoE : Singleton<Player_AoE>
{
    public GameObject magicField;
    public LayerMask enemiesMask;

    // Magic Field
    int magicFieldAbilityLevel = 1;
    const float radius = 29f;   // to match sprite
    internal float currentRadius = radius * .3f;
    float startRadius;
    bool magicFieldEnabled;

    private void Start()
    {
        startRadius = currentRadius;
        magicField.transform.localScale = new Vector3(.3f, .3f, 0f);

        AbilityManager.Instance.onAbilityIncrease += UpgradeAbilityLevel;
        PlayerHealth.Instance.onDeath += ResetAbilityLevel;
    }

    void Update()
    {
        if (magicFieldEnabled)
            RadiusCheck();
    }

    private void UpgradeAbilityLevel(AbilityManager.Abilities ability)
    {
        if (ability.Equals(AbilityManager.Abilities.MagicField))
        {
            Debug.Log("Upgrade MagicField");

            magicFieldAbilityLevel = AbilityManager.Instance.abilityLevels[ability];
            currentRadius = Mathf.Lerp(radius * .23f, radius * 1.8f, (float)magicFieldAbilityLevel / (float)AbilityManager.Instance.maxAbilityLevels[AbilityManager.Abilities.MagicField]);
            Vector3 currentScale = new Vector3(currentRadius / 29f, currentRadius / 29f, 1f);
            magicField.transform.localScale = currentScale;
            Debug.Log($"Magic Field upgraded. It is now Lvl {magicFieldAbilityLevel}. Current Radius is {currentRadius}. Current Scale is {currentScale.x} ??!! It has been lerped by t = {(float)magicFieldAbilityLevel / (float)AbilityManager.Instance.maxAbilityLevels[AbilityManager.Abilities.MagicField]}");
            magicFieldEnabled = true;
            magicField.SetActive(true);
        }
    }

    private void ResetAbilityLevel()
    {
        magicFieldAbilityLevel = 0;
        magicFieldEnabled = false;
        magicField.SetActive(false);
    }
    private void RadiusCheck()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position, currentRadius, enemiesMask);

        // Iterate through all magnetizable objects
        foreach (Collider2D hit in hits)
        {
            // Get the distance between the player and the object
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (hit.transform.CompareTag("Entity"))
            {
                if (hit.transform.TryGetComponent(out EntityHealth eh))
                {
                    if (hit.TryGetComponent(out SharkMovement sm))
                    {
                        if (!sm.chasing)
                            sm.TriggerChase();
                    }
                    eh.ApplyDamage(magicFieldAbilityLevel);
                }
            }
        }
    }
}
