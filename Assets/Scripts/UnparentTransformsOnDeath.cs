using UnityEngine;

[RequireComponent(typeof(EntityHealth))]
public class UnparentTransformsOnDeath : MonoBehaviour
{
    public EntityHealth eh;

    public Transform[] childrenToUnparent;

    [Tooltip("Puts selfDestructionTimer on unparented Objects")]
    public bool setSelfDestructionTimer = true;

    private void Start()
    {
        eh.onDeath += OnDeath;
    }
    void OnDeath()
    {
        eh.onDeath -= OnDeath;

        Debug.Log($"OnDeath called. Unparenting {childrenToUnparent.Length} children");

        for (int i = 0; i < childrenToUnparent.Length; i++)
        {
            childrenToUnparent[i].gameObject.AddComponent<SelfDestructionTimer>();
            childrenToUnparent[i]?.SetParent(null);
            Debug.Log($"Unparented {childrenToUnparent[i]}");
        }
    }
}
