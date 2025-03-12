using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LayerEditorWindow : EditorWindow
{
    int selGridInt = 0;
    string[] selStrings = { "GanzWeitHinten", "FarBackground", "Background", "Foreground", "NearForeground", "GanzVorne" };

    [MenuItem("Window/LayerEditorWindow")]
    public static void ShowWindow()
    {
        LayerEditorWindow window = (LayerEditorWindow)EditorWindow.GetWindow(typeof(LayerEditorWindow), true, "LayerEditorWindow");
        window.Show();
    }

    void OnGUI()
    {
        GameObject selection = Selection.activeGameObject;

        GUILayout.Label("LayerEditorWindow", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.white;
        GUILayout.BeginVertical("Box");
        selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 1);

        GUI.backgroundColor = new Color(0, .7f, .3f, .3f);
        if (GUILayout.Button("Set Layer by selection"))
        {
            string selectedLayer = selStrings[selGridInt];
            GameObject[] parallaxArray = GameObject.FindGameObjectsWithTag("ParallaxLayer");

            foreach (Transform selectedTrans in selection.GetComponentsInChildren<Transform>())
            {
                if (selectedTrans.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
                {
                    foreach (GameObject parallaxLayer in parallaxArray)
                    {
                        if (parallaxLayer.name.Contains(selectedLayer))
                        {
                            selectedTrans.SetParent(parallaxLayer.transform);
                            return;
                        }
                    }
                }
            }
        }
        GUILayout.EndVertical();

        GUILayout.Space(16f);

        if (!Application.isPlaying)
        {
            GUI.backgroundColor = new Color(0, 1f, 0f, .3f);
            if (GUILayout.Button("AutoSort to selected Scene"))   //Set Layer from SortingLayer into the selected Scene
            {
                Scene s = SceneManager.GetActiveScene();
                GameObject[] objectsInScene = s.GetRootGameObjects();
                List<GameObject> parallaxArray = new List<GameObject>();
                for (int i = 0; i < objectsInScene.Length; i++)
                {
                    if (objectsInScene[i].CompareTag("ParallaxLayer"))
                    {
                        parallaxArray.Add(objectsInScene[i]);
                        Debug.Log("ParallaxLayer added to List");
                    }
                }

                foreach (Transform selectedTrans in selection.GetComponentsInChildren<Transform>())
                {
                    if (selectedTrans.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
                    {
                        foreach (GameObject parallaxLayer in parallaxArray)
                        {
                            if (parallaxLayer.name.Equals("+++" + sr.sortingLayerName + "+++"))
                            {
                                selectedTrans.SetParent(parallaxLayer.transform);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
