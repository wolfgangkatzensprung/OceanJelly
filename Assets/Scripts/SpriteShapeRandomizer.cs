using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeRandomizer : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;
    public Sprite[] sprites;

    public int minSides = 3;
    public int maxSides = 8;
    public float minRadius = 0.5f;
    public float maxRadius = 2f;

    private void Start()
    {
        GenerateSpriteShape();
    }

    public void GenerateSpriteShape()
    {
        int sides = Random.Range(minSides, maxSides + 1);
        float radius = Random.Range(minRadius, maxRadius);

        //SpriteShape spriteShape = new SpriteShape();
        //spriteShapeController.spline.SetSprite(sprites[Random.Range(0, sprites.Length)]);

        Vector2 center = Vector2.zero;
        float angleStep = 360f / sides;

        for (int i = 0; i < sides; i++)
        {
            float angle = i * angleStep;
            Vector2 point = center + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            spriteShapeController.spline.InsertPointAt(i, point);
        }

        spriteShapeController.spline.SetPosition(0, spriteShapeController.spline.GetPosition(sides));
        spriteShapeController.spline.RemovePointAt(sides);

        //spriteShapeController.spriteShape = spriteShape;
        spriteShapeController.BakeCollider();
    }
}
