using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager _instance;
    public static AbilityManager Instance { get { return _instance; } }

    int extraJumps;
    float hitBonus;

    // Shield Ability
    Collider2D shieldCol;
    bool shieldRdy;
    public float shieldTime = 1f;   // spaeter durch playerpref ersetzen
    bool shieldOnCd;
    public float shieldCooldown = 3f;   // beinhaltet aktive zeit

    // Recall Ability
    Coroutine recallEffect;
    bool recallRdy;
    bool recalling;
    public float recallWarmupTime = 3f;
    public int healFactor = 1;

    public enum Abilities
    {
        Speed,
        HopeMagnet,
        TreasureHunter,
        DangerSense,
        Health,
        MagicField,
        /*, Anchor, TentacleBiceps, VenemousTentacles, InkCloud, PoisonShot,*/
        /*ElectricOrb, Shapeshifter, Invisibility, MedusasGaze, SonicPulse*/
    }
    private Queue<Abilities> abilitiesQueue = new Queue<Abilities>();    // for GetRandomAbility() 
    public Dictionary<Abilities, int> abilityLevels = new Dictionary<Abilities, int>();
    internal Dictionary<Abilities, int> maxAbilityLevels = new Dictionary<Abilities, int>()
    {
        { Abilities.Speed, 99 },
        { Abilities.HopeMagnet, 99 },
        { Abilities.TreasureHunter, 99 },
        { Abilities.DangerSense, 1 },
        { Abilities.Health, 50 },
        { Abilities.MagicField, 10 }
    };

    public delegate void AbilityIncreaseDelegate(Abilities ability);
    public AbilityIncreaseDelegate onAbilityIncrease;

    public void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        PlayerHealth.Instance.onDeath += ResetAbilities;
    }

    private void Update()
    {
        //if (shieldRdy)
        //    if (Input.GetButtonDown("Shoot"))
        //        StartCoroutine(ShieldEffect());
        //if (recallRdy)
        //    if (Input.GetButtonDown("Shoot"))
        //        if (!recalling)
        //            recallEffect = StartCoroutine(RecallEffect());
    }


    internal Abilities GetRandomAbility()
    {
        if (abilitiesQueue.Count == 0)
        {
            Type type = typeof(Abilities);
            Array values = type.GetEnumValues();
            var filteredValues = values.OfType<Abilities>().Where(a => GetAbilityLevel(a) < GetMaxAbilityLevel(a)).ToList();

            // Shuffle the abilities using the Fisher-Yates shuffle algorithm
            int n = filteredValues.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                var temp = filteredValues[k];
                filteredValues[k] = filteredValues[n];
                filteredValues[n] = temp;
            }

            foreach (var value in filteredValues)
            {
                abilitiesQueue.Enqueue(value);
            }
        }
        if (GetAbilityLevel(abilitiesQueue.Peek()) < GetMaxAbilityLevel(abilitiesQueue.Peek()))
        {
            return abilitiesQueue.Dequeue();
        }
        else
        {
            abilitiesQueue.Dequeue();
            return GetRandomAbility();
        }
    }

    //internal Abilities GetRandomAbility()
    //{
    //    if (abilitiesQueue.Count == 0)
    //    {
    //        Type type = typeof(Abilities);
    //        Array values = type.GetEnumValues();
    //        var filteredValues = values.OfType<Abilities>().Where(a => GetAbilityLevel(a) < GetMaxAbilityLevel(a));
    //        foreach (var value in filteredValues)
    //        {
    //            abilitiesQueue.Enqueue((Abilities)value);
    //        }
    //    }
    //    if (GetAbilityLevel(abilitiesQueue.Peek()) < GetMaxAbilityLevel(abilitiesQueue.Peek()))
    //    {
    //        return abilitiesQueue.Dequeue();
    //    }
    //    else
    //    {
    //        abilitiesQueue.Dequeue();
    //        return GetRandomAbility();
    //    }
    //}

    private int GetAbilityLevel(Abilities a)
    {
        if (abilityLevels.ContainsKey(a))
        {
            return abilityLevels[a];
        }
        else
        {
            return 0;
        }
    }
    private int GetMaxAbilityLevel(Abilities a)
    {
        return maxAbilityLevels[a];
    }

    public void IncreaseAbilityLevel(AbilityManager.Abilities ability)
    {
        if (abilityLevels.ContainsKey(ability))
        {
            abilityLevels[ability] += 1;
        }
        else
        {
            abilityLevels.Add(ability, 1);
        }

        SetAbilityPower(ability, abilityLevels[ability]);   // Set the Ability values for this specific ability

        onAbilityIncrease?.Invoke(ability);
    }

    private void SetAbilityPower(Abilities ability, int abilityLevel)
    {
        switch (ability)
        {
            case Abilities.HopeMagnet:
                ItemCollector.Instance.SetMagnetStrength(abilityLevel);
                break;
        }
    }

    // Shield Item
    public void ActivateShieldItem(Collider2D shieldCollider)
    {
        shieldRdy = true;
        shieldCol = shieldCollider;
    }
    public void DisableShieldItem(Collider2D shieldCollider)
    {
        shieldRdy = false;
    }
    IEnumerator ShieldEffect()
    {
        if (!shieldOnCd)
        {
            StartCoroutine(ShieldCooldown());
            // Shield Visualisierung
            shieldCol.enabled = true;
            yield return new WaitForSeconds(shieldTime);
            shieldCol.enabled = false;
            // deaktiviere Shield Visualisierung
        }
    }
    IEnumerator ShieldCooldown()
    {
        shieldOnCd = true;
        yield return new WaitForSeconds(shieldCooldown);
        shieldOnCd = false;
    }

    // Recall Item

    public void ActivateRecallItem()
    {
        recallRdy = true;
    }
    public void DisableRecallItem()
    {
        Debug.Log("DisableRecallItem()");
        recallRdy = false;
        if (recallEffect != null)
            StopCoroutine(recallEffect);
        recalling = false;
    }
    public bool IsRecalling()
    {
        return recalling;
    }
    IEnumerator RecallEffect()
    {
        Debug.Log("RecallEffect() coroutine");
        recalling = true;
        yield return new WaitForSeconds(recallWarmupTime);
        SceneManagerScript.Instance.LoadCheckpoint();
        recalling = false;
    }

    public void ResetAbilities()
    {
        abilityLevels.Clear();
    }
}
