using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxMovement : MonoBehaviour
{
    ParallaxCamera parallaxCamera;
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    public Transform[] layers = new Transform[6];
    public float[] parallaxFactors = { .06f, .045f, .03f, -.02f, -.04f, -.08f };

    public void Start()
    {
        if (!(SceneManager.GetActiveScene().name == "Master"))
        {
            if (parallaxCamera == null)

                parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
        }

        if (parallaxCamera != null)
        {
            parallaxCamera.onCameraTranslate += Move;
            if (parallaxLayers != null)
            {
                InitializeLayers();
            }
        }

    }
    void InitializeLayers()
    {
        //Debug.Log("InitializeLayers() for Parallax");
        parallaxLayers.Clear();
        for (int i = 0; i < layers.Length; i++)
        {
            ParallaxLayer layer = layers[i].GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.SetParallaxFactor(parallaxFactors[i]);
                parallaxLayers.Add(layer);
            }
        }
    }

    void Move(float delta)
    {
        Debug.Log("Move ParallaxLayer");
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            if (layer != null)
            {
                layer.Move(delta);
                //Debug.Log($"Move ParallaxLayer by {delta}");
            }
        }
    }

    public Transform[] GetLayers()
    {
        return layers;
    }
}