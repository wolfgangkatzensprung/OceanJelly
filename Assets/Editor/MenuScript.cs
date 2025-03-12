using UnityEditor;
using UnityEngine;


public class MenuScript : MonoBehaviour
{
    [MenuItem("Move/SelectedGameObject")]
    static void MoveObjectToSceneView()
    {
        Transform trans;
        trans = Selection.activeGameObject.transform;
        SceneView.lastActiveSceneView.MoveToView(trans);
        trans.position = new Vector3(trans.position.x, trans.position.y, 0f);

    }
    [MenuItem("Move/Player")]
    static void MovePlayerToSceneView()
    {
        Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        SceneView.lastActiveSceneView.MoveToView(playerTrans);
        playerTrans.position = new Vector3(playerTrans.position.x, playerTrans.position.y, 0f);
    }
}
