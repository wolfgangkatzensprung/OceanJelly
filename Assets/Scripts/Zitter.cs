using System.Collections;
using UnityEngine;

public class Zitter : MonoBehaviour
{
    public float zitterStrength = 1f;
    Vector2 zitterNoise;

    private void Start()
    {
        StartZitter();
    }

    public void StartZitter()
    {
        StartCoroutine(ZitterObject());
    }

    IEnumerator ZitterObject()
    {
        while (true)
        {
            zitterNoise = new Vector2(Mathf.PerlinNoise(PlayerManager.Instance.playerPosition.x, PlayerManager.Instance.playerPosition.y), 0);
            Vector3 rnd = new Vector3(Random.Range(Mathf.Min(-zitterNoise.x, -zitterNoise.y), Mathf.Max(zitterNoise.x, zitterNoise.y)), 0, 0);
            transform.position += rnd * zitterStrength;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
