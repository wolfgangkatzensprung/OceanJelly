using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class NautiWindow : EditorWindow
{
    string[] scenesToLoad;
    int index;

    Scene[] offeneScenes;

    [MenuItem("Window/NautiWindow")]
    public static void ShowWindow()
    {
        GetWindow<NautiWindow>("NautiWindow");
    }
    private void OnGUI()
    {
        GUILayout.Label("Das Nauti Window", EditorStyles.boldLabel);
        try
        {
            if (!Application.isPlaying)
            {
                offeneScenes = new Scene[SceneManager.sceneCount];
                scenesToLoad = new string[SceneManager.sceneCount];
                for (int i = 0; i < offeneScenes.Length; i++)
                {
                    offeneScenes[i] = SceneManager.GetSceneAt(i);
                    scenesToLoad[i] = offeneScenes[i].name;
                }

                // Popup Menu das alle Scenen anzeigt. Dort kann man die zu ladende Scene auswaehlen
                index = EditorGUILayout.Popup(index = EditorPrefs.GetInt("SceneIndex"), scenesToLoad);
                if (index != EditorPrefs.GetInt("SceneIndex"))
                {
                    EditorPrefs.SetInt("SceneIndex", index);
                }
                if (index <= scenesToLoad.Length)
                {
                    PlayerPrefs.SetString("currentScene", scenesToLoad[index]);
                }
            }

            // Button der Master Scene aktiv setzt und dann Play startet
            GUI.backgroundColor = new Color(0, .9f, 0f, .42f);
            if (GUILayout.Button("Start Game"))
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Master"));
                EditorApplication.isPlaying = true;
            }

            GUILayout.Space(8f);
            GUI.backgroundColor = new Color(0, .7f, .7f, .5f);

            // Button der Player in die Mitte des Scene View verschiebt
            if (GUILayout.Button("Move Player"))
            {
                Transform playerTrans = GameObject.FindGameObjectsWithTag("Player")[0].transform;
                SceneView.lastActiveSceneView.MoveToView(playerTrans);
                playerTrans.position = new Vector3(playerTrans.position.x, playerTrans.position.y, 0f);
            }

            GUI.backgroundColor = new Color(0, .7f, .7f, .3f);
            // Button der Player selektiert
            if (GUILayout.Button("Select Player"))
            {
                Transform playerTrans = GameObject.FindGameObjectsWithTag("Player")[0].transform;
                Selection.activeGameObject = playerTrans.gameObject;
            }

            GUILayout.Space(8f);

            GUI.backgroundColor = new Color(0, .7f, .7f, .5f);

            if (GUILayout.Button("Move Object Position"))
            {
                Transform trans = Selection.activeGameObject.transform;
                SceneView.lastActiveSceneView.MoveToView(trans);
                trans.position = new Vector3(trans.position.x, trans.position.y, 0f);
            }

            GUI.backgroundColor = new Color(.1f, .7f, .3f, .4f);
            // Button der das selektierte GameObject in die im NautiWindow selektierte Szene verschiebt
            if (GUILayout.Button("Set Object Scene"))
            {
                Scene sceneToMoveTo = SceneManager.GetSceneAt(EditorPrefs.GetInt("SceneIndex"));
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (go.transform.parent == null)
                        SceneManager.MoveGameObjectToScene(go, sceneToMoveTo);
                    else
                    {
                        go.transform.parent = null;
                        SceneManager.MoveGameObjectToScene(go, sceneToMoveTo);
                    }
                }
            }

            GUILayout.Space(8f);

            GUI.backgroundColor = new Color(.3f, .6f, .2f, .6f);
            //Button der selektierte Objekte in die ParallaxLayers ihrer jeweiligen Scene verschiebt
            if (GUILayout.Button("AutoSort Parallax"))
            {
                GameObject[] allParallaxObjects = GameObject.FindGameObjectsWithTag("ParallaxLayer");
                List<GameObject> parallaxArray = new List<GameObject>();
                for (int i = 0; i < allParallaxObjects.Length; i++)
                {
                    if (allParallaxObjects[i].CompareTag("ParallaxLayer"))
                    {
                        parallaxArray.Add(allParallaxObjects[i]);
                    }
                }

                foreach (GameObject selectedObject in Selection.gameObjects)
                {
                    if (selectedObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
                    {
                        foreach (GameObject parallaxLayer in parallaxArray)
                        {
                            if (parallaxLayer.scene.Equals(selectedObject.scene) && parallaxLayer.name.Equals("+++" + sr.sortingLayerName + "+++"))
                            {
                                Debug.Log($"AUTO SORTED {selectedObject.transform.name}.");
                                selectedObject.transform.SetParent(parallaxLayer.transform);
                                continue;
                            }
                        }
                    }
                    else if (selectedObject.TryGetComponent<SpriteShapeRenderer>(out SpriteShapeRenderer ssr))
                    {
                        foreach (GameObject parallaxLayer in parallaxArray)
                        {
                            if (parallaxLayer.scene.Equals(selectedObject.scene) && parallaxLayer.name.Equals("+++" + ssr.sortingLayerName + "+++"))
                            {
                                Debug.Log($"AUTO SORTED {selectedObject.transform.name}.");
                                selectedObject.transform.SetParent(parallaxLayer.transform);
                                continue;
                            }
                        }
                    }
                }
            }

        }
        catch
        {
            Debug.Log("NautiWindow Error");
        }
    }
}