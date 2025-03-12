using UnityEngine;
using UnityEngine.U2D;

public class RandomShape : MonoBehaviour
{
    public float maxLength = 200f;
    public float expansionAmount = 50f;

    private SpriteShapeController shapeController;
    private Spline spline;
    private EdgeCollider2D ec;

    private void Start()
    {
        shapeController = GetComponent<SpriteShapeController>();
        ec = shapeController.GetComponent<EdgeCollider2D>();
        spline = shapeController.spline;

        ModifyShape();

        DirectorScript.Instance.UpdateConfiner(ec);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ModifyShape();
            DirectorScript.Instance.UpdateConfiner(ec);
        }
    }

    private void ModifyShape()
    {
        // Calculate the scale factor based on the desired maximum length
        float scaleFactor = maxLength / (spline.GetPosition(0).magnitude * 2f);

        // Iterate through the control points and modify their positions with a random offset
        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            Vector3 point = spline.GetPosition(i);
            point *= scaleFactor;

            // Generate a random offset within a range that increases linearly with the overall length
            float offsetRange = maxLength * 0.1f + maxLength * 0.1f * (point.magnitude / maxLength);
            float offsetX = Random.Range(-offsetRange, offsetRange);
            float offsetY = Random.Range(-offsetRange, offsetRange);

            point += new Vector3(offsetX, offsetY, 0f);

            spline.SetPosition(i, point);
        }

        // Update the shape's geometry
        shapeController.BakeCollider();
        shapeController.BakeMesh();
    }

    //private void ExpandShape()
    //{
    //    // Calculate the new maximum length
    //    maxLength += expansionAmount;

    //    // Iterate through the control points and modify their positions with an expansion offset
    //    for (int i = 0; i < spline.GetPointCount(); i++)
    //    {
    //        Vector3 point = spline.GetPosition(i);

    //        // Calculate the distance from the center
    //        float distance = point.magnitude;

    //        // Calculate the expansion factor based on the distance from the center
    //        float expansionFactor = Mathf.Lerp(1f, maxLength / distance, expansionAmount / maxLength);

    //        // Apply the expansion factor to the point position
    //        point *= expansionFactor;

    //        // Generate a random offset within a range that increases linearly with the overall length
    //        float offsetRange = maxLength * 0.1f + maxLength * 0.1f * (distance / maxLength);
    //        float offsetX = Random.Range(-offsetRange, offsetRange);
    //        float offsetY = Random.Range(-offsetRange, offsetRange);

    //        point += new Vector3(offsetX, offsetY, 0f);

    //        // Update the control point position
    //        spline.SetPosition(i, point);
    //    }

    //    // Update the shape's geometry
    //    shapeController.BakeCollider();
    //    shapeController.BakeMesh();
    //}
}
