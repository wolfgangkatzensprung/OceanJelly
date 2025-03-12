using UnityEngine;

public static class PerlinHelper
{
    public static float GetPerlin(Vector2 pos)
    {
        float pingPong = Mathf.PingPong(-1f, 1f);
        float perlin = Mathf.PerlinNoise(pos.x + pingPong, pos.y + pingPong);
        return perlin;
    }

    public static Vector2 GetPerlin2(Vector2 pos)
    {
        float pingPong = Mathf.PingPong(-1f, 1f);
        Vector2 perlin = new Vector2(Mathf.PerlinNoise(pos.x + pingPong, pos.y + pingPong), Mathf.PerlinNoise(pos.y + pingPong, pos.x + pingPong));
        return perlin;
    }
}
