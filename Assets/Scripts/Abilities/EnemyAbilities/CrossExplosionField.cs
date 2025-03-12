using UnityEngine;
using UnityEngine.Tilemaps;

public class CrossExplosionField : MonoBehaviour
{
    public LayerMask tilemapLayer;

    public float maxDistance = 1000f;

    public float lifetime = 3f;
    public int damage = 6;
    float timer;
    public float explosionRadius = 11f;
    public float pushStrength = 10f;

    const float tileSize = 16f;

    private float horizontalDistance = 0f;
    private float verticalDistance = 0f;

    private void Start()
    {
        CrossExplosion();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            CombatManager.Instance.DealDamageToPlayer(damage);
        }
        else if (collision.collider.TryGetComponent(out EntityHealth eh))
        {
            eh.ApplyDamage(damage);

        }
        if (collision.collider.TryGetComponent(out TilemapCollider2D tilemapCollider))
        {
        }
    }
    private void CrossExplosion()
    {
        maxDistance *= tileSize;

        // Perform raycasts in 4 directions
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, maxDistance, tilemapLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, maxDistance, tilemapLayer);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, maxDistance, tilemapLayer);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, maxDistance, tilemapLayer);

        // Destroy tiles within the blast radius
        DestroyTiles(hitLeft);
        DestroyTiles(hitRight);
        DestroyTiles(hitUp);
        DestroyTiles(hitDown);

        Debug.Log($"CrossExplosion Distances: Left {hitLeft.distance}, Right {hitRight.distance}, Up {hitUp.distance}, Down {hitDown.distance}");
    }

    private void DestroyTiles(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out TilemapCollider2D tc))
            {
                Tilemap tilemap = tc.GetComponent<Tilemap>();
                Vector3 hitPoint = hit.point;
                Vector3Int tilePosition = tilemap.WorldToCell(hitPoint);
                float distance = Vector3.Distance(transform.position, hitPoint);

                for (int x = -Mathf.CeilToInt(explosionRadius); x <= Mathf.CeilToInt(explosionRadius); x++)
                {
                    for (int y = -Mathf.CeilToInt(explosionRadius); y <= Mathf.CeilToInt(explosionRadius); y++)
                    {
                        Vector3Int currentTilePos = new Vector3Int(tilePosition.x + x, tilePosition.y + y, tilePosition.z);
                        Vector3 currentTileWorldPos = tilemap.CellToWorld(currentTilePos) + new Vector3(0.5f, 0.5f, 0f);

                        if (Vector3.Distance(currentTileWorldPos, hitPoint) <= explosionRadius)
                        {
                            tilemap.SetTile(currentTilePos, null);
                        }
                    }
                }
            }
        }
    }
}
