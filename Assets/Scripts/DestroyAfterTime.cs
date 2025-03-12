﻿using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [Tooltip("Maximale Lebenszeit in MINUTEN")]
    public float maxLifeTime = 15f;
    float lifeTimer = 0;
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > maxLifeTime * 60f)
        {
            Destroy(gameObject);
        }
    }
}
