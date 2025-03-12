using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    PlayerHealth ph;

    private void Start()
    {
        ph = FindObjectOfType<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetString("checkpointScene", gameObject.scene.name);
            PlayerPrefs.SetFloat("checkpointX", transform.position.x);
            PlayerPrefs.SetFloat("checkpointY", transform.position.y);
            Debug.Log($"Checkpoint set to {gameObject.scene.name}: {transform.position.x}/{transform.position.y}");

            UIManager.Instance.ShowSaveCircle();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, .32f);
        Gizmos.DrawSphere(transform.position, 15);
    }
}
