using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    Image image;
    PlayerHealth ph;

    private void Start()
    {
        ph = PlayerManager.Instance.player.GetComponent<PlayerHealth>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = (float)ph.hp / (float)ph.maxHp;
    }
}