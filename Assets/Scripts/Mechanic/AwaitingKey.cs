using UnityEngine;

public class AwaitingKey : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (ItemCollector.Instance.GetIsHoldingItem() && ItemCollector.Instance.GetHeldItem().name.Contains("Key"))
            {
                anim.SetTrigger("openGate");
            }
        }
        else if (collision.gameObject.CompareTag("Item"))
        {
            if (collision.gameObject.TryGetComponent(out Item item))
            {
                if (item.GetItemType().Equals(Item.Type.Key))
                {
                    anim.SetTrigger("openGate");
                }
            }
        }
    }
}
