using System;
using TMPro;
using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    public Animator anim;
    public AbilityManager.Abilities ability;
    public TextMeshProUGUI abilityNameText;

    private void OnEnable()
    {
        Debug.Log("AbilityButton OnEnable");
        ability = AbilityManager.Instance.GetRandomAbility();
        string abilityName = ability.ToString();
        for (int i = 1; i < abilityName.Length; i++)
        {
            if (Char.IsUpper(abilityName[i]))
            {
                abilityName = $"{abilityName.Substring(0, i)} \n {abilityName.Substring(i)}";
                break;
            }
        }
        abilityNameText.text = abilityName;
        anim.Play("Popup");
    }

    public void ChooseAbility()
    {
        AbilityManager.Instance.IncreaseAbilityLevel(ability);
        UIManager.Instance.levelUpPanel.SetActive(false);
    }
}