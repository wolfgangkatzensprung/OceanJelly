using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

class SceneLoaderWindow : EditorWindow
{
    static EditorWindow editorWindow, deletePopUp;
    static string[] filesInScenesFolder;
    static Texture2D titleTexture;
    static FileInfo[] fileInfo;
    static Vector2 scrollPosition;
    static string fileToShit;

    [MenuItem("Window/SceneLoader")]
    public static void ShowWindow()
    {
        editorWindow = EditorWindow.GetWindow(typeof(SceneLoaderWindow));
        editorWindow.titleContent = new GUIContent("Scene Loader");

        CheckExistingScenes();

        editorWindow.minSize = new Vector2(100f, 240f);
        editorWindow.Focus();
    }

    void OnGUI()
    {
        GUILayout.Space(2f);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(titleTexture);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (titleTexture != null || editorWindow != null)
        {
            EditorGUI.DrawRect(new Rect(0, 100f, editorWindow.position.width, 2f), Color.white);
        }
        else
        {
            ShowWindow();
        }

        GUILayout.Space(6f);


        if (GUILayout.Button("Refresh Scenes"))
        {
            CheckExistingScenes();
        }

        GUILayout.Space(12f);

        GUILayout.Label("Scenes in project:");

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
        for (int i = 0; i < fileInfo.GetLength(0); i++)
        {
            GUILayout.BeginHorizontal();
            //string fileName = fileInfo[i].Split(new string[] {"/", "."}, System.StringSplitOptions.RemoveEmptyEntries)[2].ToString();
            string fileName = fileInfo[i].Name.Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries)[0].ToString();

            if (GUILayout.Button(fileName, GUILayout.Height(21)))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    EditorSceneManager.OpenScene(fileInfo[i].FullName, OpenSceneMode.Additive);
            }

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

    }

    static void CheckExistingScenes()
    {
        fileInfo = (new DirectoryInfo(Application.dataPath)).GetFiles("*.unity", SearchOption.AllDirectories);
    }
}