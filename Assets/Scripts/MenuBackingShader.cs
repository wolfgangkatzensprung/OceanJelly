using UnityEngine;

public class MenuBackingShader : MonoBehaviour
{
    Material mat;
    private void Start()
    {
        mat = GetComponent<Renderer>().material;

    }
    private void Update()
    {
        mat.SetFloat("UnscaledTime", Time.unscaledTime);
        mat.SetFloat("UnscaledDeltaTime", Time.unscaledDeltaTime);
    }
}
