using System.Collections;
using UnityEngine;

/// <summary>
/// momentan unused // stand 220110
/// </summary>
public class ShaderManager : MonoBehaviour
{
    public static ShaderManager _instance;
    public static ShaderManager Instance { get { return _instance; } }

    public Material damagedMaterial;
    bool cooldownActive;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public void TrySetTempShader(GameObject go, string materialName, float time)
    {
        Debug.Log("TrySetShaderTemporary(" + materialName + ")");
        if (cooldownActive)
            return;

        switch (materialName)
        {
            case "Damaged":
                StartCoroutine(SetShaderTemporary(go, damagedMaterial, time));
                break;
        }
    }

    IEnumerator SetShaderTemporary(GameObject go, Material mat, float t)
    {
        Debug.Log("SetShaderTemporary()");
        cooldownActive = true;
        Material gameObjectMat = go.GetComponent<Renderer>().material;
        Material goMatCopy = gameObjectMat;
        gameObjectMat = new Material(damagedMaterial);
        yield return new WaitForSeconds(t);
        gameObjectMat = goMatCopy;
        cooldownActive = false;
    }
}