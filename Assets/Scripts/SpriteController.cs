using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteController : MonoBehaviour
{
    [Header("References")]
    Transform playerTrans;
    public ParallaxMovement pm;

    [Header("Settings")]
    public float renderDistance = 1000f;
    [Tooltip("Update Rate in Sekunden")]
    public float updateRate = 2f;

    Transform[] layers;
    bool rdyForParallaxUpdate = true;

    Dictionary<Transform, Transform> droppedSprites = new Dictionary<Transform, Transform>();

    private void Start()
    {
        layers = pm.GetLayers();
        playerTrans = PlayerManager.Instance.GetPlayerTransform();

        if (Application.isEditor)
            return;

        InstantiateLayerSprites();
    }

    internal void InstantiateLayerSprites()
    {
        foreach (Transform layerTrans in layers)
        {
            foreach (Transform spriteTrans in layerTrans)
            {
                if (spriteTrans.TryGetComponent(out SpriteRenderer sr) || spriteTrans.TryGetComponent(out SpriteShapeRenderer ssr))
                {
                    if (!spriteTrans.TryGetComponent(out ParallaxSprite ps))
                        spriteTrans.gameObject.AddComponent<ParallaxSprite>();
                }
            }
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (rdyForParallaxUpdate)
            StartCoroutine(UpdateParallax());
    }

    IEnumerator UpdateParallax()
    {
        rdyForParallaxUpdate = false;
        yield return new WaitForSeconds(updateRate);
        //Debug.Log("Updating Sprites on Parallax");
        UpdateSpritesOnParallax();
        UpdateDroppedSprites();
        rdyForParallaxUpdate = true;
    }

    private void UpdateSpritesOnParallax()
    {
        foreach (Transform layerTrans in layers)
        {
            foreach (Transform spriteTrans in layerTrans)
            {
                if (spriteTrans.TryGetComponent(out SpriteRenderer sr))
                {
                    HandleFarSprites(spriteTrans, sr, layerTrans);
                }
                else if (spriteTrans.TryGetComponent(out SpriteShapeRenderer ssr))
                {
                    HandleFarSpriteShapes(spriteTrans, ssr, layerTrans);
                }
            }
        }
    }

    void UpdateDroppedSprites()
    {
        List<Transform> droppedSpriteTransforms = droppedSprites.Keys.ToList();

        foreach (Transform spriteTrans in droppedSpriteTransforms)
        {
            if (spriteTrans.TryGetComponent(out SpriteRenderer sr))
            {
                if (droppedSprites.TryGetValue(spriteTrans, out Transform layerTrans))
                    HandleCloseSprites(spriteTrans, sr, layerTrans);
            }
            else if (spriteTrans.TryGetComponent(out SpriteShapeRenderer ssr))
            {
                if (droppedSprites.TryGetValue(spriteTrans, out Transform layerTrans))
                    HandleCloseSpriteShapes(spriteTrans, ssr, layerTrans);
            }
        }
    }

    private void HandleCloseSprites(Transform spriteTrans, SpriteRenderer sr, Transform layerTrans)
    {
        if (Vector2.Distance(spriteTrans.position, playerTrans.position) < renderDistance)
        {
            droppedSprites.Remove(spriteTrans);
            spriteTrans.SetParent(layerTrans);
            sr.enabled = true;
            //Debug.Log(spriteTrans.name + " wieder Child von " + layerTrans.name);
        }
    }
    private void HandleFarSprites(Transform spriteTrans, SpriteRenderer sr, Transform layerTrans)
    {
        if (Vector2.Distance(spriteTrans.position, playerTrans.position) > renderDistance)
        {
            sr.enabled = false;
            droppedSprites.Add(spriteTrans, layerTrans);
            spriteTrans.SetParent(null);
            //Debug.Log(spriteTrans.name + " nicht mehr Child von " + layerTrans.name);
        }
    }
    private void HandleCloseSpriteShapes(Transform spriteTrans, SpriteShapeRenderer ssr, Transform layerTrans)
    {
        if (Vector2.Distance(spriteTrans.position, playerTrans.position) < renderDistance)
        {
            droppedSprites.Remove(spriteTrans);
            spriteTrans.SetParent(layerTrans);
            ssr.enabled = true;
            //Debug.Log(spriteTrans.name + " wieder Child von " + layerTrans.name);
        }
    }
    private void HandleFarSpriteShapes(Transform spriteTrans, SpriteShapeRenderer ssr, Transform layerTrans)
    {
        if (Vector2.Distance(spriteTrans.position, playerTrans.position) > renderDistance)
        {
            ssr.enabled = false;
            droppedSprites.Add(spriteTrans, layerTrans);
            spriteTrans.SetParent(null);
            //Debug.Log(spriteTrans.name + " nicht mehr Child von " + layerTrans.name);
        }
    }

#endif

}




