using UnityEngine;
using UnityEngine.UI;

public class SetRandomImageOnEnable : MonoBehaviour
{
    public Sprite[] img;

    private void OnEnable()
    {
        GetComponent<Image>().sprite = img[Random.Range(0, img.Length)];
    }
}
