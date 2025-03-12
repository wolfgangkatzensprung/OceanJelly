using Cinemachine;
using UnityEngine;


public class PlateauKrakCaveCam : MonoBehaviour
{
    CinemachineVirtualCamera vcam;

    public GameObject globalLightPlateau;
    //UnityEngine.Rendering.Universal.Light2D lightComponent;

    float startLightIntensity;
    public float dimmedLightIntensity = .1f;
    public float lightTransitionSpeed = 1;

    //private void Start()
    //{
    //    vcam = GetComponent<CinemachineVirtualCamera>();
    //    lightComponent = LightManager.Instance.GetGlobalLight();
    //    startLightIntensity = lightComponent.intensity;
    //}

    //private void Update()
    //{

    //    if (vcam.m_Priority < 20)
    //    {
    //        if (lightComponent.intensity >= (startLightIntensity - Time.deltaTime))
    //        {
    //            return;
    //        }
    //        Debug.Log("Lerping from " + dimmedLightIntensity + " to " + lightComponent.intensity);
    //        lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, startLightIntensity, lightTransitionSpeed * Time.deltaTime);

    //    }
    //    else if (vcam.m_Priority > 0)
    //    {
    //        if (lightComponent.intensity <= (dimmedLightIntensity + Time.deltaTime))
    //        {
    //            return;
    //        }
    //        Debug.Log("Lerping from " + lightComponent.intensity + " to " + dimmedLightIntensity);
    //        lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, dimmedLightIntensity, lightTransitionSpeed * Time.deltaTime);
    //    }
    //}
}
