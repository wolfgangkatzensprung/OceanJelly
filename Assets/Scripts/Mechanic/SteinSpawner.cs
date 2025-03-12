using UnityEngine;

public class SteinSpawner : MonoBehaviour
{
    public GameObject kleinerStein;
    public GameObject rollendesFelschen;

    float spawnCd = 6f;
    float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnCd)
        {
            SpawnSteine();
            timer = 0f;
        }
    }

    void SpawnSteine()
    {
        Instantiate(rollendesFelschen, transform.position, Quaternion.identity);
        Instantiate(kleinerStein, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
    }
}
