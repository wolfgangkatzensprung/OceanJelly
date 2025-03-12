using UnityEngine;

public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;
    private float currentPositionX;

    void Start()
    {
        currentPositionX = transform.position.x;
    }
    void Update()
    {
        //if (!SceneManagerScript.Instance.loadingFinished)
        //    return;



        // wird aktuell noch nicht gebraucht, daher:
        return;



        Debug.Log("ParallaxCamera Update()");

        if (transform.position.x != currentPositionX)
        {
            float delta = currentPositionX - transform.position.x;
            onCameraTranslate?.Invoke(delta);
            currentPositionX = transform.position.x;
        }
    }
}