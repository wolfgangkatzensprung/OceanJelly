using UnityEngine;

public class LightAreas : Singleton<LightAreas>
{
    [System.Serializable]
    public struct AreaLight
    {
        public string areaName;
        public float intensity;
        public Color color;
    }
    public AreaLight[] areaLights = new AreaLight[1];

}
