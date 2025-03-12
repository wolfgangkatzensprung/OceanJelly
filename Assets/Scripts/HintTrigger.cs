using TMPro;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    SpriteRenderer sr;

    public Transform hintPosition;
    public TextMeshProUGUI buttonNameText;

    [Header("Settings")]
    public bool isItemHint = true;
    public bool isAttackHint = false;

    private void Start()
    {
        sr = hintPosition.GetComponent<SpriteRenderer>();
        DisableButtonHint();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (isAttackHint)
        {
            EnableButtonHint("X");
        }
        else if (isItemHint && !ItemCollector.Instance.GetIsHoldingItem())
        {
            EnableButtonHint("Y");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            DisableButtonHint();
    }

    private void EnableButtonHint(string buttonName)
    {
        buttonNameText.text = buttonName;
        sr.enabled = true;
    }
    private void DisableButtonHint()
    {
        buttonNameText.text = null;
        sr.enabled = false;
    }
}
