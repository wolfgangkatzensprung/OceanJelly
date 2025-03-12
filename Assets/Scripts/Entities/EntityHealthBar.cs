using UnityEngine;
using UnityEngine.UI;

public class EntityHealthBar : MonoBehaviour
{
    Image image;
    EntityHealth eh;

    private void Start()
    {
        eh = gameObject.GetComponentInParent<EntityHealth>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = eh.GetHealth() / eh.maxHealth;
    }
}
