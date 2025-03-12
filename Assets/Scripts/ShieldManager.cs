using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : Singleton<ShieldManager>
{
    public List<Shield> shields = new List<Shield>();
    private void OnDisable()
    {
#if UNITY_EDITOR
        return;
#endif
        PlayerHealth.Instance.onDeath -= ClearShields;
    }

    private void Start()
    {
        PlayerHealth.Instance.onDeath += ClearShields;
    }

    public void AddShield(Shield shield)
    {
        shields.Add(shield);
        shield.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .69f);

        SetShieldsCircularOffsets();
    }

    private void SetShieldsCircularOffsets()
    {
        Shield[] shieldsArray = shields.ToArray();

        int i = 0;
        foreach (Shield shield in shields)
        {
            float increment = 1f / shields.Count;
            float offset = (i + 1) * increment;
            shield.circularOffset = offset;
            Debug.Log($"Shield {i} hat offset {offset}");
            i++;
        }
    }

    public void RemoveShield(Shield shield)
    {
        shields.Remove(shield);
    }

    private void ClearShields()
    {
        Shield[] shieldsArray = shields.ToArray();
        for (int i = shields.Count - 1; i >= 0; i--)
        {
            shieldsArray[i].gameObject.SetActive(false);
        }
        shields.Clear();
    }
}