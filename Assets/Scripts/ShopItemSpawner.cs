using UnityEngine;

public class ShopItemSpawner : MonoBehaviour
{
    public bool spawnItemsLeftSide = true;
    public GameObject[] repertoire = new GameObject[3];

    private void Start()
    {
        int spawnDirection = -1;
        float offset = 30f;

        if (!spawnItemsLeftSide)
        {
            spawnDirection *= 1;
        }

        int i = 0;
        while (i < repertoire.Length)
        {
            Vector2 pos = new Vector2(spawnDirection * (offset + offset * i) + transform.position.x, transform.position.y);
            Instantiate(repertoire[i], pos, Quaternion.identity);
            i++;
        }
    }
}
