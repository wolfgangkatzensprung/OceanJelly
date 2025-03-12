using System.Collections.Generic;
using UnityEngine;

public class OceanLights : MonoBehaviour
{
    private Transform playerTrans;

    [SerializeField] private Color startColor;
    [SerializeField] private List<BiomeData> biomes;
    private int currentBiomeIndex = 0;

    private void Start()
    {
        playerTrans = PlayerManager.Instance.playerTrans;
    }

    private void Update()
    {
        UpdateCurrentBiome();
        UpdateOceanLight();
    }

    private void UpdateCurrentBiome()
    {
        currentBiomeIndex = 0;
        for (int i = 0; i < biomes.Count; i++)
        {
            if (biomes[i].IsInside(playerTrans.position))
            {
                //Debug.Log($"Player is inside {biomes[i].name} biome.");
                currentBiomeIndex = i;
                break;
            }
        }
    }


    private void UpdateOceanLight()
    {
        BiomeData currentBiome = biomes[currentBiomeIndex];
        Color currentColor = LightManager.Instance.globalLight.color;
        Color targetColor = currentBiome.color;

        if (currentBiome.IsInDeadzone(playerTrans.position))
        {
            // Player is in the deadzone, color is at its maximum
            LightManager.Instance.SetGlobalLightColor(targetColor);
        }
        else
        {
            // Player is outside the deadzone
            float distanceFromEdge = Vector2.Distance(playerTrans.position, currentBiome.center) - currentBiome.radius;
            float lerpValue = 1f;

            if (distanceFromEdge > 0f)
            {
                lerpValue = Mathf.Clamp01(1f - (distanceFromEdge / (currentBiome.deadzoneRadius * 2f)));
            }

            //Debug.Log($"OceanLights Color lerpValue = {lerpValue}");

            Color lerpedColor = Color.Lerp(startColor, targetColor, lerpValue);
            LightManager.Instance.SetGlobalLightColor(lerpedColor);


        }
    }
}

[System.Serializable]
public class BiomeData
{
    public string name;
    public Color color;
    public Vector2 center;
    public float radius;
    public float deadzoneRadius;

    public bool IsInside(Vector2 position)
    {
        return Vector2.Distance(position, center) < radius;
    }

    public bool IsInDeadzone(Vector2 position)
    {
        return Vector2.Distance(position, center) < deadzoneRadius;
    }
}
