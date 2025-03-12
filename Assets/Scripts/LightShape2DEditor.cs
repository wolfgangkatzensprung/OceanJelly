//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Rendering.Universal;

//[CustomEditor(typeof(Light2D))]
//public class Light2DShapeEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        Light2D light2D = (Light2D)target;

//        if (GUILayout.Button("Set shape to Polygon Collider 2D"))
//        {
//            var polygonCollider2D = light2D.GetComponent<PolygonCollider2D>();

//            if (polygonCollider2D != null)
//            {
//                Vector3[] points = new Vector3[polygonCollider2D.GetTotalPointCount()];
//                for (int i = 0; i < polygonCollider2D.GetTotalPointCount(); i++)
//                {
//                    points[i] = polygonCollider2D.points[i];
//                }

//                light2D.SetShapePath(points);
//            }
//        }
//    }
//}