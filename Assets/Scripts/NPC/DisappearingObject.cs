using UnityEngine;

public class DisappearingObject : MonoBehaviour
{
    SpriteRenderer sr;

    public float disappearTime = 2f;
    public string catchphrase;

    bool isDisappearing;
    float disappearTimer;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        disappearTimer = disappearTime;
    }

    private void Update()
    {
        if (DialogManager.Instance.GetDialogText() == catchphrase)
        {
            isDisappearing = true;
        }
        if (isDisappearing)
        {
            disappearTimer -= Time.deltaTime;

            sr.color = new Vector4(1, 1, 1, disappearTimer / disappearTime);

            if (disappearTimer < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
